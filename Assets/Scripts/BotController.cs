using UnityEngine;
using AAA.Gemini;
using UnityEngine.UI;
using Photon.Pun;

public class BotController : MonoBehaviourPunCallbacks
{
    public Button ChatStartButton;
    public GameObject ChatPanel;

    void Start()
    {
        ChatStartButton.onClick.AddListener(()=>
        {
            if (photonView.IsMine)
            {
                chatStart();
            }

        });
        void chatStart()
        {
            ChatPanel.SetActive(true);
        }
    }
}
