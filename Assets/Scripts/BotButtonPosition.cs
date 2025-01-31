using UnityEngine;

public class BotButtonPosition : MonoBehaviour
{
    public GameObject bot; 
    public GameObject canvas; 

    void Start()
    {
        Vector3 botPosition = bot.transform.position;

        canvas.transform.position = botPosition + new Vector3(0, 1.2f, 0); 
    }
}
