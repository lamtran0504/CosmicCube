using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class ButtonManager : Singleton<ButtonManager> {

    public int SceneIndex { get; private set; } // mainMenu is 0, chapterSelection is 1, stageSelection is 2, gameplay is 3, settings is 4, about is 5

    private int selectedChapter;
    public int SelectedChapter {
        get { return selectedChapter; }
        set {
            if (value < 1 || value > GameManager.MaxChapter) { Debug.Log("Inappropriate chapter selection call: Chapter " + value); return; } 
            selectedChapter = value;
        }
    }
    private int selectedStage;
    public int SelectedStage {
        get { return selectedStage; }
        set {
            if (value < 0 || value > GameManager.MaxStage) { Debug.Log("Inappropriate stage selection call: Stage " + value); return; }
            selectedStage = value;
        }
    }

    [SerializeField]
    private GameObject[] scenes;

    // Gameplay Panel
    [SerializeField]
    private GameObject upperLeftBtnPanel, mainMenuConfirmationPanel, inGameStageSelectionPanel, inGameChapterBtnGrid, settingsPanel, tutorialPanel;

    // Gameplay Buttons
    public GameObject[] inGameBtns; // 1 is hintCD
    [SerializeField]
    private GameObject hintBtn, inGameChapterBtns, inGameStageBtns, notSelectedReminder, inGameStarText, togglePopup;

    // Tutorial
    [SerializeField]
    private GameObject tutorial, tutorialLeftBtn, tutorialRightBtn, tutorialCountText, tutorialText, ayeBtn;

    // ChapterSelection
    [SerializeField]
    private GameObject chapterBtnGrid;
    [SerializeField]
    private GameObject[] progressText, lockObj;

    // StageSelection
    [SerializeField]
    private GameObject stageBtnGrid, chapterText, decreaseChapterBtn, increaseChapterBtn, starText;

    [SerializeField]
    private GameObject quitConfirmationPanel;  

    [SerializeField]
    private Sprite goldStarIcon, greyStarIcon, stageIcon, selectedStageIcon;

    private bool moveButtonsHidden, upperLeftPanelHiden;

    private int tutCount;

    private float upperLeftY;
    private byte r = 200;
    private byte a;
    private float selectionReminderCD;
    Color32 highlightedColor = new Color32(255, 200, 0, 255);
    Color32 white = new Color32(255, 255, 255, 255);
    Color32 grey50 = new Color32(50, 50, 50, 255);

    private void FixedUpdate() {
        if (SceneIndex != 3)
            return;
        if (selectionReminderCD > 0)
            selectionReminderCD -= Time.deltaTime;
        else if (a > 5) {
            a -= 5;
            notSelectedReminder.GetComponent<Text>().color = new Color32(r, 0, 0, a);
        }
        else notSelectedReminder.GetComponent<Text>().color = new Color32(r, 0, 0, 0);
    }

    // Scene Changing
    public void ChangeScene(int sceneIndex) {
        GameManager.Instance.stageClearPanel.SetActive(false);
        if (SceneIndex == 0 & sceneIndex != 4)
            AnimationManager.Instance.cubeObject.SetActive(false);
        if (SceneIndex == 3) {
            LevelManager.Instance.ExitLevel(0, true);
            mainMenuConfirmationPanel.SetActive(false);
            AudioManager.Instance.ChangeBackgroundMusic(0);
            Cube.Instance.ExitGameplay();
        }
        if (sceneIndex < 0) return;
        SceneIndex = sceneIndex;
        foreach (GameObject scene in scenes)
            scene.SetActive(false);
        scenes[sceneIndex].SetActive(true);
        if (sceneIndex == 0) {
            Camera.main.GetComponent<CameraScript>().ToMainMenu();
            AnimationManager.Instance.cubeObject.SetActive(true);
            AnimationManager.Instance.PlayMainMenuAnimation();
        }
        else if (sceneIndex == 1) {
            progressText[0].GetComponent<Text>().text = "Progress: " + Player.Instance.Percentage[0] + "%";
            int maxUnlockedChapter = Player.Instance.CurrentChapter;
            for (int i = 1; i < maxUnlockedChapter; i++) {
                lockObj[i - 1].SetActive(false);
                chapterBtnGrid.transform.GetChild(i).GetComponent<Button>().enabled = true;
                progressText[i].SetActive(true);
                progressText[i].GetComponent<Text>().text = "Progress: " + Player.Instance.Percentage[i] + "%";
            }
            for (int i = maxUnlockedChapter; i < GameManager.MaxChapter; i++)
                chapterBtnGrid.transform.GetChild(i).GetComponent<Button>().enabled = false;

            chapterBtnGrid.GetComponent<RectTransform>().localPosition = new Vector2(1344, 0);
        }
    }

    #region Quit

    public void Quit() {
        quitConfirmationPanel.SetActive(true);
    }

    public void ConfirmQuit() {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void CancelQuit() {
        quitConfirmationPanel.SetActive(false);
    }
    
    #endregion

    #region StageSelection

    public void SelectChapter(int selectedChapter) {
        SelectedChapter = selectedChapter;
        ChangeScene(2);
        chapterText.GetComponent<Text>().text = "Chapter " + SelectedChapter;
        if (selectedChapter == 1)
            decreaseChapterBtn.SetActive(false);
        else
            decreaseChapterBtn.SetActive(true);
        if (selectedChapter == Player.Instance.CurrentChapter)
            increaseChapterBtn.SetActive(false);
        else
            increaseChapterBtn.SetActive(true);
        starText.GetComponent<Text>().text = Player.Instance.ChapterStar[SelectedChapter - 1].ToString() + " / 60";

        int w = 10;
        for (int i = 0; i < Player.Instance.CurrentStage[selectedChapter - 1]; i++) {
            stageBtnGrid.transform.GetChild(i / 2).gameObject.SetActive(true);
            Transform btnTr = stageBtnGrid.transform.GetChild(i / 2).GetChild(i % 2);
            btnTr.GetComponent<StageButton>().SetStarNumber(Player.Instance.Star[SelectedChapter - 1, i], goldStarIcon, greyStarIcon);
            for (int j = 0; j < 4; j++)
                btnTr.GetChild(j).gameObject.SetActive(true);
            btnTr.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
            btnTr.GetComponent<Button>().enabled = true;
        }
        int k = Player.Instance.CurrentStage[selectedChapter - 1] - 1;
        w = k / 2 + 1;
        for (int j = k / 2 + 1; j < 10; j++)
            stageBtnGrid.transform.GetChild(j).gameObject.SetActive(false);
        if (k % 2 == 0) {
            Transform btnTransform = stageBtnGrid.transform.GetChild(k / 2).GetChild(1);
            for (int j = 0; j < 4; j++)
                btnTransform.GetChild(j).gameObject.SetActive(false);
            btnTransform.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
            btnTransform.GetComponent<Button>().enabled = false;
        }
        stageBtnGrid.GetComponent<RectTransform>().localPosition = new Vector2(85 * w, -30);
        AudioManager.Instance.PlayButtonClickedSound();
    }

    public void IncreaseChapter() {
        if (SelectedChapter == GameManager.MaxChapter) return;
        SelectedChapter++;
        chapterText.GetComponent<Text>().text = "Chapter " + SelectedChapter;
        SelectChapter(SelectedChapter);
    }

    public void DecreaseChapter() {
        if (SelectedChapter == 0) return;
        SelectedChapter--;
        chapterText.GetComponent<Text>().text = "Chapter " + SelectedChapter;
        SelectChapter(SelectedChapter);
    }

    public void SelectStage(int selectedStage) {
        GetChanceToShowAd();
        SelectedStage = selectedStage;
        GameManager.Instance.Chapter = SelectedChapter;
        GameManager.Instance.Stage = SelectedStage;
        LevelManager.Instance.EnterLevel(SelectedChapter, SelectedStage);
        ChangeScene(3);
        
        hintPanel.SetActive(false);
        AudioManager.Instance.ChangeBackgroundMusic(1);
        Cube.Instance.cannotMove = false;
        GameManager.Instance.IsInEndGame = false;

        Cube.Instance.EnterGameplay();
        Camera.main.GetComponent<CameraScript>().EnterGameplay();
        AudioManager.Instance.PlayButtonClickedSound();

        if (SelectedChapter == 1 & SelectedChapter == 1) {
            if (Player.Instance.ChapterStar[0] == 0)
                OpenTutorialPanel();
        }
    }

    #endregion

    // back = 1, left = 2, forward = 4, right = 5
    public void MoveCube(int d) {
        if (Cube.Instance.isFalling || Cube.Instance.cannotMove)
            return;
        int cubeX, cubeY;
        cubeX = Cube.Instance.position.x;
        cubeY = Cube.Instance.position.y;

        if (d == 1) {
            if (cubeY == 0) return;
            if (LevelManager.Instance.obstacles.Contains(new GridPosition(cubeX, cubeY - 1))) return;
            Cube.Instance.Move(Direction.back);
        }
        else if (d == 4) {
            if (cubeY == LevelManager.Instance.Rows - 1) return;
            if (LevelManager.Instance.obstacles.Contains(new GridPosition(cubeX, cubeY + 1))) return;
            Cube.Instance.Move(Direction.forward);
        }
        else if (d == 2) {
            if (cubeX == 0) return;
            if (LevelManager.Instance.obstacles.Contains(new GridPosition(cubeX - 1, cubeY))) return;
            Cube.Instance.Move(Direction.left);
        }
        else if (d == 5) {
            if (cubeX == LevelManager.Instance.Cols - 1) return;
            if (LevelManager.Instance.obstacles.Contains(new GridPosition(cubeX + 1, cubeY))) return;
            Cube.Instance.Move(Direction.right);
        }
        else return;
    }

    public void ResetStage() {
        LevelManager.Instance.EnterLevel(GameManager.Instance.Chapter, GameManager.Instance.Stage, true);
        GameManager.Instance.stageClearPanel.SetActive(false);
    }

    #region IngameStageSelection

    public void EnterIngameStageSelection() {
        inGameStageSelectionPanel.SetActive(true);
        GameManager.Instance.lastStageClearPanel.SetActive(false);
        //ToggleIngameButtons(false);
        Cube.Instance.cannotMove = true;

        inGameChapterBtns.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Progress: " + Player.Instance.Percentage[0] + "%";

        for (int i = 1; i < Player.Instance.CurrentChapter; i++) {
            Transform chapterBtnTr = inGameChapterBtns.transform.GetChild(i);
            chapterBtnTr.GetChild(1).gameObject.SetActive(true);
            chapterBtnTr.GetChild(1).GetComponent<Text>().text = "Progress: " + Player.Instance.Percentage[i] + "%";
            chapterBtnTr.GetChild(2).gameObject.SetActive(false);
            chapterBtnTr.GetComponent<Button>().enabled = true;
        }

        for (int i = Player.Instance.CurrentChapter; i < GameManager.MaxChapter; i++) {
            Transform chapterBtnTr = inGameChapterBtns.transform.GetChild(i);
            chapterBtnTr.GetChild(1).gameObject.SetActive(false);
            chapterBtnTr.GetChild(2).gameObject.SetActive(true);
            chapterBtnTr.GetComponent<Button>().enabled = false;
        }

        int c = GameManager.Instance.Chapter;
        if (c < 3)
            inGameChapterBtnGrid.transform.localPosition = new Vector2(0, -249);
        else if (c > 8)
            inGameChapterBtnGrid.transform.localPosition = new Vector2(0, 249);
        else
            inGameChapterBtnGrid.transform.localPosition = new Vector2(0, -215 + 87 * (c - 3));
        SelectChapterInGame(SelectedChapter);    
    }

    public void SelectChapterInGame(int selectedChapter) {
        for (int i = 0; i < GameManager.MaxChapter; i++)
            inGameChapterBtns.transform.GetChild(i).GetComponent<Image>().color = white;
        inGameChapterBtns.transform.GetChild(selectedChapter - 1).GetComponent<Image>().color = highlightedColor;

        SelectedChapter = selectedChapter;
        inGameStarText.GetComponent<Text>().text = Player.Instance.ChapterStar[SelectedChapter - 1].ToString() + " / 60";
        int maxUnlockedStage = Player.Instance.CurrentStage[SelectedChapter - 1];
        for (int i = 0; i < maxUnlockedStage; i++) {
            inGameStageBtns.transform.GetChild(i).gameObject.SetActive(true);
            inGameStageBtns.transform.GetChild(i).GetComponent<Image>().sprite = stageIcon;
            SelectedStage = 0;
            Transform starTr = inGameStageBtns.transform.GetChild(i).transform.GetChild(1);
            int stars = Player.Instance.Star[selectedChapter - 1, i];
            for (int j = 0; j < stars; j++)
                starTr.GetChild(j).GetComponent<Image>().sprite = goldStarIcon;
            for (int j = stars; j < 3; j++)
                starTr.GetChild(j).GetComponent<Image>().sprite = greyStarIcon;
        }
        for (int i = maxUnlockedStage; i < GameManager.MaxStage; i++)
            inGameStageBtns.transform.GetChild(i).gameObject.SetActive(false);
        AudioManager.Instance.PlayButtonClickedSound();
    }

    public void SelectStageInGame(int selectedStage) {
        if (SelectedStage > 0 & SelectedStage <= GameManager.MaxStage)
            inGameStageBtns.transform.GetChild(SelectedStage - 1).GetComponent<Image>().sprite = stageIcon;
        else
            for (int i = 0; i < GameManager.MaxStage; i++)
                inGameStageBtns.transform.GetChild(i).GetComponent<Image>().sprite = stageIcon;
        inGameStageBtns.transform.GetChild(selectedStage - 1).GetComponent<Image>().sprite = selectedStageIcon;
        SelectedStage = selectedStage;
        AudioManager.Instance.PlayButtonClickedSound();
    }

    public void ConfirmStageSelection() {
        if (SelectedStage == 0) {
            notSelectedReminder.GetComponent<Text>().color = new Color32(r, 0, 0, 255);
            selectionReminderCD = 1;
            a = 255;
            return;
        }
        inGameStageSelectionPanel.SetActive(false);
        //ToggleIngameButtons(true);
        GameManager.Instance.Chapter = SelectedChapter;
        GameManager.Instance.Stage = SelectedStage;
        inGameStageBtns.transform.GetChild(SelectedStage - 1).GetComponent<Image>().sprite = stageIcon;
        LevelManager.Instance.EnterLevel(GameManager.Instance.Chapter, GameManager.Instance.Stage, true);
        Cube.Instance.cannotMove = false;
        GameManager.Instance.IsInEndGame = false;
    }

    public void CancelStageSelection() {
        inGameStageSelectionPanel.SetActive(false);
        //ToggleIngameButtons(true);
        inGameChapterBtns.transform.GetChild(SelectedChapter - 1).GetComponent<Image>().color = white;
        SelectedChapter = GameManager.Instance.Chapter;
        SelectedStage = GameManager.Instance.Stage;
        Cube.Instance.cannotMove = false;
    }

    #endregion



    public void ToggleIngameButtons(bool toggle) {
        for (int i = 0; i < inGameBtns.Length; i++)
            inGameBtns[i].SetActive(toggle);
    }

    public void ToggleUpperLeftPanel() {
        if (!upperLeftPanelHiden)
            StartCoroutine(IHideUpperLeftPanel());
        else
            StartCoroutine(IExpandUpperLeftPanel());
    }

    private IEnumerator IHideUpperLeftPanel() {
        togglePopup.GetComponent<Text>().text = "Expand";
        upperLeftPanelHiden = true;
        float speed = 100;
        float x = upperLeftBtnPanel.GetComponent<RectTransform>().localPosition.x;
        float y = upperLeftBtnPanel.GetComponent<RectTransform>().localPosition.y;
        float yHidden = y + 50;
        while (y < yHidden) {
            y += speed * Time.deltaTime;
            upperLeftBtnPanel.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
            yield return new WaitForEndOfFrame();
        }
        y = yHidden;
        upperLeftBtnPanel.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
        yield return null;
    }

    private IEnumerator IExpandUpperLeftPanel() {
        togglePopup.GetComponent<Text>().text = "Hide";
        upperLeftPanelHiden = false;
        float speed = 100;
        float x = upperLeftBtnPanel.GetComponent<RectTransform>().localPosition.x;
        float y = upperLeftBtnPanel.GetComponent<RectTransform>().localPosition.y;
        float yExpanded = y - 50;
        while (y > yExpanded) {
            y -= speed * Time.deltaTime;
            upperLeftBtnPanel.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
            yield return new WaitForEndOfFrame();
        }
        y = yExpanded;
        upperLeftBtnPanel.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
        yield return null;
    }

    #region Ads, Hint and Solution

    [SerializeField]
    private GameObject hintPanel, solutionPanel, solutionTitleText, movesPanel;
    [SerializeField]
    private Sprite upArrowSprite;

    public void GetChanceToShowAd() {
        if (Player.Instance.CurrentChapter < 3)
            return;
        if (Utilities.rand.Next(100) < 20)
            ShowAd();
    }

    private void ShowAd() {
        if (!Advertisement.IsReady()) {
            Debug.Log("Ads not ready for default placement");
            return;
        }
        Advertisement.Show();
    }

    public void ShowHintAd() {
        if (Advertisement.IsReady("rewardedVideo")) {
            var options = new ShowOptions { resultCallback = HandleShowHintResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowHintResult(ShowResult result) {
        switch (result) {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                ShowHint();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

    private void ShowHint() {
        hintPanel.SetActive(true);
        Cube.Instance.cannotMove = true;
        string textStr = string.Format("Clear the stage in <color=#FFC800>{0}</color> moves to earn <color=#FFC800>3 stars.</color>", 
                                        GameManager.Instance.AllShortestPathLengths[GameManager.Instance.Chapter - 1, GameManager.Instance.Stage - 1]);
        hintPanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = textStr;
    }

    public void CloseHint() {
        hintPanel.SetActive(false);
        Cube.Instance.cannotMove = false;
    }

    public void ShowSolutionAd() {
        hintPanel.SetActive(false);
        LevelManager.Instance.LoadSolution();
        if (GameManager.Instance.ShortestPath == null) return;
        if (Advertisement.IsReady("rewardedVideo")) {
            var options = new ShowOptions { resultCallback = HandleShowSolutionResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowSolutionResult(ShowResult result) {
        switch (result) {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                ShowSolution();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

    private void ShowSolution() {
        solutionPanel.SetActive(true);
        string textStr = string.Format("Solution for Stage <color=#FFC800>{0} - {1}</color>",
                                        GameManager.Instance.Chapter, GameManager.Instance.Stage);
        solutionTitleText.GetComponent<Text>().text = textStr;

        if (GameManager.Instance.ShortestPath == null) {
            Debug.Log("Solution is null: Stage " + GameManager.Instance.Chapter + " - " + GameManager.Instance.Stage);
            return;
        }
        for (int i = 0; i < GameManager.Instance.ShortestPath.Length; i++) {
            movesPanel.transform.GetChild(i).gameObject.SetActive(true);
            movesPanel.transform.GetChild(i).GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, GameManager.Instance.ShortestPath[i] * 90);
        }
        for (int i = GameManager.Instance.ShortestPath.Length; i < 52; i++) {
            movesPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void CloseSolution() {
        solutionPanel.SetActive(false);
        Cube.Instance.cannotMove = false;
    }

    #endregion

    #region tutorial

    public void OpenTutorialPanel() {
        tutCount = 0;
        tutorialLeftBtn.SetActive(false);
        tutorialRightBtn.SetActive(true);
        tutorialText.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 1; i < 4; i++)
            tutorialText.transform.GetChild(i).gameObject.SetActive(false);
        tutorialCountText.GetComponent<Text>().text = "1 / 4";
        ayeBtn.SetActive(false);
        tutorialPanel.SetActive(true);
        Cube.Instance.cannotMove = true;
    }

    public void TutorialBack() {
        tutCount--;
        tutorialRightBtn.SetActive(true);
        if (tutCount == 0)
            tutorialLeftBtn.SetActive(false);
        for (int i = 0; i < 4; i++)
            tutorialText.transform.GetChild(i).gameObject.SetActive(false);
        tutorialText.transform.GetChild(tutCount).gameObject.SetActive(true);
        tutorialCountText.GetComponent<Text>().text = (tutCount + 1).ToString() + " / " + 4;
    }

    public void TutorialNext() {
        tutCount++;
        tutorialLeftBtn.SetActive(true);
        if (tutCount == 3) {
            tutorialRightBtn.SetActive(false);
            ayeBtn.SetActive(true);
        }
        for (int i = 0; i < 4; i++)
            tutorialText.transform.GetChild(i).gameObject.SetActive(false);
        tutorialText.transform.GetChild(tutCount).gameObject.SetActive(true);
        tutorialCountText.GetComponent<Text>().text = (tutCount + 1).ToString() + " / " + 4;
    }

    public void TutorialClose() {
        tutorialPanel.SetActive(false);
        Cube.Instance.cannotMove = false;
    }

    #endregion

    // In-game Main Menu Button
    public void OpenMainMenuConfirmationPanel() {
        mainMenuConfirmationPanel.SetActive(true);
        Cube.Instance.cannotMove = true;
    }

    public void CancelMainMenu() {
        mainMenuConfirmationPanel.SetActive(false);
        Cube.Instance.cannotMove = false;
    }

    // In-game Settings

    public void OpenSettingsPanel() {
        settingsPanel.SetActive(true);
        Cube.Instance.cannotMove = true;
    }

    public void CloseSettingsPanel() {
        settingsPanel.SetActive(false);
        Cube.Instance.cannotMove = false;
    }


    public void UpdatePathForAllStages() {
        string writerDir = "Assets/Data/Solution/AllPaths.txt";
        StreamWriter writer = new StreamWriter(writerDir, true);
        int[][] paths = new int[5][];
        for (int i = 0; i < 5; i++)
            paths[i] = new int[20];
        for (int c = 0; c < 5; c++) {
            string readerDir = "Assets/Data/Solution/" + (c + 1) + ".txt";
            StreamReader reader = new StreamReader(readerDir);
            for (int s = 0; s < 20; s++)
            {
                paths[c][s] = reader.ReadLine().Split(' ').Length;
            }
            reader.Close();
        }
        for (int c = 0; c < 5; c++)
        {
            writer.Write(paths[c][0]);
            for (int s = 1; s < 20; s++)
            {
                writer.Write(" " + paths[c][s]);
            }
            writer.Write('\n');
        }
        writer.Flush();
        writer.Close();
    }

}
