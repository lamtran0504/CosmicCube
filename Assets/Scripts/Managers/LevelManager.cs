using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager> {

    [SerializeField]
    private TextAsset solutionDataTxt;
    
    [SerializeField]
    public TextAsset[] stageDataTxt;

    [SerializeField]
    private TextAsset[] stageSolutionTxt;

    public int[,] SolutionData { get; set; }
    private int[,][] allStageData;

    public Transform tilesTransform;
    [SerializeField]
    private GameObject tilePrefab, planePrefab, groundPrefab, dustPrefab, levelText;
    [SerializeField]
    private Material purple, blue, red, orange, green, yellow;
    
    private List<Material> tileColors;

    public GameObject testCubeObject;

    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public Vector3 MapStart { get; private set; }
    public CubePosition start;
    public List<GridPosition>[] targets;
    public List<GridPosition> obstacles;

    public Dictionary<GridPosition, GameObject> targetDict;

    public Dictionary<GridPosition, GameObject> tilesDict;

    public const float TILE_SIZE = 1.1f;

    public Vector3 TileToWorldPos(int x, int y) {
        return MapStart + new Vector3((x - (Cols - 1f) / 2) * TILE_SIZE, 0, (y - (Rows - 1f) / 2) * TILE_SIZE);
    }

    public Vector3 CubeToWorldPos(int x, int y) {
        return MapStart + new Vector3((x - (Cols - 1f) / 2) * TILE_SIZE, Utilities.CUBE_SIZE / 2 + Utilities.TILE_H / 2, (y - (Rows - 1f) / 2) * TILE_SIZE);
    }

    private void BuildTiles(int rows, int cols, List<GridPosition>[] targets, List<GridPosition> obstacles, Vector3 mapStart) {
        MapStart = mapStart;
        this.obstacles = obstacles;

        int tileIndex = 6;
        tilesDict = new Dictionary<GridPosition, GameObject>();
        List<Material> tileColorList = new List<Material> { null, null, null, null, null, null };
        int pointerColor = 0;
        for (int i = 0; i < 6; i++) {
            if (targets[i].Count > 0)
                tileColorList[i] = tileColors[pointerColor++];
        }


        Instantiate(groundPrefab, mapStart, Quaternion.identity).transform.SetParent(tilesTransform);
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {

                GridPosition gp = new GridPosition(c, r);
                if (obstacles != null)
                    if (obstacles.Contains(gp)) continue;
                GameObject tileObj;
                for (int i = 0; i < 6; i++)
                    if (targets[i].Contains(gp)) { tileIndex = i; continue; }

                tileObj = Instantiate(tilePrefab, TileToWorldPos(c, r), Quaternion.identity);
                tileObj.GetComponent<Tile>().x = c;
                tileObj.GetComponent<Tile>().y = r;

                if (tileIndex < 6) {
                    GameObject colorPlane = Instantiate(planePrefab, TileToWorldPos(c, r), Quaternion.identity);
                    colorPlane.GetComponent<MeshRenderer>().material = tileColorList[tileIndex];
                    colorPlane.transform.SetParent(tileObj.transform);
                    targetDict.Add(gp, tileObj);
                }
                tileObj.transform.SetParent(tilesTransform);
                tilesDict.Add(gp, tileObj);
                tileIndex = 6;
            }
        }
    }

    private void SetupColors(int chapter) {
        switch (chapter) {
            case 1: tileColors = new List<Material> { blue }; break;
            case 2: tileColors = new List<Material> { yellow }; break;
            case 3: tileColors = new List<Material> { blue }; break;
            case 4: tileColors = new List<Material> { red }; break;
            case 5: tileColors = new List<Material> { purple }; break;
            case 6: tileColors = new List<Material> { yellow }; break;
            case 7: tileColors = new List<Material> { green }; break;
            default: tileColors = new List<Material> { purple, blue, red, orange, green, yellow }; break;
        }
    }

    public void EnterLevel(int chapter, int stage, bool withoutAnimation = false) {
        
        if (Player.Instance.CurrentChapter < chapter) {
            ButtonManager.Instance.ChangeScene(0);
            return;
        }

        if (AnimationManager.Instance.AllStagesClearedObj != null)
            Destroy(AnimationManager.Instance.AllStagesClearedObj);

        
        GetStageData(chapter, stage);

        SetupColors(chapter);
        List<int> cubeColors = new List<int>();
        for (int i = 0; i < 6; i++)
            Cube.Instance.faces[i].enabled = false;
        int colorPointer = 0;
        for (int i = 0; i < 6; i++) {
            if (targets[i].Count > 0) {
                Cube.Instance.faces[i].enabled = true;
                Cube.Instance.faces[i].material = tileColors[colorPointer++];
            }
        }

        Cube.Instance.StartNewStage(new CubePosition(start.x, start.y, Rotation.Start));

        Vector3 cubePos = Cube.Instance.transform.position;
        Vector3 mapStart;
        if (!withoutAnimation) { mapStart = cubePos + new Vector3(((Cols - 1f)/2 - start.x) * TILE_SIZE, -20, ((Rows - 1f)/2 - start.y) * TILE_SIZE); ExitLevel(2); }
        else { mapStart = cubePos + new Vector3(((Cols - 1f)/2 - start.x) * TILE_SIZE, -TILE_SIZE / 2 - 0.05f, ((Rows - 1f)/2 - start.y) * TILE_SIZE); ExitLevel(0); }

        targetDict = new Dictionary<GridPosition, GameObject>();

        BuildTiles(Rows, Cols, targets, obstacles, mapStart);
    
        Camera.main.GetComponent<CameraScript>().EnterNewStage(mapStart, withoutAnimation);

        levelText.GetComponent<Text>().text = "Level " + chapter + " - " + stage;
        GameManager.Instance.MoveCount = 0;
   
        GameManager.Instance.lastStageClearPanel.SetActive(false);
        foreach (GameObject ui in ButtonManager.Instance.inGameBtns)
            ui.SetActive(true);

    }

    public void GetStageData2(int chapter, int stage) {

        int[] data = allStageData[chapter - 1, stage - 1];
        if (data == null) { Debug.Log("Failed to load level data: " + chapter.ToString() + " - " + stage); return; }
        if (data.Length < 5) { Debug.Log("Failed to load level data: " + chapter.ToString() + " - " + stage); return; }

        obstacles = new List<GridPosition>();


        try {
            int pointer = 0;
            Rows = data[pointer];
            Cols = data[++pointer];
            start = new CubePosition(data[++pointer], data[++pointer], Rotation.Start);
            int targetCount = data[++pointer];
            targets = new List<GridPosition>[6];
            for (int i = 0; i < 6; i++)
                targets[i] = new List<GridPosition>();
            for (int i = 0; i < targetCount; i++) {
                int color = data[++pointer];
                targets[color].Add(new GridPosition(data[++pointer], data[++pointer]));
            }
            int obstacleCount = data[++pointer];
            for (int i = 0; i < obstacleCount; i++)
                obstacles.Add(new GridPosition(data[++pointer], data[++pointer]));
        }
        catch (IndexOutOfRangeException) { Debug.Log("Failed to load level data: " + chapter + " - " + stage); return; }

    }

    public void GetStageData(int chapter, int stage) {

        int[] data = Array.ConvertAll(stageDataTxt[chapter - 1].text.Split('\n')[stage - 1].Split(' '), int.Parse);
        if (data == null) { Debug.Log("Failed to load level data: " + chapter.ToString() + " - " + stage); return; }
        if (data.Length < 5) { Debug.Log("Failed to load level data: " + chapter.ToString() + " - " + stage); return; }

        obstacles = new List<GridPosition>();


        try
        {
            int pointer = 0;
            Rows = data[pointer];
            Cols = data[++pointer];
            start = new CubePosition(data[++pointer], data[++pointer], Rotation.Start);
            int targetCount = data[++pointer];
            targets = new List<GridPosition>[6];
            for (int i = 0; i < 6; i++)
                targets[i] = new List<GridPosition>();
            for (int i = 0; i < targetCount; i++)
            {
                int color = data[++pointer];
                targets[color].Add(new GridPosition(data[++pointer], data[++pointer]));
            }
            int obstacleCount = data[++pointer];
            for (int i = 0; i < obstacleCount; i++)
                obstacles.Add(new GridPosition(data[++pointer], data[++pointer]));
        }
        catch (IndexOutOfRangeException) { Debug.Log("Failed to load level data: " + chapter + " - " + stage); return; }

    }

    public void LoadSolutionData() {
        SolutionData = new int[GameManager.MaxChapter, GameManager.MaxStage];
        string dataTxt = solutionDataTxt.text;
        string[] chapterSolTxtArr = dataTxt.Split('\n');
        if (chapterSolTxtArr.Length != GameManager.MaxChapter) {
            Debug.Log(chapterSolTxtArr.Length + " - " + GameManager.MaxChapter);
            GameManager.Instance.AllShortestPathLengths = SolutionData;
            return;
        }
        
        for (int c = 0; c < GameManager.MaxChapter; c++) {
            string[] stageSolTxtArr = chapterSolTxtArr[c].Split(' ');
            if (stageSolTxtArr.Length != GameManager.MaxStage)
                 return;
            for (int s = 0; s < GameManager.MaxStage; s++)
                SolutionData[c, s] = Int32.Parse(stageSolTxtArr[s]);
        }

        GameManager.Instance.AllShortestPathLengths = SolutionData;
    }

    public void LoadAllStageData() {
        int mc = GameManager.MaxChapter;
        int ms = GameManager.MaxStage;
        allStageData = new int[mc, ms][];

        string[] chapterDataArr;
        for (int c = 0; c < mc; c++) {
            chapterDataArr = stageDataTxt[c].text.Split('\n');

            for (int s = 0; s < ms; s++) {
                string[] stageDataArr = chapterDataArr[s].Split(' ');

                int dataLength = stageDataArr.Length;
                allStageData[c, s] = new int[dataLength];
                for (int i = 0; i < dataLength; i++) {
                    try {
                        allStageData[c, s][i] = Int32.Parse(stageDataArr[i]);
                    }
                    catch (FormatException) { Debug.Log(c + ", " + s + ", " + i);  return; }
                }
            }
        }
    }

    public void LoadSolution() {
        int c = GameManager.Instance.Chapter;
        int s = GameManager.Instance.Stage;
        int[] solution = new int[SolutionData[c - 1, s - 1]];

        string dataTxt = stageSolutionTxt[c - 1].text;

        dataTxt = dataTxt.Split('\n')[s - 1];

        string[] moveTxtArr = dataTxt.Split(' ');

        if (moveTxtArr.Length != solution.Length)
            return;
        int[] moveArr = new int[solution.Length];

        try {
            for (int i = 0; i < solution.Length; i++) {
                moveArr[i] = Int32.Parse(moveTxtArr[i]);
            }
        }
        catch (FormatException) { GameManager.Instance.ShortestPath = null; return; }

        int startPos = start.x * 10 + start.y;

        int move = moveArr[0] - startPos;
        solution[0] = GetNextMove(move);
        if (solution[0] == -2) {
            Debug.Log("Solution File Corrupted " + c + " - " + s);
            GameManager.Instance.ShortestPath = null;
            return;
        }

        for (int i = 1; i < solution.Length; i++) {
            move = moveArr[i] - moveArr[i - 1];
            solution[i] = GetNextMove(move);
            if (solution[i] == -2) {
                Debug.Log("Solution File Corrupted");
                GameManager.Instance.ShortestPath = null;
                return;
            }
        }
        GameManager.Instance.ShortestPath = solution;
    }

    private int GetNextMove(int n) {
        if (n == 10) return -1;      // right;
        if (n == 1) return 0;       // forward;
        if (n == -1) return 2;      // back;
        if (n == -10) return 1;    // left;
        return -2;
    }

    public void ExitLevel(int time, bool exitGameplay = false) {
        if (exitGameplay) { time = 0; }
        if (targetDict != null) {
            List<GameObject> targetObjList = targetDict.Values.ToList();
            for (int i = 0; i < targetObjList.Count; i++)
            if (targetObjList[i] != null)
                Destroy(targetObjList[i]);
            targetDict = null;
        }
        Cube.Instance.isFalling = true;
        foreach (Transform t in tilesTransform)
            Destroy(t.gameObject, time);
    }

    public void RemoveTarget(GridPosition target) {
        if (!targetDict.ContainsKey(target)) {
            Debug.Log("No target with such position was found.");
            return;
        }
        if (targetDict.Count == 1)
            Destroy(targetDict[target]);
        else
            Destroy(targetDict[target].transform.GetChild(0).gameObject);
        targetDict.Remove(target);
    }  

}
