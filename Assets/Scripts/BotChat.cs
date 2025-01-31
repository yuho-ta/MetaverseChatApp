using System;
using UnityEngine;
using UnityEngine.UI; 
using Cysharp.Threading.Tasks;
using TMPro;

namespace AAA.OpenAI
{
    public class BotChat : MonoBehaviour
    {
        private ChatGPTConnection chatGPT;
        private string apiKey = "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";
        public Button BackButton; 
        public Button SendButton; 
        public GameObject ChatPanel;
        public GameObject SentMessageTemplate;
        public GameObject ReplyMessageTemplate;
        public TMP_InputField MessageInput;
        public RectTransform ContentTransform;

        private void Start()
        {
            chatGPT = new ChatGPTConnection(apiKey);
            BackButton.onClick.AddListener(()=>ChatPanel.SetActive(false));
            SendButton.onClick.AddListener(SendMessageToChatGPT);
        }
        public async void SendMessageToChatGPT()
        {
            string userMessage = MessageInput.text;
            GameObject sentMessageInstance = Instantiate(SentMessageTemplate);
            sentMessageInstance.transform.SetParent(ContentTransform, false);
            sentMessageInstance.GetComponent<TextMeshProUGUI>().text = userMessage;
            try
            {
                var response = await chatGPT.RequestAsync(userMessage);
                string botReply = response.choices[0].message.content;

                Debug.Log($"Bot({name}): {botReply}");

                if (botReply != null)
                {
                    GameObject replyMessageInstance = Instantiate(ReplyMessageTemplate);
                    replyMessageInstance.transform.SetParent(ContentTransform, false);
                    replyMessageInstance.GetComponent<TextMeshProUGUI>().text = botReply;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ChatGPTError: {e.Message}");
            }
        }
    }
}
