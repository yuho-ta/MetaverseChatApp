using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiter;
using TMPro;

public class LoginController : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public GameObject AlertMessage;
    public Button loginButton;
    public GameObject Panel;

    void Start()
    {
        loginButton.onClick.AddListener(HandleLogin);
        AlertMessage.SetActive(false);
    }

    void HandleLogin()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (SQLiter.SQLite.Instance.ValidateUser(username,password))
        {
            Panel.SetActive(true);
        }
        else
        {
            AlertMessage.SetActive(true);
        }
    }
}
