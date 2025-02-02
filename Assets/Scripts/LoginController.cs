using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiter;
using TMPro;
using System.Reflection;
using System.Linq;
using Sunbox.Avatars;
using Photon.Chat.Demo;
using Photon.Pun;

namespace Player{
    public class LoginController : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public GameObject AlertMessage;
        public GameObject LoginPanel;
        public Button loginButton;
        public GameObject SidePanel;
        public AvatarReferences AvatarReferences;
        public GameObject avatarPrefab;
        private GameObject playerAvatar;
        public Canvas canvas;
        public Canvas chatstartcanvas;
        private AvatarCustomization avatarCustomization;
        public Button DanceButton;
        public Button ClapButton;
        public Button WaveButton;
        public string username;
        public string password;

        void Start()
        {
            loginButton.onClick.AddListener(HandleLogin);
            AlertMessage.SetActive(false);
        }

        void HandleLogin()
        {
            username = usernameInput.text;
            password = passwordInput.text;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                AlertMessage.SetActive(true);
            }
            JoinRoom();
            SidePanel.SetActive(true);
            LoginPanel.SetActive(false);
        }
        void JoinRoom()
        {
            if (PhotonNetwork.IsConnectedAndReady) 
            {
                ChatGui chatGui = FindObjectOfType<ChatGui>();
                if (chatGui != null)
                {
                    chatGui.UserName = username.Trim();
                    chatGui.Connect();
                    chatGui.selectedChannelName = "General";
                }
                else
                {
                    Debug.LogError("ChatGui instance not found!");
                }

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
                if (SQLiter.SQLite.Instance.ValidateUser(username,password) != -1)
                {
                    avatarCustomization = playerAvatar.GetComponent<AvatarCustomization>();
                    Dictionary<string, int> playerData = SQLiter.SQLite.Instance.GetPlayerCustomization(username, password);
                    Debug.Log($"{playerData}");
                    ClothingItem top = null;
                    ClothingItem hat = null;
                    ClothingItem bottom = null;
                    ClothingItem glasses = null;
                    ClothingItem shoes = null;
                    FieldInfo[] fields = typeof(AvatarCustomization).GetFields();

                    foreach (var entry in playerData)
                    {
                        string slotType = entry.Key;
                        int index = entry.Value;

                        if (slotType == "hat")
                        {
                            hat = AvatarReferences.AvailableClothingItems[index];
                        }
                        else if (slotType == "bottom")
                        {
                            bottom = AvatarReferences.AvailableClothingItems[index];
                        }
                        else if (slotType == "glasses")
                        {
                            glasses = AvatarReferences.AvailableClothingItems[index];
                        }
                        else if (slotType == "top")
                        {
                            
                            top = AvatarReferences.AvailableClothingItems[index];
                        }
                        else if (slotType == "shoes")
                        {
                            shoes = AvatarReferences.AvailableClothingItems[index];
                        }
                        else if (slotType == "top_material")
                        {
                            if (top != null)
                            {
                                avatarCustomization.AttachClothingItem(top);
                                avatarCustomization.SetClothingItemVariation(Sunbox.Avatars.SlotType.Top, index);
                                avatarCustomization.UpdateClothing();
                            }
                        }
                        else if (slotType == "hat_material")
                        {
                            if (hat != null)
                            {
                                avatarCustomization.AttachClothingItem(hat);
                                avatarCustomization.SetClothingItemVariation(Sunbox.Avatars.SlotType.Hat, index);
                                avatarCustomization.UpdateClothing();
                            }
                        }
                        else if (slotType == "bottom_material")
                        {
                            if (bottom != null)
                            {
                                avatarCustomization.AttachClothingItem(bottom);
                                avatarCustomization.SetClothingItemVariation(Sunbox.Avatars.SlotType.Bottom, index);
                                avatarCustomization.UpdateClothing();
                            }
                        }
                        else if (slotType == "glasses_material")
                        {
                            if (glasses != null)
                            {
                                avatarCustomization.AttachClothingItem(glasses);
                                avatarCustomization.SetClothingItemVariation(Sunbox.Avatars.SlotType.Glasses, index);
                                avatarCustomization.UpdateClothing();
                            }
                        }
                        else if (slotType == "shoes_material")
                        {
                            if (shoes != null)
                            {
                                avatarCustomization.AttachClothingItem(shoes);
                                avatarCustomization.SetClothingItemVariation(Sunbox.Avatars.SlotType.Shoes, index);
                                avatarCustomization.UpdateClothing();
                            }
                        }
                        else if (slotType == "skin" || slotType == "hair_material" || slotType == "eye_material" || slotType == "brow_material")
                        {
                            FieldInfo field = fields.FirstOrDefault(f => f.Name.Replace(" ", "") == slotType.Replace("_", ""));
                            if (field != null)
                            {
                                field.SetValue(playerAvatar, index);
                                avatarCustomization.UpdateCustomization();
                            }
                        }
                    }
                    
                    AvatarManager.Instance.SetAvatarForUser(PhotonNetwork.LocalPlayer.UserId,playerAvatar);
                    BotManager.Instance.setCamera();
                }
                else
                {
                    AlertMessage.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning("Photon connection failed. Try again.");
                AlertMessage.SetActive(true); 
            }
        }
        
    }
}
