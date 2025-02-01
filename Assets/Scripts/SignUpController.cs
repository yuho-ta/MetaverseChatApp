using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiter;
using TMPro;
using Photon.Chat.Demo;

namespace Player{
    public class SignUpController : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public GameObject AlertMessage;
        public GameObject Panel;
        public Button signupButton;
        public PlayerController playerController;

        void Start()
        {
            signupButton.onClick.AddListener(HandleSignup);
            AlertMessage.SetActive(false);
        }

        void HandleSignup()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
            playerController.username = username;
            playerController.password = password;
            Debug.Log($"username: '{username}', password: '{password}'");
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                AlertMessage.SetActive(true);
            }
            else
            {
                int playerId = SQLiter.SQLite.Instance.ValidateUser(username, password);
                Debug.Log($"   PlayerId:{playerId}");
                if (playerId != -1)
                {
                    AlertMessage.SetActive(true);
                }
                else
                {
                    ChatGui chatGui = FindObjectOfType<ChatGui>(); 
                    if (chatGui != null)
                    {
                        chatGui.UserName = username.Trim();
                        chatGui.Connect();
                    }
                    else
                    {
                        Debug.LogError("ChatGui instance not found!");
                    }

                    Panel.SetActive(true);
                    SQLiter.SQLite.Instance.InsertPlayer(username, password);
                    playerController.playerId = SQLiter.SQLite.Instance.ValidateUser(username, password);
                }
            }
        }
    }
}