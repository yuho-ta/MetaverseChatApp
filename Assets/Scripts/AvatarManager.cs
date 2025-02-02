using System.Collections.Generic;
using UnityEngine;
using Sunbox.Avatars;

public class AvatarManager : MonoBehaviour
{
    public static AvatarManager Instance;
    private AvatarCustomization avatarCustomization;

    private Dictionary<string, GameObject> userAvatars = new Dictionary<string,GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetAvatarForUser(string userId, GameObject avatar)
    {
        if (!userAvatars.ContainsKey(userId))
        {
            userAvatars.Add(userId, avatar);
        }
        else
        {
            userAvatars[userId] = avatar;
        }
    }

    public GameObject GetAvatarForUser(string userId)
    {
        return userAvatars.ContainsKey(userId) ? userAvatars[userId] : null;
    }
}
