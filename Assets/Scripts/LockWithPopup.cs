using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockWithPopup : MonoBehaviour {

    [SerializeField]
    GameObject popupText;

    public void ShowPopupText() {
        popupText.transform.localPosition = new Vector2(transform.localPosition.x + 140, transform.localPosition.y);
        popupText.SetActive(true);
    }

    public void HidePopupText() {
        popupText.SetActive(false);
    }

}
