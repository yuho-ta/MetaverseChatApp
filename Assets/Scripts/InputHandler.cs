using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public InputField inputField;
    public Text displayText;

    void Start()
    {
        // OnEndEditイベントにリスナーを追加
        inputField.onEndEdit.AddListener(OnInputFieldEndEdit);
    }

    void OnInputFieldEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            displayText.text = "入力された内容: " + input;
            inputField.text = ""; 
        }
    }
}
