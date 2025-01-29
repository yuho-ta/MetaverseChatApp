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

                ClothingItem top = null;
                FieldInfo[] fields = typeof(AvatarCustomization).GetFields();

                foreach (var entry in playerData)
                {
                    string slotType = entry.Key;
                    int index = entry.Value;

                    if (slotType == "hat")
                    {
                        ClothingItem selectedClothingItem = AvatarReferences.AvailableClothingItems[index];
                        Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Hat, 0);
                        Avatar.AttachClothingItem(selectedClothingItem);
                        Avatar.UpdateClothing();
                    }
                    else if (slotType == "bottom")
                    {
                        ClothingItem selectedClothingItem = Avatar.AvatarReferences.AvailableClothingItems[index];
                        Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Bottom, 0);
                        Avatar.AttachClothingItem(selectedClothingItem);
                        Avatar.UpdateClothing();
                    }
                    else if (slotType == "glasses")
                    {
                        ClothingItem selectedClothingItem = Avatar.AvatarReferences.AvailableClothingItems[index];
                        Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Glasses, 0);
                        Avatar.AttachClothingItem(selectedClothingItem);
                        Avatar.UpdateClothing();
                    }
                    else if (slotType == "top")
                    {
                        ClothingItem selectedClothingItem = Avatar.AvatarReferences.AvailableClothingItems[index];
                        Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Top, 0);
                        Avatar.AttachClothingItem(selectedClothingItem);
                        Avatar.UpdateClothing();
                    }
                    else if (slotType == "top_material")
                    {
                        if (top != null)
                        {
                            Avatar.SetClothingItemVariation(Sunbox.Avatars.SlotType.Top, index);
                            Avatar.AttachClothingItem(top);
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
        }
    }
}

