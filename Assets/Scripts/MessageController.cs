using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Chat.Demo;
using SQLiter;
    public class MessageController : MonoBehaviour
    {
        public string target;
        public GameObject messagePanel;
        public GameObject targetName;
        public GameObject friendPanel;
        public GameObject friendsListItem;
        public GameObject sentMessageTemplate;
        public GameObject replyMessageTemplate;
        public RectTransform ContentTransform;
        public Button friendsListItemButton;
        void Start()
        {
            friendsListItemButton.onClick.AddListener(showMessagePanel);
        }

        void showMessagePanel(){
            ClearMessagePanel();
            target = friendsListItem.GetComponentInChildren<Text>().text;
            targetName.GetComponent<TextMeshProUGUI>().text = target;

            ChatGui chatGuiInstance = ChatGui.Instance;
            int user1Id = SQLiter.SQLite.Instance.GetPlayerId(chatGuiInstance.UserName);
            int user2Id = SQLiter.SQLite.Instance.GetPlayerId(target);

            if (user1Id != -1 && user2Id != -1)  
            {
                List<(int senderId, string message, string timestamp)> Messages = SQLiter.SQLite.Instance.GetAllMessages(user1Id, user2Id);
                foreach (var message in Messages) 
                {
                    int senderId = message.senderId;
                    string messageText = message.message;
                    string timestamp = message.timestamp;

                    if (senderId == user1Id)
                    {
                        GameObject sentMessage = Instantiate(sentMessageTemplate); 
                        sentMessage.GetComponentInChildren<TextMeshProUGUI>().text = chatGuiInstance.UserName + ": " + messageText;
                        sentMessage.transform.SetParent(ContentTransform, false);
                        sentMessage.SetActive(true);
                    }
                    else
                    {
                        GameObject replyMessage = Instantiate(replyMessageTemplate); 
                        replyMessage.GetComponentInChildren<TextMeshProUGUI>().text = target + ": " + messageText;
                        replyMessage.transform.SetParent(ContentTransform, false);
                        replyMessage.SetActive(true);
                    }
                }
            }
            friendPanel.SetActive(false);
            messagePanel.SetActive(true);
        }
        void ClearMessagePanel()
        {
            foreach (Transform child in ContentTransform)
            {
                Destroy(child.gameObject);
            }
        }
    }
