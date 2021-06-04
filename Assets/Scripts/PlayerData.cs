using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {

    public int[,] star;
    public int[] settings; // length is 5

    public static PlayerData blank = new PlayerData(new int[GameManager.MaxChapter, GameManager.MaxStage], new int[] { 1, 1, 30, 1, 30, 1, 30, 1, 30 });

    public PlayerData(Player player) {
        star = player.Star;
        settings = player.Settings;
    }

    private PlayerData(int[,] star, int[] settings) {
        this.star = star;
        this.settings = settings;
    }

}
