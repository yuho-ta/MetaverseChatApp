using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiter;
using TMPro;
using System.Reflection;
using System.Linq;
using Sunbox.Avatars;

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
        public AvatarCustomization Avatar;
        public PlayerController playerController;

        void Start()
        {
            loginButton.onClick.AddListener(HandleLogin);
            AlertMessage.SetActive(false);
        }

        void HandleLogin()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
            Debug.Log($"username: '{username}', password: '{password}'");
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                AlertMessage.SetActive(true);
            }
            playerController.username = username;
            playerController.password = password;
            playerController.playerId = SQLiter.SQLite.Instance.ValidateUser(username, password);

            if (playerController.playerId != -1)
            {
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
                            Avatar.AttachClothingItem(top);
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Top, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "hat_material")
                    {
                        if (hat != null)
                        {
                            Avatar.AttachClothingItem(hat);
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Hat, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "bottom_material")
                    {
                        if (bottom != null)
                        {
                            Avatar.AttachClothingItem(bottom);
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Bottom, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "glasses_material")
                    {
                        if (glasses != null)
                        {
                            Avatar.AttachClothingItem(glasses);
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Glasses, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "shoes_material")
                    {
                        if (shoes != null)
                        {
                            Avatar.AttachClothingItem(shoes);
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Shoes, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "skin" || slotType == "hair_material" || slotType == "eye_material" || slotType == "brow_material")
                    {
                        FieldInfo field = fields.FirstOrDefault(f => f.Name.Replace(" ", "") == slotType.Replace("_", ""));
                        if (field != null)
                        {
                            field.SetValue(Avatar, index);
                            Avatar.UpdateCustomization();
                        }
                    }
                }
                SidePanel.SetActive(true);
                LoginPanel.SetActive(false);
            }
            else
            {
                AlertMessage.SetActive(true);
            }
        }
    }
}
