using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager> {

    private bool ignoreValueChange;
    
    private GameObject backgroundMusic;
    [SerializeField]
    private GameObject mainMenuMusic, gameplayMusic, cubeSound, stageClearSound, buttonClickedSound;

    [SerializeField]
    private GameObject bgMusicObj0, bgMusicObj1, ingameSoundObj0, ingameSoundObj1, uiSoundObj0, uiSoundObj1, stageClearSoundObj0, stageClearSoundObj1;

    public bool BackGroundMusicOn { get; private set; }
    public bool IngameSoundOn { get; private set; }
    public bool UISoundOn{ get; private set; }
    public bool StageClearSoundOn { get; private set; }

    private Color onColor = new Color32(255, 200, 0, 255);
    private Color offColor = new Color32(70, 70, 70, 255);

    private void Start() {
    }

    public void Startup() {
        backgroundMusic = mainMenuMusic;
    }

    public void LoadVolumeSettings(int toggle0, int volume0, int toggle1, int volume1, int toggle2, int volume2, int toggle3, int volume3) {
        BackGroundMusicOn = toggle0 != 0;
        ToggleUISound(toggle1 != 0);
        ToggleIngameSound(toggle2 != 0);
        ToggleStageClearSound(toggle3 != 0);
        ChangeVolumeBackgroundMusic(((float)volume0) / 100);
        ChangeVolumeUISound(((float)volume1) / 100);
        ChangeVolumeIngameSound(((float)volume2) / 100);
        ChangeVolumeStageClearSound(((float)volume3) / 100);
    }

    public void ChangeBackgroundMusic(int songIndex) {
        backgroundMusic.GetComponent<AudioSource>().Stop();
        backgroundMusic = (songIndex == 0) ? mainMenuMusic : gameplayMusic;
        if (BackGroundMusicOn)
            backgroundMusic.GetComponent<AudioSource>().Play();
    }

    #region toggle audio

    public void ToggleBackgroundMusic(bool toggle) {
        ToggleAudioElement(0, toggle);
    }

    public void ToggleUISound(bool toggle) {
        ToggleAudioElement(1, toggle);
    }

    public void ToggleIngameSound(bool toggle) {
        ToggleAudioElement(2, toggle);
    }

    public void ToggleStageClearSound(bool toggle) {
        ToggleAudioElement(3, toggle);
    }

    private void ToggleAudioElement(int audioIndex, bool toggle) {
        if (ignoreValueChange)
            return;

        GameObject sliderUIObj0 = null, sliderUIObj1 = null;
        
        switch (audioIndex) {
            case 0:
                BackGroundMusicOn = toggle;
                if (toggle)
                    backgroundMusic.GetComponent<AudioSource>().Play();
                else
                    backgroundMusic.GetComponent<AudioSource>().Stop();
                sliderUIObj0 = bgMusicObj0;
                sliderUIObj1 = bgMusicObj1;
                break;
            case 1:
                UISoundOn = toggle;
                sliderUIObj0 = uiSoundObj0;
                sliderUIObj1 = uiSoundObj1;
                break;
            case 2:
                IngameSoundOn = toggle;
                sliderUIObj0 = ingameSoundObj0;
                sliderUIObj1 = ingameSoundObj1;
                break;     
            case 3:
                StageClearSoundOn = toggle;
                sliderUIObj0 = stageClearSoundObj0;
                sliderUIObj1 = stageClearSoundObj1;
                break;
            default:
                Debug.Log("Inappropriate audio name");
                return;
        }

        ignoreValueChange = true;

        sliderUIObj0.transform.GetChild(0).GetComponent<Toggle>().isOn = toggle;
        sliderUIObj1.transform.GetChild(0).GetComponent<Toggle>().isOn = toggle;
        ignoreValueChange = false;

        Color knobColor = toggle ? onColor : offColor;

        sliderUIObj0.transform.GetChild(1).GetComponent<Text>().color = knobColor;
        sliderUIObj0.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().color = knobColor;
        sliderUIObj1.transform.GetChild(1).GetComponent<Text>().color = knobColor;
        sliderUIObj1.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().color = knobColor;

        Player.Instance.UpdateSettingsData(1 + audioIndex * 2, toggle ? 1 : 0);
    }

    #endregion

    #region play audio

    public void PlayCubeSound() {
        if (IngameSoundOn)
            cubeSound.GetComponent<AudioSource>().Play();
    }

    public void PlayStageClearSound() {
        if (StageClearSoundOn)
            stageClearSound.GetComponent<AudioSource>().Play();
    }

    public void PlayButtonClickedSound() {
        if (UISoundOn)
            buttonClickedSound.GetComponent<AudioSource>().Play();
    }

    #endregion

    #region volume change

    public void ChangeVolumeBackgroundMusic(float v) {
        ChangeAudioVolume(0, v);
    }

    public void ChangeVolumeUISound(float v) {
        ChangeAudioVolume(1, v);
    }

    public void ChangeVolumeIngameSound(float v) {
        ChangeAudioVolume(2, v);
    }

    public void ChangeVolumeStageClearSound(float v) {
        ChangeAudioVolume(3, v);
    }

    private void ChangeAudioVolume(int audioIndex, float v) {
        if (ignoreValueChange)
            return;
        GameObject sliderUIObj0 = null, sliderUIObj1 = null;

        switch (audioIndex) {
            case 0:
                mainMenuMusic.GetComponent<AudioSource>().volume = v;
                gameplayMusic.GetComponent<AudioSource>().volume = v;
                sliderUIObj0 = bgMusicObj0;
                sliderUIObj1 = bgMusicObj1;
                break;
            case 1:
                buttonClickedSound.GetComponent<AudioSource>().volume = v;
                sliderUIObj0 = uiSoundObj0;
                sliderUIObj1 = uiSoundObj1;
                break;
            case 2:
                cubeSound.GetComponent<AudioSource>().volume = v;
                sliderUIObj0 = ingameSoundObj0;
                sliderUIObj1 = ingameSoundObj1;
                break;
            case 3:
                stageClearSound.GetComponent<AudioSource>().volume = v;
                sliderUIObj0 = stageClearSoundObj0;
                sliderUIObj1 = stageClearSoundObj1;
                break;
            default:
                Debug.Log("Inappropriate audio name");
                return;
        }

        ignoreValueChange = true;
        sliderUIObj0.transform.GetChild(2).GetComponent<Slider>().value = v;
        sliderUIObj1.transform.GetChild(2).GetComponent<Slider>().value = v;
        ignoreValueChange = false;
        Player.Instance.UpdateSettingsData(2 + audioIndex * 2, (int)(v * 100));
    }

    #endregion

}
