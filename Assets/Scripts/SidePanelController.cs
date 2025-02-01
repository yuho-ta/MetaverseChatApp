using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat.Demo;
using TMPro;

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
    public GameObject FriendsListItem;
    public GameObject SentMessageTemplate;
    public GameObject ReplyMessageTemplate;
    public Button MessagePanelBackButton;
    public Button FriendPanelBackButton;
    public Button ChatSendButton;
    public RectTransform ContentTransform;
    public TMP_InputField ChatSendInput;
    
    void Start()
    {
        MessagePanel.SetActive(false);
        SentMessageTemplate.SetActive(false);
        ReplyMessageTemplate.SetActive(false);
        DancePanel.SetActive(false);
        FriendPanel.SetActive(false);
        FriendsListItem.SetActive(false);
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
        ChatSendButton.onClick.AddListener(() => {
            sendMessage();
        });
    }
    void sendMessage()
        {
            ChatGui chatGuiInstance = ChatGui.Instance;
            string target = MessagePanel.GetComponentInChildren<TextMeshProUGUI>().text;
            string inputLine = "\\msg " + target + " " + ChatSendInput.text;
            chatGuiInstance.SendChatMessage(inputLine);
            Debug.Log("Done");
            GameObject sentMessage = (GameObject)Instantiate(SentMessageTemplate);
            sentMessage.GetComponentInChildren<TextMeshProUGUI>().text = ChatSendInput.text;
            sentMessage.transform.SetParent(ContentTransform, false);
            sentMessage.gameObject.SetActive(true);
    }

}
