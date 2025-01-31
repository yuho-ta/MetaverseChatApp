using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePanelController : MonoBehaviour
{
    public Button DanceButton;
    public Button FriendButton;
    public Button ClothesButton;
    public GameObject DancePanel;
    public GameObject FriendPanel;
    public GameObject Canvas;
    public GameObject SidePanel;
    void Start()
    {
        DancePanel.SetActive(false);
        FriendPanel.SetActive(false);
        DanceButton.onClick.AddListener(() => {
            DancePanel.SetActive(!DancePanel.activeSelf);
        });
        FriendButton.onClick.AddListener(() => {
            FriendPanel.SetActive(!FriendPanel.activeSelf);
        });
        ClothesButton.onClick.AddListener(() => {
                Canvas.SetActive(true);
                SidePanel.SetActive(false);
        });
    }

}
