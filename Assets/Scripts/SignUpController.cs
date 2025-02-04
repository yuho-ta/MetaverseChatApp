using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiter;
using TMPro;
using Sunbox.Avatars;
using Photon.Chat.Demo;

namespace Player{
    public class SignUpController : MonoBehaviourPunCallbacks
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public GameObject AlertMessage;
        public GameObject Panel;
        public Button signupButton;
        public GameObject avatarPrefab; 
        private GameObject playerAvatar;
        public Canvas canvas;
        public Canvas chatstartcanvas;
        public Button DanceButton;
        public Button ClapButton;
        public Button WaveButton;
        private AvatarCustomization avatarCustomization;

        void Start()
        {
            usernameInput.onValueChanged.RemoveAllListeners();
            usernameInput.onValueChanged.AddListener((text) =>
            {
                if (photonView.IsMine)
                {
                    usernameInput.text = text;
                }
            });
            passwordInput.onValueChanged.RemoveAllListeners();
            passwordInput.onValueChanged.AddListener((text) =>
            {
                if (photonView.IsMine)
                {
                    passwordInput.text = text;
                }
            });
            signupButton.onClick.AddListener(() =>
            {
                if (photonView.IsMine)
                { 
                    HandleSignup();
                }
            });
            AlertMessage.SetActive(false);
        }

        void HandleSignup()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
            Debug.Log($"username: '{username}', password: '{password}'");
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                AlertMessage.SetActive(true);
            }
            else
            {
                int playerId = SQLiter.SQLite.Instance.GetPlayerId(username);
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
                        chatGui.selectedChannelName = "General";
                        chatGui.SendChatMessage("\\subscribe GeneralChat");
                        if (PhotonNetwork.IsConnected)
                        {
                            Vector3 spawnPosition = new Vector3(5,0.1f,11);
                            Quaternion spawnRotation = Quaternion.Euler(0, 180, 0);
                            playerAvatar = PhotonNetwork.Instantiate(avatarPrefab.name, spawnPosition, spawnRotation);
                            PlayerMovement[] playerMovements = playerAvatar.GetComponentsInChildren<PlayerMovement>();
                            Camera camera = playerAvatar.GetComponentInChildren<Camera>();
                            camera.transform.position = new Vector3(5,0,8);
                            camera.transform.rotation = Quaternion.Euler(0, 0, 0);
                            chatstartcanvas.worldCamera = camera;
                            foreach (PlayerMovement playerMovement in playerMovements)
                            {
                                playerMovement.DanceButton = DanceButton;
                                playerMovement.ClapButton = ClapButton;
                                playerMovement.WaveButton = WaveButton;
                                playerMovement.cameraTransform = camera.transform;
                                playerMovement.canvas = canvas;
                            }
                        }
                        AvatarManager.Instance.SetAvatarForUser(PhotonNetwork.LocalPlayer.UserId,playerAvatar);
                        BotManager.Instance.setCamera();
                    }
                    else
                    {
                        Debug.LogError("ChatGui instance not found!");
                    }

                    Panel.SetActive(true);
                    SQLiter.SQLite.Instance.InsertPlayer(username, password);
                }
            }
        }
    }
}