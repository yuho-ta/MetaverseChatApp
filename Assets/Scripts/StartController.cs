using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Sunbox.Avatars;

public class StartController : MonoBehaviourPunCallbacks
{
    public GameObject SignUpPanel;
    public GameObject LoginPanel;
    public GameObject Canvas;
    public GameObject SidePanel;
    public Button ToLoginButton;
    public Button ToSignUpButton;
    public GameObject BotTemplate;
    public GameObject ChatPanel;
    
    void Start()
    {
        SidePanel.SetActive(false);
        Canvas.SetActive(false);
        LoginPanel.SetActive(false);
        BotTemplate.SetActive(false);
        ChatPanel.SetActive(false);
        
        ToLoginButton.onClick.AddListener(() =>
        {
            if (photonView.IsMine)
            { 
                SignUpPanel.SetActive(false);
                LoginPanel.SetActive(true);
            }
        });
        ToSignUpButton.onClick.AddListener(() =>
        {
            if (photonView.IsMine)
            { 
                SignUpPanel.SetActive(true);
                LoginPanel.SetActive(false);
            }
        });
    }
}
