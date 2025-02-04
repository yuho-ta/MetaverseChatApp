using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public static BotManager Instance { get; private set; }
    private static int BOT_LIMITS = 1;
    private GameObject[] bot = new GameObject[BOT_LIMITS];

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void SetBot(GameObject botPrefab, int index)
    {
        if (index >= 0 && index < BOT_LIMITS)
        {
            bot[index] = botPrefab;  
        }
    }

    public void setCamera()
    {
        GameObject userAvatar = AvatarManager.Instance.GetAvatarForUser(PhotonNetwork.LocalPlayer.UserId);
        Camera userCamera = userAvatar?.GetComponentInChildren<Camera>(); 

        if (userCamera != null)
        {
            foreach (GameObject botObject in bot)
            {
                if (botObject != null)
                {
                    botObject.GetComponentInChildren<Canvas>().worldCamera = userCamera; 
                }
            }
        }
        else
        {
            Debug.LogError("User Avatar or Camera not found.");
        }
    }
}
