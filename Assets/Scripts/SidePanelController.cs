using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat.Demo;
using SQLiter;
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
    public GameObject TargetName;
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
            string target = TargetName.GetComponent<TextMeshProUGUI>().text;
            string message = ChatSendInput.text;
            ChatSendInput.text = "";
            string inputLine = "\\msg " + target + " " + message;
            chatGuiInstance.SendChatMessage(inputLine);
        
            int senderId = SQLiter.SQLite.Instance.GetPlayerId(target);
            int playerId = SQLiter.SQLite.Instance.GetPlayerId(chatGuiInstance.UserName);
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            SQLiter.SQLite.Instance.InsertMessage(playerId, senderId, message.ToString(), timestamp);


            GameObject sentMessage = (GameObject)Instantiate(SentMessageTemplate);
            sentMessage.GetComponentInChildren<TextMeshProUGUI>().text = chatGuiInstance.UserName + ": "+ ChatSendInput.text;
            sentMessage.transform.SetParent(ContentTransform, false);
            sentMessage.gameObject.SetActive(true);
    }

}
