using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePanelController : MonoBehaviour
{
    public Button MessageButton;
    public Button FriendButton;
    public GameObject InputField;
    void Start()
    {
        InputField.SetActive(false);
        MessageButton.onClick.AddListener(() => {
                InputField.SetActive(true);
        });
        FriendButton.onClick.AddListener(() => {
                
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
