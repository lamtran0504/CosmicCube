using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Player : Singleton<Player> {

    public int[,] Star { get; private set; }

    public int[] ChapterStar { get; private set; }

    public int[] Percentage { get; private set; }

    public int[] CurrentStage { get; private set; }

    public int CurrentChapter { get; private set; }

    public int[] Settings { get; private set; }

    public bool IsFirstTime { get; private set; }

    public void Startup() {
        LoadData();
        try {
            AudioManager.Instance.LoadVolumeSettings(Settings[1], Settings[2], Settings[3], Settings[4], Settings[5], Settings[6], Settings[7], Settings[8]);
        }
        catch (System.IndexOutOfRangeException) { Debug.Log(Settings.Length); };
    }

    private void LoadData() {
        PlayerData data = SaveSystem.LoadPlayer();
        int mc = GameManager.MaxChapter;
        int ms = GameManager.MaxStage;
        Star = data.star;
        Settings = data.settings;
        CurrentChapter = GameManager.MaxChapter;

        ChapterStar = new int[mc];
        Percentage = new int[mc];
        CurrentStage = new int[mc];
        for (int i = 0; i < mc; i++) {
            ChapterStar[i] = GameManager.MaxStage * 3;
            for (int j = 0; j < ms; j++) {
                Star[i, j] = 3;
            }
            Percentage[i] = 100;
            CurrentStage[i] = GameManager.MaxStage;
        }
    }

    private void LoadData3() {
        PlayerData data = SaveSystem.LoadPlayer();
        int mc = GameManager.MaxChapter;
        int ms = GameManager.MaxStage;
        Star = data.star;
        Settings = data.settings;

        ChapterStar = new int[mc];
        Percentage = new int[mc];
        CurrentStage = new int[mc];
        for (int i = 0; i < mc; i++) {
            bool hasMoreStage = true;
            for (int j = 0; j < ms & hasMoreStage; j++) {
                ChapterStar[i] += Star[i, j];
                if (Star[i, j] == 0) {
                    CurrentStage[i] = j + 1;
                    hasMoreStage = false;
                }
            }
            Percentage[i] = (ChapterStar[i] * 5 + 1) / 3;
            CurrentChapter = i + 1;
            if (Percentage[i] < 70)
                break;

            if (CurrentStage[i] == 0)
                CurrentStage[i] = GameManager.MaxStage;
        }
    }

    public void UpdateProgress(int chapter, int stage, int star) {

        ChapterStar[chapter - 1] += star - Star[chapter - 1, stage - 1];
        if (Star[chapter - 1, stage - 1] == 0 & star > 0 & stage < GameManager.MaxStage)
            CurrentStage[chapter - 1]++;
        Star[chapter - 1, stage - 1] = star;
        bool nextChapterNotUnlocked = false;
        if (Percentage[chapter - 1] < 70)
            nextChapterNotUnlocked = true;
        Percentage[chapter - 1] = (ChapterStar[chapter - 1] * 5 + 1) / 3;
        if (Percentage[chapter - 1] >= 70 & nextChapterNotUnlocked) {
            CurrentChapter++;
            CurrentStage[CurrentChapter - 1] = 1;
        }

        SaveSystem.SavePlayer(this);

    }

    

    public void UpdateSettingsData(int setting, int value) {
        if (Settings.Length < setting) return;
        Settings[setting] = value;
        SaveSystem.SavePlayer(this);
    }

}
