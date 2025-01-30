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
        public static int playerId;
        public GameObject AlertMessage;
        public GameObject LoginPanel;
        public Button loginButton;
        public GameObject SidePanel;
        public AvatarReferences AvatarReferences;
        public AvatarCustomization Avatar;

        void Start()
        {
            loginButton.onClick.AddListener(HandleLogin);
            AlertMessage.SetActive(false);
        }

        void HandleLogin()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
            playerId = SQLiter.SQLite.Instance.ValidateUser(username, password);

            if (playerId != -1)
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
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Top, index);
                            Avatar.AttachClothingItem(top, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "hat_material")
                    {
                        if (hat != null)
                        {
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Hat, index);
                            Avatar.AttachClothingItem(hat, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "bottom_material")
                    {
                        if (bottom != null)
                        {
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Bottom, index);
                            Avatar.AttachClothingItem(bottom, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "glasses_material")
                    {
                        if (glasses != null)
                        {
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Glasses, index);
                            Avatar.AttachClothingItem(glasses, index);
                            Avatar.UpdateClothing();
                        }
                    }
                    else if (slotType == "shoes_material")
                    {
                        if (shoes != null)
                        {
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Shoes, index);
                            Avatar.AttachClothingItem(shoes, index);
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
            }
            else
            {
                AlertMessage.SetActive(true);
            }
            SidePanel.SetActive(true);
            LoginPanel.SetActive(false);
        }
    }
}

