using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData {

    public int[] start;
    public int[] targets;
    public int[] obstacles;

    public static StageData firstStage = new StageData(new int[] { 2, 1 }, new int[] { 0, 2, 5 }, new int[] { });

    public StageData(int[] start, int[] targets, int[] obstacles) {
        if (start.Length != 2 || targets.Length % 3 != 0 || obstacles.Length % 2 != 0) {
            this.start = new int[] { 2, 1 };
            this.targets = new int[] { 0, 2, 5 };
            this.obstacles = new int[] { };
            Debug.Log("Stage data file corrupted");
            return;
        }
        this.start = start;
        this.targets = targets;
        this.obstacles = obstacles;
    }
	
}
