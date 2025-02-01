using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Chat.Demo;
    public class MessageController : MonoBehaviour
    {
        public string target;
        public GameObject messagePanel;
        public GameObject targetName;
        public GameObject friendPanel;
        public GameObject friendsListItem;
        public Button friendsListItemButton;
        void Start()
        {
            friendsListItemButton.onClick.AddListener(showMessagePanel);
        }

        void showMessagePanel(){
            friendPanel.SetActive(false);
            messagePanel.SetActive(true);
            target = friendsListItem.GetComponentInChildren<Text>().text;
            targetName.GetComponent<TextMeshProUGUI>().text = target;
            messagePanel.GetComponentInChildren<TextMeshProUGUI>().text = target;
        }
    }
