using UnityEngine;
using AAA.Gemini;
using UnityEngine.UI;

public class BotController : MonoBehaviour
{
    public Button ChatStartButton;
    public GameObject ChatPanel;

    void Start()
    {
        ChatStartButton.onClick.AddListener(chatStart);
        void chatStart()
        {
            ChatPanel.SetActive(true);
        }
    }
}
