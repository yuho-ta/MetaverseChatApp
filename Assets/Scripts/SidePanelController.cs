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
    public GameObject MessagePanel;
    public GameObject Canvas;
    public GameObject SidePanel;
    public GameObject Side;
    public GameObject friendsListItem;
    public Button MessagePanelBackButton;
    public Button FriendPanelBackButton;
    void Start()
    {
        DancePanel.SetActive(false);
        FriendPanel.SetActive(false);
        friendsListItem.SetActive(false);
        DanceButton.onClick.AddListener(() => {
            DancePanel.SetActive(!DancePanel.activeSelf);
        });
        FriendButton.onClick.AddListener(() => {
            FriendPanel.SetActive(!FriendPanel.activeSelf);
            Side.SetActive(false);
        });
        ClothesButton.onClick.AddListener(() => {
                Canvas.SetActive(true);
                SidePanel.SetActive(false);
        });
        MessagePanelBackButton.onClick.AddListener(() => {
            MessagePanel.SetActive(false);
            FriendPanel.SetActive(true);
        });
        FriendPanelBackButton.onClick.AddListener(() => {
            FriendPanel.SetActive(false);
            Side.SetActive(true);
        });
    }

}
