using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : Singleton<AnimationManager> {


    // MainMenu
    [SerializeField]
    private GameObject playBtn, settingsBtn, aboutBtn, quitBtn;
    [SerializeField]
    private GameObject[] lights;
    public GameObject cubeObject;

    public GameObject tilePrefab, dustPrefab;
    [SerializeField]
    private GameObject allstagesclearedPrefab;
    public GameObject AllStagesClearedObj { get; set; }

    [SerializeField]
    private GameObject chapterUnlockPanel;
    public GameObject skipAnimBtn;

    [SerializeField]
    private GameObject logo, directionalLight;

    public bool SkippingEndgame { get; set; }
    private IEnumerator EndgameCor;

    private void Start() { 
        
    }

    public void Startup()
    {
        //PlayStartAnimation();
        PlayMainMenuAnimation();
    }

    // MainMenu
    public void PlayStartAnimation() {
        StartCoroutine("IPlayStartAnim");
    }

    public IEnumerator IPlayStartAnim() {

        float a = 0;
        logo.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)a);
        yield return new WaitForSeconds(1);
        while (a < 255) {
            logo.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)a);
            a += 1f;          
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(2);

        Destroy(logo.gameObject);

        AudioManager.Instance.ToggleBackgroundMusic(AudioManager.Instance.BackGroundMusicOn);

        cubeObject.SetActive(true);
        Camera.main.GetComponent<CameraScript>().ToMainMenu();
        
        directionalLight.SetActive(true);
        PlayMainMenuAnimation();

        yield return null;
    }

    public void PlayMainMenuAnimation() {

        Destroy(logo.gameObject);

        AudioManager.Instance.ToggleBackgroundMusic(AudioManager.Instance.BackGroundMusicOn);

        cubeObject.SetActive(true);
        Camera.main.GetComponent<CameraScript>().ToMainMenu();

        directionalLight.SetActive(true);

        StartCoroutine("IPlayBtnAnim");
        StartCoroutine("IPlayCubeAnim");
    }

    private IEnumerator IPlayBtnAnim() {
        int playX = -110, settingsX = -30, aboutX = 30, quitX = 110;
        int speed = 30;
        float x0 = playX - 940, x1 = settingsX - 810, x2 = aboutX - 680, x3 = quitX - 550, y0, y1, y2, y3, z;
        y0 = playBtn.transform.localPosition.y;
        y1 = settingsBtn.transform.localPosition.y;
        y2 = aboutBtn.transform.localPosition.y;
        y3 = quitBtn.transform.localPosition.y;
        z = playBtn.transform.localPosition.z;
        playBtn.GetComponent<Button>().enabled = false;
        settingsBtn.GetComponent<Button>().enabled = false;
        aboutBtn.GetComponent<Button>().enabled = false;
        quitBtn.GetComponent<Button>().enabled = false;

        while (x0 < playX || x1 < settingsX || x2 < aboutX || x3 < quitX) {
            x0 += speed;
            x1 += speed;
            x2 += speed;
            x3 += speed;
            playBtn.transform.localPosition = (x0 < playX) ? new Vector3(x0, y0, z) : new Vector3(playX, y0, z);
            settingsBtn.transform.localPosition = (x1 < settingsX) ? new Vector3(x1, y1, z) : new Vector3(settingsX, y1, z);
            aboutBtn.transform.localPosition = (x2 < aboutX) ? new Vector3(x2, y2, z) : new Vector3(aboutX, y2, z);
            quitBtn.transform.localPosition = (x3 < quitX) ? new Vector3(x3, y3, z) : new Vector3(quitX, y3, z);
            yield return new WaitForFixedUpdate();
        }

        playBtn.GetComponent<Button>().enabled = true;
        settingsBtn.GetComponent<Button>().enabled = true;
        aboutBtn.GetComponent<Button>().enabled = true;
        quitBtn.GetComponent<Button>().enabled = true;

        yield return null;
    }

    private IEnumerator IPlayCubeAnim() {
        float n = 0;
        float angle;
        Transform transform = cubeObject.transform;

        while (ButtonManager.Instance.SceneIndex == 0 || ButtonManager.Instance.SceneIndex == 4) {
            n += 5;
            angle = (n / 10) % 360;
            transform.rotation = Quaternion.Euler(0, angle, 0);
            yield return new WaitForFixedUpdate();
        }
    }


    // Gameplay
    public void PlayStageClearAnim(int stars, Transform starsTransform) {
        StartCoroutine(IPlayStageClearAnim(stars, starsTransform));
    }

    private IEnumerator IPlayStageClearAnim(int stars, Transform starsTransform) {
        for (int i = 0; i < 3; i++)
            starsTransform.GetChild(i).localScale = Vector2.zero;
        for (int i = 0; i < stars; i++) {
            float scale = 0;
            for (int j = 0; j < 20; j++) {
                scale += 5;
                starsTransform.GetChild(i).localScale = new Vector3(scale / 100, scale / 100);
                yield return new WaitForFixedUpdate();     
            }
        }
        yield return null;
    }

    public void PlayChapterUnlockAnim(int chapter) {
        StartCoroutine(IPlayChapterUnlockAnim(chapter));
    }

    private IEnumerator IPlayChapterUnlockAnim(int chapter) {
        chapterUnlockPanel.SetActive(true);
        chapterUnlockPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        chapterUnlockPanel.transform.GetChild(0).GetComponent<Image>().color = new Color32(70, 70, 70, 255);
        chapterUnlockPanel.transform.GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, 255);
        chapterUnlockPanel.transform.GetChild(1).GetComponent<Text>().text = "Chapter " + chapter.ToString() + " Unlocked!";
        float cFl;
        byte c;
        float speed = 250;
        for (int i = 0; i < 3; i ++) {
            cFl = 255;
            while (cFl > 70) {          
                c = (byte)cFl;
                chapterUnlockPanel.GetComponent<Image>().color = new Color32(c, c, c, 255);
                cFl -= Time.deltaTime * speed;
                yield return new WaitForEndOfFrame();
            }
            while (cFl < 255) {    
                c = (byte)cFl;
                chapterUnlockPanel.GetComponent<Image>().color = new Color32(c, c, c, 255);
                cFl += Time.deltaTime * speed;
                yield return new WaitForEndOfFrame();
            }
            //chapterUnlockPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        yield return new WaitForSeconds(2);
        float aFl = 255;
        byte a;
        while (aFl > 0) {        
            a = (byte)aFl;
            chapterUnlockPanel.GetComponent<Image>().color = new Color32(255, 255, 255, a);
            chapterUnlockPanel.transform.GetChild(0).GetComponent<Image>().color = new Color32(70, 70, 70, a);
            chapterUnlockPanel.transform.GetChild(1).GetComponent<Text>().color = new Color32(0, 0, 0, a);
            aFl -= Time.deltaTime * speed / 2;
            yield return new WaitForEndOfFrame();
        }

        chapterUnlockPanel.SetActive(false);
        yield return null;
    } 
    
    // Endgame
    public void PlayEndGameAnimation() {
        skipAnimBtn.SetActive(true);
        StartCoroutine("ISnap");
        StartCoroutine("IPlayEndGameAnimation");
    }

    public void Snap() {
        StartCoroutine("ISnap");
    }

    private IEnumerator ISnap() {
        CubePosition cp = Cube.Instance.GetComponent<Cube>().position;
        GridPosition cubeGP = new GridPosition(cp.x, cp.y);
        int remaining = 22;
        int total = 44;
        System.Random rand = new System.Random();
        for (int x = 0; x < 5; x++) {
            for (int y = 0; y < 9; y++) {
                GridPosition gp = new GridPosition(x, y);
                if (gp == cubeGP)
                    continue;
                if (rand.Next(total) < remaining) {
                    LevelManager.Instance.tilesDict[gp].GetComponent<Tile>().Disintegrate(rand.Next(100) * 0.03f);
                    remaining--;
                }
                total--;
            }
        }
        LevelManager.Instance.tilesDict[cubeGP].GetComponent<Tile>().Disintegrate(3);
        yield return new WaitForSeconds(5);
        if (LevelManager.Instance.tilesDict[cubeGP] != null & !SkippingEndgame)
            LevelManager.Instance.tilesDict[cubeGP].GetComponent<BoxCollider>().enabled = false;
    }

    private IEnumerator IPlayEndGameAnimation() {
        Cube.Instance.cannotMove = true;
        GameManager.Instance.IsInEndGame = true;
        SkippingEndgame = false;
        for (int i = 0; i < 2; i++)
            ButtonManager.Instance.inGameBtns[i].SetActive(false);

        AllStagesClearedObj = Instantiate(allstagesclearedPrefab, Cube.Instance.gameObject.transform.position - new Vector3(23.1f, 66, 1.1f), Quaternion.identity);
        Vector3 cameraFinalPos = Cube.Instance.gameObject.transform.position - new Vector3(.5f, 8, 2.3f);
        yield return new WaitForSeconds(5);

        Camera.main.GetComponent<CameraScript>().EndGame(cameraFinalPos);
        yield return new WaitForSeconds(3);

        if (!SkippingEndgame) {
            int count = LevelManager.Instance.tilesTransform.transform.childCount;
            for (int i = 0; i < count; i++)
                Destroy(LevelManager.Instance.tilesTransform.transform.GetChild(i).gameObject);
            yield return new WaitForSeconds(2);
        }

        while (!SkippingEndgame & GameManager.Instance.IsInEndGame)
            yield return new WaitForEndOfFrame();
        skipAnimBtn.SetActive(false);

        if (!SkippingEndgame)
            GameManager.Instance.StartCoroutine("ICompleteLastStage");
        yield return null;
    }

    public void SkipEndgameAnimation() {
        GameManager.Instance.StartCoroutine("ICompleteLastStage");
        skipAnimBtn.SetActive(false);
        SkippingEndgame = true;
        if (AllStagesClearedObj == null) {
            AllStagesClearedObj = Instantiate(allstagesclearedPrefab, Cube.Instance.gameObject.transform.position - new Vector3(23.1f, 66, 1.1f), Quaternion.identity);
        }

        Cube.Instance.transform.position = AllStagesClearedObj.transform.position + new Vector3(23.1f, 0.6f, 1.1f);
        Camera.main.transform.position = AllStagesClearedObj.transform.position + new Vector3(22.6f, 58, -1.2f);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
        int count = LevelManager.Instance.tilesTransform.transform.childCount;
        for (int i = 0; i < count; i++)
            Destroy(LevelManager.Instance.tilesTransform.transform.GetChild(i).gameObject);
    }
}
