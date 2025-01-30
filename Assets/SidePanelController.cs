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
    public GameObject Canvas;
    public GameObject SidePanel;
    void Start()
    {
        DancePanel.SetActive(false);
        DanceButton.onClick.AddListener(() => {
            DancePanel.SetActive(!DancePanel.activeSelf);
        });
        FriendButton.onClick.AddListener(() => {
                
        });
        ClothesButton.onClick.AddListener(() => {
                Canvas.SetActive(true);
                SidePanel.SetActive(false);
        });
    }

}
