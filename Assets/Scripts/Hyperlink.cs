using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyperlink : MonoBehaviour {

    [SerializeField]
    private string URL;

    [SerializeField]
    private GameObject FollowURLConfirmationPanel;

    private static string currentURL;

    public void OpenLink() {
        FollowURLConfirmationPanel.SetActive(false);
        Application.OpenURL(currentURL);
    }

    public void OpenConfirmationPanel() {
        FollowURLConfirmationPanel.SetActive(true);
        currentURL = URL;
    }

    public void CloseConfirmationPanel() {
        FollowURLConfirmationPanel.SetActive(false);
    }

}
