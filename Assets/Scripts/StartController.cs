using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Sunbox.Avatars;

public class StartController : MonoBehaviour
{
    public GameObject SignUpPanel;
    public GameObject LoginPanel;
    public GameObject Canvas;
    public GameObject SidePanel;
    public Button ToLoginButton;
    public Button ToSignUpButton;
    public GameObject BotTemplate;
    private int BOT_LIMITS = 1;

    void Start()
    {
        SidePanel.SetActive(false);
        Canvas.SetActive(false);
        LoginPanel.SetActive(false);
        for (int i = 0; i < BOT_LIMITS; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-6.5f, 13.5f), 
                0,                         
                Random.Range(-3f, 17f)      
            );
            GameObject bot = Instantiate(BotTemplate, randomPosition, Quaternion.identity);
            bot.name = $"bot({i})"; 

            AvatarCustomization avatarCustomization = bot.GetComponent<AvatarCustomization>();
            if (avatarCustomization != null)
            {
                avatarCustomization.RandomizeBodyParameters();
                avatarCustomization.RandomizeClothing();
            }
            else
            {
                Debug.LogWarning($"bot({i}) に AvatarCustomization が見つかりませんでした！");
            }
        }
        ToLoginButton.onClick.AddListener(() =>
        {
            SignUpPanel.SetActive(false);
            LoginPanel.SetActive(true);
        });
        ToSignUpButton.onClick.AddListener(() =>
        {
            SignUpPanel.SetActive(true);
            LoginPanel.SetActive(false);
        });
    }
}
