using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

    public int Chapter { get; set; }
    public int Stage { get; set; }

    public const int MaxChapter = 12;
    public const int MaxStage = 20;

    private int moveCount;
    public int MoveCount {
        get { return moveCount; }
        set { moveCount = value; numberOfMovesText.GetComponent<Text>().text = value.ToString(); }
    }

    public int[,] AllShortestPathLengths { get; set; }
    public int[] ShortestPath { get; set; }
    public int ShortestPathLength;

    public TextAsset streamfile;

    [SerializeField]
    private GameObject numberOfMovesText;
    public GameObject stageClearPanel, lastStageClearPanel, nextChapterConfirmPanel;
    [SerializeField]
    private Transform nextBtn;

    public bool IsInEndGame { get; set; }

    private Cube cube;

    void Start() {
        ReadSolution();
        LevelManager.Instance.LoadAllStageData();
        Player.Instance.Startup();
        AudioManager.Instance.Startup();
        AnimationManager.Instance.Startup();

        cube = Cube.Instance;

        TestOnStartup();
    }

    private void Update() {
        HandleMovement();
    }


    int currentX, currentY, newX, newY;
    private void HandleMovement() {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        Direction dir;
        if (Input.GetMouseButtonUp(0) && currentX + currentY != 0) {
            CubePosition CP = Cube.Instance.position;
            GridPosition GP;
            GP = new GridPosition(CP.x + currentX, CP.y + currentY);
            LevelManager.Instance.tilesDict[GP].GetComponent<Tile>().LightOff();
            if (currentX == -1)
                dir = Direction.left;
            else if (currentX == 1)
                dir = Direction.right;
            else if (currentY == -1)
                dir = Direction.back;
            else
                dir = Direction.forward;
            currentX = 0;
            currentY = 0;
            Cube.Instance.Move(dir);
        }
        else if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layermask;
            layermask = 1 << 9; // ground-only
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask)) {
                newX = 0; newY = 0;
                float dx = hit.point.x - cube.transform.position.x;
                float dz = hit.point.z - cube.transform.position.z;
                if (Mathf.Abs(dx) > Mathf.Abs(dz)) {
                    if (dx < -Utilities.TILE_SIZE)
                        newX = -1;
                    else if (dx > Utilities.TILE_SIZE)
                        newX = 1;
                    else
                        newX = 0;
                }
                else {
                    if (dz < -Utilities.TILE_SIZE)
                        newY = -1;
                    else if (dz > Utilities.TILE_SIZE)
                        newY = 1;
                    else
                        newY = 0;
                }
            }
            if (newX == currentX && newY == currentY)
                return;
            CubePosition CP = Cube.Instance.position;
            GridPosition GP;
            GP = new GridPosition(CP.x + currentX, CP.y + currentY);
            LevelManager.Instance.tilesDict[GP].GetComponent<Tile>().LightOff();
            currentX = newX; currentY = newY;
            GP = new GridPosition(CP.x + newX, CP.y + newY);
            if (GP.x < 0 || GP.x >= LevelManager.Instance.Cols || GP.y < 0 || GP.y >= LevelManager.Instance.Rows || LevelManager.Instance.obstacles.Contains(GP)) {
                newX = 0; newY = 0;
                currentX = 0; currentY = 0;
                return;
            }
            LevelManager.Instance.tilesDict[GP].GetComponent<Tile>().LightOn();
        }
    }


    private void ReadSolution() {
        LevelManager.Instance.LoadSolutionData();
    }

    public void CompleteStage() {
        AudioManager.Instance.PlayStageClearSound();
        if (Chapter == MaxChapter & Stage == MaxStage)
            AnimationManager.Instance.PlayEndGameAnimation();
        else
            StartCoroutine("ICompleteStage");
    }

    private IEnumerator ICompleteStage() {
        Cube.Instance.cannotMove = true;
        stageClearPanel.SetActive(true);
        nextChapterConfirmPanel.SetActive(false);
        int stars;

        int moveMin = AllShortestPathLengths[Chapter - 1, Stage - 1];

        if (MoveCount == moveMin)
            stars = 3;
        else if ((moveMin >= 30 & MoveCount <= moveMin + 4) || MoveCount <= moveMin + 2)
            stars = 2;
        else stars = 1;
        int improvement = stars - Player.Instance.Star[Chapter - 1, Stage - 1];
        if (improvement > 0) {    
            if (Chapter < MaxChapter & Player.Instance.ChapterStar[Chapter - 1] < 42 & Player.Instance.ChapterStar[Chapter - 1] + improvement >= 42)
                AnimationManager.Instance.PlayChapterUnlockAnim(Chapter + 1);
            Player.Instance.UpdateProgress(Chapter, Stage, stars);
        }
        if (Stage == MaxStage & Player.Instance.ChapterStar[Chapter - 1] < 42)
            nextBtn.GetChild(1).gameObject.SetActive(true);
        else nextBtn.GetChild(1).gameObject.SetActive(false);

        AnimationManager.Instance.PlayStageClearAnim(stars, stageClearPanel.transform.GetChild(3));
        yield return null;
    }

    public void EndGame() {
        AnimationManager.Instance.PlayEndGameAnimation();
    }

    public IEnumerator ICompleteLastStage() {
        Cube.Instance.cannotMove = true;
        lastStageClearPanel.SetActive(true);
        int stars;
        if (MoveCount == AllShortestPathLengths[Chapter - 1, Stage - 1])
            stars = 3;
        else if (MoveCount <= AllShortestPathLengths[Chapter - 1, Stage - 1] + 4)
            stars = 2;
        else stars = 1;
        if (stars > Player.Instance.Star[MaxChapter - 1, MaxStage - 1])
            Player.Instance.UpdateProgress(Chapter, Stage, stars);

        AnimationManager.Instance.PlayStageClearAnim(stars, lastStageClearPanel.transform.GetChild(3));
        yield return null;
    }

    public void ShowNextChapterConfirmPanel() {
        nextChapterConfirmPanel.transform.GetChild(1).GetComponent<Text>().text 
            = "All stages in Chapter " + Chapter + " have been cleared. Move to Chapter " + (Chapter + 1) + "?";
        stageClearPanel.SetActive(false);
        nextChapterConfirmPanel.SetActive(true);
    }

    public void HideNextChapterConfirmPanel() {
        stageClearPanel.SetActive(true);
        nextChapterConfirmPanel.SetActive(false);
    }

    public void OnNextBtnClicked() {
        if (Stage == MaxStage)
            ShowNextChapterConfirmPanel();
        else
            NextStage();
    }

    private void NextStage() {
        //ButtonManager.Instance.GetChanceToShowAd();
        stageClearPanel.SetActive(false);
        nextChapterConfirmPanel.SetActive(false);
        Stage++;
        if (Stage > MaxStage) {
            Stage = 1;
            Chapter = Chapter + 1;
        }
        ButtonManager.Instance.SelectedChapter = Chapter;
        ButtonManager.Instance.SelectedStage = Stage;
        LevelManager.Instance.EnterLevel(Chapter, Stage);
    }

    public void PrevStageDev() {
        if (Stage == 1 && Chapter == 1)
            return;
        stageClearPanel.SetActive(false);
        nextChapterConfirmPanel.SetActive(false);
        Stage--;
        if (Stage == 0)
        {
            Stage = MaxStage;
            Chapter--;
        }
        ButtonManager.Instance.SelectedChapter = Chapter;
        ButtonManager.Instance.SelectedStage = Stage;
        LevelManager.Instance.EnterLevel(Chapter, Stage, true);
    }


    public void NextStageDev() {
        if (Stage == MaxStage && Chapter == MaxChapter)
            return;
        stageClearPanel.SetActive(false);
        nextChapterConfirmPanel.SetActive(false);
        Stage++;
        if (Stage > MaxStage) {
            Stage = 1;
            Chapter++;
        }
        ButtonManager.Instance.SelectedChapter = Chapter;
        ButtonManager.Instance.SelectedStage = Stage;
        LevelManager.Instance.EnterLevel(Chapter, Stage, true);
    }

    private void TestOnStartup() {
        
    }

}
