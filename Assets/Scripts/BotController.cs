using UnityEngine;
using AAA.Gemini;
using UnityEngine.UI; 

public class BotController : MonoBehaviour
{
    public GameObject Bot;
    public Button ChatStartButton;
    public GameObject ChatPanel;
    private int BOT_LIMITS = 5;

    void Start()
    {
        ChatStartButton.onClick.AddListener(chatStart);
        void chatStart(){
            ChatPanel.SetActive(true);
        }
    }
}
