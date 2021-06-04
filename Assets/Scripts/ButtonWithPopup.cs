using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonWithPopup : MonoBehaviour {

    [SerializeField]
    GameObject popupText;

    public void ShowPopupText() {
        popupText.SetActive(true);
    }

    public void HidePopupText() {
        popupText.SetActive(false);
    }

}
