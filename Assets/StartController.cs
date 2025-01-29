using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StartController : MonoBehaviour
{
    public GameObject SignUpPanel;
    public GameObject LoginPanel;
    public GameObject Canvas;
    public GameObject SidePanel;
    public Button ToLoginButton;
    public Button ToSignUpButton;

    void Start()
    {
        SidePanel.SetActive(false);
        Canvas.SetActive(false);
        LoginPanel.SetActive(false);
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
