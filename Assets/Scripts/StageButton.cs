using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour {

    private Vector2 smallSize = new Vector2(170, 170);
    private Vector2 largeSize = new Vector2(200, 200);

    [SerializeField]
    private GameObject[] stars;

    private Rect hitBox;

    public void OnPointerEnter() {
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = largeSize;
    }

    public void OnPointerExit() {
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = smallSize;
    }

    public void OnPointerClick() {
        AudioManager.Instance.PlayButtonClickedSound();
    }

    public void SetStarNumber(int n, Sprite gold, Sprite grey) {
        for (int i = 0; i < n; i++)
            stars[i].GetComponent<Image>().sprite = gold;
        for (int i = n; i < 3; i++)
            stars[i].GetComponent<Image>().sprite = grey;
    }

}
