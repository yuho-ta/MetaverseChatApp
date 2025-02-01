using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Chat.Demo;
namespace Photon.Chat.Demo
{
    public class MessageController : MonoBehaviour
    {
        public string target;
        public GameObject messagePanel;
        public GameObject friendPanel;
        public GameObject friendsListItem;
        public GameObject sentMessageTemplate;
        public GameObject replyMessageTemplate;
        public RectTransform ContentTransform;
        public Button chatSendButton;
        public Button friendsListItemButton;
        public TMP_InputField chatSendInput;
        void Start()
        {
            messagePanel.SetActive(false);
            sentMessageTemplate.SetActive(false);
            replyMessageTemplate.SetActive(false);
            friendsListItemButton.onClick.AddListener(showMessagePanel);
            chatSendButton.onClick.AddListener(sendMessage);
        }

        void showMessagePanel(){
            friendPanel.SetActive(false);
            messagePanel.SetActive(true);
            target = friendsListItem.GetComponentInChildren<Text>().text;
        }
        void sendMessage()
        {
            string inputLine = "\\msg " + target + " " + chatSendInput.text;
            ChatGui chatGuiInstance = FindObjectOfType<ChatGui>();
            chatGuiInstance.SendChatMessage(inputLine);
            GameObject sentMessage = (GameObject)Instantiate(sentMessageTemplate);
            sentMessage.GetComponentInChildren<TextMeshProUGUI>().text = chatSendInput.text;
            sentMessage.transform.SetParent(ContentTransform, false);
            sentMessage.gameObject.SetActive(true);

        }
    }
}

