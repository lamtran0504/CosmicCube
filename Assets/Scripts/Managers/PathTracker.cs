using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PathTracker : Singleton<PathTracker> {

    private List<CubePosition> currentCubePositions;
    private List<CubePosition>[] allCubePositions;
    private int pointer;

    private Tracker0 tracker0;
    private Tracker1 tracker1;
    private Tracker2 tracker2;

    private List<int>[,] solution;
    
    //
    //
    private void SetUpCubePositions() {
        CubePosition start = LevelManager.Instance.start;
        int rows = LevelManager.Instance.Rows;
        int cols = LevelManager.Instance.Cols;
        allCubePositions = new List<CubePosition>[2] { new List<CubePosition>(), new List<CubePosition>() };
        
        List<Rotation> rotListLR = new List<Rotation>();
        List<Rotation> rotListFB = new List<Rotation>();
        List<Rotation> rotList0 = new List<Rotation>();
        List<Rotation> rotList1 = new List<Rotation>();
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 6; j++) {
                if ((i + 1) % 3 == j % 3)
                    rotListFB.Add(new Rotation(0, new Direction(i), new Direction(j)));
                else if (i % 3 == (j + 1) % 3)
                    rotListLR.Add(new Rotation(0, new Direction(i), new Direction(j)));
            }
        }
        if (rotListLR.Contains(start.rotation)) { rotList0 = rotListLR; rotList1 = rotListFB; }
        else { rotList0 = rotListFB; rotList1 = rotListLR; }

        int m = (start.x + start.y) % 2;
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                if ((r + c) % 2 == m) {
                    foreach (Rotation rot in rotList0)
                        allCubePositions[0].Add(new CubePosition(c, r, rot));
                    foreach (Rotation rot in rotList1)
                        allCubePositions[1].Add(new CubePosition(c, r, rot));
                }
                else {
                    foreach (Rotation rot in rotList1)
                        allCubePositions[0].Add(new CubePosition(c, r, rot));
                    foreach (Rotation rot in rotList0)
                        allCubePositions[1].Add(new CubePosition(c, r, rot));
                }
            }
        }

        allCubePositions[0] = allCubePositions[0].OrderBy(cp => cp.GetHashCode()).ToList();
        allCubePositions[1] = allCubePositions[1].OrderBy(cp => cp.GetHashCode()).ToList();

        pointer = 0;
        currentCubePositions = allCubePositions[0];
    }

    private void SwitchCubePositionsList() {
        pointer = 1 - pointer;
        currentCubePositions = allCubePositions[pointer];
    }

    private void Start() {
        tracker0 = new Tracker0();
        tracker1 = new Tracker1();
        tracker2 = new Tracker2();
        ReadSolutionData();       
    }

    private void ReadSolutionData() {
        solution = new List<int>[GameManager.MaxChapter, GameManager.MaxStage];
        for (int c = 0; c < GameManager.MaxChapter; c++)
            for (int s = 0; s < GameManager.MaxStage; s++)
                solution[c, s] = new List<int>();

        string dir;
        StreamReader file;
        for (int c = 0; c < GameManager.MaxChapter; c++) {
            dir = "Assets/Data/Solution/" + (c + 1) + ".txt";
            if (!File.Exists(dir)) {
                File.Create(dir);
                continue;
            }
            file = new StreamReader(dir);
            for (int s = 0; s < GameManager.MaxStage; s++) {
                if (file.Peek() == -1)
                    break;
                string[] pathStrList = file.ReadLine().Split(' ');
                try {
                    foreach (string str in pathStrList)
                        solution[c, s].Add(Int32.Parse(str));
                }
                catch (FormatException) { Debug.Log("Failed to read solution data - Stage " + (c + 1) + " - " + (s + 1)); }
            }
            file.Close();
        }
    }

    public void UpdateSolution(int chapter, int stage) {
        Debug.Log("Updating " + chapter + " - " + stage);
        Path path = Path.blank;
        //if (LevelManager.Instance.obstacles.Count == 0) {
        //    int first = -1;
        //    bool useTracker1 = false;
        //    for (int i = 0; i < 6; i++) {
        //        if (LevelManager.Instance.targets[i].Count == 0)
        //            continue;
        //        if (first == -1)
        //            first = i;
        //        else if (i != first + 3) {
        //            if (currentCubePositions == null) SetUpCubePositions();
        //            else if (!currentCubePositions.Contains(LevelManager.Instance.start)) SwitchCubePositionsList();
        //            path = tracker1.GetPath();
        //            useTracker1 = true;
        //            break;
        //        }
        //    }
        //    if (!useTracker1)
        //        path = tracker0.GetPath();
        //}
        //else {
            if (currentCubePositions == null) SetUpCubePositions();
            else if (!currentCubePositions.Contains(LevelManager.Instance.start)) SwitchCubePositionsList();
            path = tracker2.GetPath();
        //}

        solution[chapter - 1, stage - 1] = new List<int>();
        foreach (GridPosition move in path.moves)
            solution[chapter - 1, stage - 1].Add(move.GetHashCode());

        string dir = "Assets/Data/Solution/" + chapter + ".txt";
        StreamWriter writer = new StreamWriter(dir, false);
        for (int i = 0; i < GameManager.MaxStage; i++) {
            if (solution[chapter - 1, i].Count == 0) {
                writer.WriteLine("");
                continue;
            }
            writer.Write(solution[chapter - 1, i][0]);
            for (int j = 1; j < solution[chapter - 1, i].Count; j++)
                writer.Write(" " + solution[chapter - 1, i][j]);
            if (i <= GameManager.MaxStage - 1)
                writer.Write('\n');

        }       
        writer.Flush();
        writer.Close();

        dir = "Assets/Data/Solution.txt";
        writer = new StreamWriter(dir, false);
        writer.Write(solution[0, 0].Count);
        for (int s = 1; s < GameManager.MaxStage; s++)
            writer.Write(" " + solution[0, s].Count);
        writer.Write('\n');
        for (int c = 1; c < GameManager.MaxChapter; c++) {
            writer.Write(solution[c, 0].Count);
            for (int s = 1; s < GameManager.MaxStage; s++)
                writer.Write(" " + solution[c, s].Count);
            writer.Write('\n');
        }
        writer.Flush();
        writer.Close();
    }

    public void UpdateSolution() {
        float t = Time.realtimeSinceStartup * 1000;
        UpdateSolution(GameManager.Instance.Chapter, GameManager.Instance.Stage);
        Debug.Log((Time.realtimeSinceStartup * 1000 - t) + " ms");
        ShowPathLength();
    }

    private void ShowPathDebug() {
        Path p = Path.blank;
        foreach (int n in solution[GameManager.Instance.Chapter - 1, GameManager.Instance.Stage - 1])
            p = new Path(p, new GridPosition((n - n % 10) / 10, n % 10));
        Debug.Log(p);
    }

    public void UpdateShortestPath() {
        GameManager.Instance.ShortestPathLength = solution[GameManager.Instance.Chapter - 1, GameManager.Instance.Stage - 1].Count;
        ShowPathDebug();
    }

    public void ShowPathLength() {
        ShowPathDebug();
    }


    public void FixDataFiles() {
        string[] dataStr = new string[GameManager.MaxChapter];
        string dir;
        for (int c = 0; c < GameManager.MaxChapter; c++) {
            dataStr = LevelManager.Instance.stageDataTxt[c].text.Split('\n');

            dir = "Assets/Data/" + (c + 1) + ".txt";
            StreamWriter writer = new StreamWriter(dir, false);
            for (int s = 0; s < GameManager.MaxStage; s++) {
                writer.Write("9 5 " + dataStr[s]);
                if (s < GameManager.MaxStage - 1)
                    writer.Write('\n');
            }
            writer.Flush();
            writer.Close();
        }     
    }


    public struct Path {
        public int Length { get { return (moves == null) ? int.MaxValue : moves.Length; } }
        public GridPosition[] moves;
        public static Path blank = new Path(new GridPosition[0]);
        public override string ToString() {
            string pathStr = "";
            if (moves == null)
                return pathStr;
            if (moves.Length == 0)
                return pathStr;
            pathStr += moves[0].ToString();
            for (int i = 1; i < moves.Length; i++)
                pathStr += moves[i].ToString();
            return pathStr;
        }
        public Path(Path path0, Path path1) {
            if (path0.moves == null || path1.moves == null) {
                this.moves = null;
                return;
            }
            GridPosition[] moves = new GridPosition[path0.Length + path1.Length];
            for (int i = 0; i < path0.Length; i++)
                moves[i] = path0.moves[i];
            for (int i = 0; i < path1.Length; i++)
                moves[path0.Length + i] = path1.moves[i];
            this.moves = moves;
        }
        public Path(Path oldPath, GridPosition nextMove) {
            GridPosition[] moves = new GridPosition[oldPath.Length + 1];
            for (int i = 0; i < oldPath.Length; i++)
                moves[i] = oldPath.moves[i];
            moves[moves.Length - 1] = nextMove;
            this.moves = moves;
        }
        public Path(GridPosition[] moves) {
            this.moves = moves;
        }
        public static bool operator ==(Path path0, Path path1) {
            if (path0.Length != path1.Length)
                return false;
            if (path0.moves == null & path1.moves == null)
                return true;
            if (path0.moves == null || path1.moves == null)
                return false;
            for (int i = 0; i < path0.Length; i++)
                if (path0.moves[i] != path1.moves[i])
                    return false;
            return true;
        }
        public static bool operator !=(Path path0, Path path1) {
            if (path0.Length != path1.Length)
                return true;
            if (path0.moves == null & path1.moves == null)
                return false;
            if (path0.moves == null || path1.moves == null)
                return true;
            for (int i = 0; i < path0.Length; i++)
                if (path0.moves[i] != path1.moves[i])
                    return true;
            return false;
        }
        public override bool Equals(object obj) {
            if (obj == null || !(obj is Path))
                return false;
            return this == (Path)obj;
        }
    }

    // No obstacles, single color/2 colors of opposite sides
    private class Tracker0 {

        private Path[,] allPaths;

        private Path GetPathSingleTarget(FacePosition start, FacePosition target) {
            return allPaths[start.GetHashCode(), target.GetHashCode()];
        }

        private void FindAllPaths() {
            int rows = LevelManager.Instance.Rows;
            int cols = LevelManager.Instance.Cols;

            List<FacePosition> allFacePositions = new List<FacePosition>();

            for (int x = 0; x < cols; x++) {
                for (int y = 0; y < rows; y++) {
                    for (int d = 0; d < 6; d++)
                        allFacePositions.Add(new FacePosition(x, y, new Direction(d)));
                }
            }

            int arraySize = allFacePositions[allFacePositions.Count - 1].GetHashCode() + 1;
            allPaths = new Path[arraySize, arraySize];
            for (int i = 0; i < arraySize; i++)
                for (int j = 0; j < arraySize; j++)
                    allPaths[i, j] = Path.blank;

            for (int posIndex = 0; posIndex < allFacePositions.Count; posIndex++) {
                FacePosition position = allFacePositions[posIndex];

                int hashCode = position.GetHashCode();
                Queue<FacePosition> positionQ = new Queue<FacePosition>();
                positionQ.Enqueue(position);
                FacePosition current;
                List<FacePosition> nexts;

                while (positionQ.Count > 0) {
                    current = positionQ.Dequeue();
                    int currentHashCode = current.GetHashCode();
                    Path currentPath = allPaths[hashCode, currentHashCode];
                    nexts = new List<FacePosition>();

                    if (current.y < rows - 1)
                        nexts.Add(new FacePosition(current.x, current.y + 1, current.direction.GetRotatedVector(Direction.forward)));
                    if (current.y > 0)
                        nexts.Add(new FacePosition(current.x, current.y - 1, current.direction.GetRotatedVector(Direction.back)));
                    if (current.x > 0)
                        nexts.Add(new FacePosition(current.x - 1, current.y, current.direction.GetRotatedVector(Direction.left)));
                    if (current.x < cols - 1)
                        nexts.Add(new FacePosition(current.x + 1, current.y, current.direction.GetRotatedVector(Direction.right)));

                    foreach (FacePosition next in nexts) {
                        int nextHashCode = next.GetHashCode();
                        if (allPaths[hashCode, nextHashCode].moves.Length == 0) {
                            
                            allPaths[hashCode, nextHashCode] = new Path(currentPath, new GridPosition(next.x, next.y));
                            positionQ.Enqueue(next);
                        }
                    }
                }
            }
        }

        // targetsData's member array: color, x, y (color can only be 0 or 3)
        private Path GetPathMultipleTargets(FacePosition start, List<int[]> targetsData) {
            if (targetsData.Count == 0)
                return Path.blank;
            Path[] potentialPaths = new Path[targetsData.Count * 2];

            for (int i = 0; i < targetsData.Count; i++) {
                List<int[]> nextTargestData = new List<int[]>();
                for (int j = 0; j < i; j++)
                    nextTargestData.Add(targetsData[j]);
                for (int j = i + 1; j < targetsData.Count; j++)
                    nextTargestData.Add(targetsData[j]);

                FacePosition cp = new FacePosition(targetsData[i][1], targetsData[i][2], (targetsData[i][0] == 0) ? Direction.down : Direction.up);

                try {
                    potentialPaths[i] = new Path(GetPathSingleTarget(start, cp), GetPathMultipleTargets(cp, nextTargestData));
                }
                catch (OutOfMemoryException) { Debug.Log("Invalid position(s)"); return new Path(null); };
            }

            Path pathMin = new Path(null);
            foreach (Path p in potentialPaths) {
                if (p.Length < pathMin.Length)
                    pathMin = p;
            }

            return pathMin;
        }
     
        public Path GetPath() {
            if (allPaths == null)
                FindAllPaths();
            List<int[]> targetsData = new List<int[]>();
            int firstFace = -1;
            FacePosition start;
            Direction startDirection = Direction.down;

            for (int i = 0; i < 6; i++) {
                if (LevelManager.Instance.targets[i].Count == 0)
                    continue;
                if (firstFace == -1) {
                    firstFace = i;                 
                    switch (i) {
                        case 0: startDirection = Direction.down; break;
                        case 1: startDirection = Direction.back; break;
                        case 2: startDirection = Direction.left; break;
                        case 3: startDirection = Direction.up; break;
                        case 4: startDirection = Direction.forward; break;
                        case 5: startDirection = Direction.right; break;
                        default: Debug.Log("Color can only range from 0 to 5."); return Path.blank;
                    }
                    start = new FacePosition();
                    foreach (GridPosition gp in LevelManager.Instance.targets[i])
                        targetsData.Add(new int[] { 0, gp.x, gp.y });
                }
                else if (i == firstFace + 3) {
                    foreach (GridPosition gp in LevelManager.Instance.targets[i])
                        targetsData.Add(new int[] { 3, gp.x, gp.y });
                }
                else {
                    Debug.Log("Wrong Pathtracker in use");
                    return Path.blank;
                }
            }

            start = new FacePosition(LevelManager.Instance.start.x, LevelManager.Instance.start.y, startDirection);
            return GetPathMultipleTargets(start, targetsData);
        }
    }

    // No obstacles, multiple colors
    private class Tracker1 {

        private Path[,] currentPathList;
        private Path[][,] allPaths;

        private void SetUpPathsArray() {
            if (currentPathList != null) {
                currentPathList = allPaths[Instance.pointer];
                return;
            }

            allPaths = new Path[2][,];
            int arraySize = Instance.allCubePositions[0][Instance.allCubePositions[0].Count - 1].GetHashCode() + 1;
            allPaths[0] = new Path[arraySize, arraySize];
            arraySize = Instance.allCubePositions[1][Instance.allCubePositions[1].Count - 1].GetHashCode() + 1;
            allPaths[1] = new Path[arraySize, arraySize];

            for (int i = 0; i < 2; i++)
                foreach (CubePosition cp in Instance.allCubePositions[i])
                    allPaths[i][cp.GetHashCode(), cp.GetHashCode()] = Path.blank;
            UpdateShortestPathData(0);
            UpdateShortestPathData(1);
            currentPathList = allPaths[Instance.pointer];
        }

        private Path GetPathSingleTarget(CubePosition start, CubePosition target) {
            if (!Instance.currentCubePositions.Contains(start)) { Debug.Log("Invalid start position"); return new Path(null); }
            if (!Instance.currentCubePositions.Contains(target)) { Debug.Log("Invalid target position"); return new Path(null); }
            return currentPathList[start.GetHashCode(), target.GetHashCode()];
        }

        private void UpdateShortestPathData(int pointer) {
            int rows = LevelManager.Instance.Rows;
            int cols = LevelManager.Instance.Cols;
            for (int posIndex = 0; posIndex < Instance.allCubePositions[pointer].Count; posIndex++) {
                CubePosition position = Instance.allCubePositions[pointer][posIndex];
                int hashCode = position.GetHashCode();
                Queue<CubePosition> positionQ = new Queue<CubePosition>();
                positionQ.Enqueue(position);
                CubePosition current;
                List<CubePosition> nexts;

                while (positionQ.Count > 0) {
                    current = positionQ.Dequeue();
                    int currentHashCode = current.GetHashCode();
                    Path currentPath = allPaths[pointer][hashCode, currentHashCode];
                    nexts = new List<CubePosition>();

                    if (current.y < rows - 1)
                        nexts.Add(new CubePosition(current.x, current.y + 1, current.rotation.GetNext(Direction.forward)));
                    if (current.y > 0)
                        nexts.Add(new CubePosition(current.x, current.y - 1, current.rotation.GetNext(Direction.back)));
                    if (current.x > 0)
                        nexts.Add(new CubePosition(current.x - 1, current.y, current.rotation.GetNext(Direction.left)));
                    if (current.x < cols - 1)
                        nexts.Add(new CubePosition(current.x + 1, current.y, current.rotation.GetNext(Direction.right)));

                    foreach (CubePosition next in nexts) {
                        int nextHashCode = next.GetHashCode();
                        if (allPaths[pointer][hashCode, nextHashCode].moves == null) {
                            allPaths[pointer][hashCode, nextHashCode] = new Path(currentPath, new GridPosition(next.x, next.y));
                            positionQ.Enqueue(next);
                        }
                    }
                }
            }
        }

        private Path GetPathMultipleTargetsUsingArray(CubePosition start, int[][] targetsData) {
            if (targetsData.Length == 0)
                return Path.blank;

            Path[] potentialPaths = new Path[targetsData.Length * 2];

            for (int i = 0; i < targetsData.Length; i++) {
                int[][] nextTargestData = new int[targetsData.Length - 1][];
                for (int j = 0; j < i; j++)
                    nextTargestData[j] = targetsData[j];
                for (int j = i + 1; j < targetsData.Length; j++)
                    nextTargestData[j - 1] = targetsData[j];

                CubePosition cp = new CubePosition(targetsData[i][1], targetsData[i][2], new Rotation(targetsData[i][0], Direction.down, Direction.right));
                CubePosition cp0, cp1;
                Direction d = (Instance.currentCubePositions.Contains(cp)) ? Direction.right : Direction.forward;
                cp0 = new CubePosition(targetsData[i][1], targetsData[i][2], new Rotation(targetsData[i][0], Direction.down, d));
                cp1 = new CubePosition(targetsData[i][1], targetsData[i][2], new Rotation(targetsData[i][0], Direction.down, -d));

                try {
                    potentialPaths[i * 2] = new Path(GetPathSingleTarget(start, cp0), GetPathMultipleTargetsUsingArray(cp0, nextTargestData));
                    potentialPaths[i * 2 + 1] = new Path(GetPathSingleTarget(start, cp1), GetPathMultipleTargetsUsingArray(cp1, nextTargestData));
                }
                catch (OutOfMemoryException) { Debug.Log("Invalid position(s)"); return new Path(null); };
            }

            Path pathMin = new Path(null);
            foreach (Path p in potentialPaths) {
                if (p.Length < pathMin.Length)
                    pathMin = p;
            }

            return pathMin;
        }

        private Path GetPathMultipleTargets(CubePosition start, List<int[]> targetsData) {
            if (targetsData.Count == 0)
                return Path.blank;
            Path[] potentialPaths = new Path[targetsData.Count * 2];

            for (int i = 0; i < targetsData.Count; i++) {
                List<int[]> nextTargestData = new List<int[]>();
                for (int j = 0; j < i; j++)
                    nextTargestData.Add(targetsData[j]);
                for (int j = i + 1; j < targetsData.Count; j++)
                    nextTargestData.Add(targetsData[j]);

                CubePosition cp = new CubePosition(targetsData[i][1], targetsData[i][2], new Rotation(targetsData[i][0], Direction.down, Direction.right));
                CubePosition cp0, cp1;
                Direction d = (Instance.currentCubePositions.Contains(cp)) ? Direction.right : Direction.forward;
                cp0 = new CubePosition(targetsData[i][1], targetsData[i][2], new Rotation(targetsData[i][0], Direction.down, d));
                cp1 = new CubePosition(targetsData[i][1], targetsData[i][2], new Rotation(targetsData[i][0], Direction.down, -d));

                try {
                    potentialPaths[i * 2] = new Path(GetPathSingleTarget(start, cp0), GetPathMultipleTargets(cp0, nextTargestData));
                    potentialPaths[i * 2 + 1] = new Path(GetPathSingleTarget(start, cp1), GetPathMultipleTargets(cp1, nextTargestData));
                }
                catch (OutOfMemoryException) { Debug.Log("Invalid position(s)"); return new Path(null); };
            }

            Path pathMin = new Path(null);
            foreach (Path p in potentialPaths)
            {
                if (p.Length < pathMin.Length)
                    pathMin = p;
            }

            return pathMin;
        }

        public Path GetPath() {
            SetUpPathsArray();
            List<int[]> targetsData = new List<int[]>();
            for (int i = 0; i < 6; i++)
                foreach (GridPosition gp in LevelManager.Instance.targets[i])
                    targetsData.Add(new int[] { i, gp.x, gp.y });
            return GetPathMultipleTargets(LevelManager.Instance.start, targetsData);
        }

        public Path GetPathUsingArray() {
            SetUpPathsArray();
            int c = 0;
            for (int i = 0; i < 6; i++)
                foreach (GridPosition gp in LevelManager.Instance.targets[i])
                    c++;
            int[][] targetsData = new int[c][];
            int p = 0;
            for (int i = 0; i < 6; i++)
                foreach (GridPosition gp in LevelManager.Instance.targets[i]) {
                    targetsData[p] = new int[] { i, gp.x, gp.y };
                    p++;
                }
            return GetPathMultipleTargetsUsingArray(LevelManager.Instance.start, targetsData);
        }
    }

    // Obstacles
    private class Tracker2 {

        private Dictionary<CubePosition, Path> currentPathDict;
        private Dictionary<CubePosition, Path>[] allPathsDict;

        private void SetupDictionary2(CubePosition start) {
            if (currentPathDict != null) {
                currentPathDict = allPathsDict[Instance.pointer];
                foreach (CubePosition cp in Instance.currentCubePositions) currentPathDict[cp] = new Path(null);
                currentPathDict[start] = Path.blank;
                return;
            }

            allPathsDict = new Dictionary<CubePosition, Path>[2] { new Dictionary<CubePosition, Path>(), new Dictionary<CubePosition, Path>() };

            for (int i = 0; i < 2; i++)
                foreach (CubePosition cp in Instance.allCubePositions[i])
                    allPathsDict[i].Add(cp, new Path(null));
            currentPathDict = allPathsDict[Instance.pointer];
            currentPathDict[start] = Path.blank;
        }

        private void SetupDictionary(CubePosition start) {
            int rows = LevelManager.Instance.Rows;
            int cols = LevelManager.Instance.Cols;

            List<CubePosition> currentCubePositions = new List<CubePosition>();

            List<Rotation> rotListLR = new List<Rotation>();
            List<Rotation> rotListFB = new List<Rotation>();
            List<Rotation> rotList0 = new List<Rotation>();
            List<Rotation> rotList1 = new List<Rotation>();
            for (int i = 0; i < 6; i++) {
                for (int j = 0; j < 6; j++) {
                    if ((i + 1) % 3 == j % 3)
                        rotListFB.Add(new Rotation(0, new Direction(i), new Direction(j)));
                    else if (i % 3 == (j + 1) % 3)
                        rotListLR.Add(new Rotation(0, new Direction(i), new Direction(j)));
                }
            }
            if (rotListLR.Contains(start.rotation)) { rotList0 = rotListLR; rotList1 = rotListFB; }
            else { rotList0 = rotListFB; rotList1 = rotListLR; }

            int m = (start.x + start.y) % 2;
            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < cols; c++) {
                    if ((r + c) % 2 == m) {
                        foreach (Rotation rot in rotList0)
                            currentCubePositions.Add(new CubePosition(c, r, rot));
                    }
                    else {
                        foreach (Rotation rot in rotList1)
                            currentCubePositions.Add(new CubePosition(c, r, rot));
                    }
                }
            }

            if (currentPathDict != null) {
                currentPathDict = allPathsDict[Instance.pointer];
                foreach (CubePosition cp in currentCubePositions) currentPathDict[cp] = new Path(null);
                currentPathDict[start] = Path.blank;
                return;
            }

            allPathsDict = new Dictionary<CubePosition, Path>[2] { new Dictionary<CubePosition, Path>(), new Dictionary<CubePosition, Path>() };

            for (int i = 0; i < 2; i++)
                foreach (CubePosition cp in Instance.allCubePositions[i])
                    allPathsDict[i].Add(cp, new Path(null));
            currentPathDict = allPathsDict[Instance.pointer];
            currentPathDict[start] = Path.blank;
        }

        private Path GetPathSingleTarget(CubePosition start, CubePosition target) {
            SetupDictionary(start);
            Queue<CubePosition> positionQ = new Queue<CubePosition>();
            positionQ.Enqueue(start);
            CubePosition current;
            List<CubePosition> nexts;
            if (!currentPathDict.ContainsKey(target))
                return new Path(null);
            bool outerloopBool = true;
            while (outerloopBool) {
                if (positionQ.Count == 0) {
                    Debug.Log("Wrong lr analysis!");
                    return new Path(null);
                }
                current = positionQ.Dequeue();
                nexts = new List<CubePosition>();

                if (current.y < LevelManager.Instance.Rows - 1 & !LevelManager.Instance.obstacles.Contains(new GridPosition(current.x, current.y + 1)))
                    nexts.Add(new CubePosition(current.x, current.y + 1, current.rotation.GetNext(Direction.forward)));
                if (current.y > 0 & !LevelManager.Instance.obstacles.Contains(new GridPosition(current.x, current.y - 1)))
                    nexts.Add(new CubePosition(current.x, current.y - 1, current.rotation.GetNext(Direction.back)));
                if (current.x > 0 & !LevelManager.Instance.obstacles.Contains(new GridPosition(current.x - 1, current.y)))
                    nexts.Add(new CubePosition(current.x - 1, current.y, current.rotation.GetNext(Direction.left)));
                if (current.x < LevelManager.Instance.Cols - 1 & !LevelManager.Instance.obstacles.Contains(new GridPosition(current.x + 1, current.y)))
                    nexts.Add(new CubePosition(current.x + 1, current.y, current.rotation.GetNext(Direction.right)));
                foreach (CubePosition next in nexts) {
                    if (currentPathDict[next] == new Path(null)) {
                        currentPathDict[next] = new Path(currentPathDict[current], new GridPosition(next.x, next.y));
                        if (target == next) { outerloopBool = false; break; }
                        positionQ.Enqueue(next);
                    }
                }
            }
            return currentPathDict[target];
        }

        private Path GetPathMultipleTargets(CubePosition start, List<int[]> targetData) {
            if (targetData.Count == 0) {
                return Path.blank;
            }
            Path[] paths = new Path[targetData.Count * 2];
            for (int i = 0; i < targetData.Count; i++) {
                List<int[]> nextTargetData = new List<int[]>();
                foreach (int[] target in targetData)
                    nextTargetData.Add(target);
                nextTargetData.RemoveAt(i);
                int[] nextStart = targetData[i];

                bool lr = true;
                if ((start.rotation.Vector[0].value + 1) % 3 == start.rotation.Vector[1].value % 3)
                    lr = !lr;
                if ((start.x + start.y) % 2 != (nextStart[1] + nextStart[2]) % 2)
                    lr = !lr;
                Path path0, path1;
                Rotation rot0, rot1;
                if (lr) {
                    rot0 = new Rotation(nextStart[0], Direction.down, Direction.left);
                    rot1 = new Rotation(nextStart[0], Direction.down, Direction.right);
                }
                else {
                    rot0 = new Rotation(nextStart[0], Direction.down, Direction.forward);
                    rot1 = new Rotation(nextStart[0], Direction.down, Direction.back);
                }

                path0 = GetPathSingleTarget(start, new CubePosition(nextStart[1], nextStart[2], rot0));
                path1 = GetPathSingleTarget(start, new CubePosition(nextStart[1], nextStart[2], rot1));

                paths[i * 2] = new Path(path0, GetPathMultipleTargets(new CubePosition(nextStart[1], nextStart[2], rot0), nextTargetData));
                paths[i * 2 + 1] = new Path(path1, GetPathMultipleTargets(new CubePosition(nextStart[1], nextStart[2], rot1), nextTargetData));

            }
            int lengthMin = int.MaxValue;
            int iMin = 0;
            for (int i = 0; i < paths.Length; i++) {
                if (paths[i].Length < lengthMin) {
                    iMin = i;
                    lengthMin = paths[i].Length;
                }
            }
            return paths[iMin];
        }

        public Path GetPath() {
            List<int[]> targetsData = new List<int[]>();
            for (int i = 0; i < 6; i++)
                foreach (GridPosition gp in LevelManager.Instance.targets[i])
                    targetsData.Add(new int[] { i, gp.x, gp.y });
            return GetPathMultipleTargets(LevelManager.Instance.start, targetsData);
        }
    }


}
