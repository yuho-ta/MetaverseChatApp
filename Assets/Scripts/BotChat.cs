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
            SentMessageTemplate.SetActive(false);
            ReplyMessageTemplate.SetActive(false);
            BackButton.onClick.AddListener(()=>ChatPanel.SetActive(false));
            SendButton.onClick.AddListener(SendMessageToChatGPT);
        }
        public async void SendMessageToChatGPT()
        {
            string userMessage = MessageInput.text;
            GameObject sentMessageInstance = Instantiate(SentMessageTemplate);
            sentMessageInstance.transform.SetParent(ContentTransform, false);
            sentMessageInstance.GetComponentInChildren<TextMeshProUGUI>().text = userMessage;
            try
            {
                var response = await chatGPT.RequestAsync(userMessage);
                string botReply = response.choices[0].message.content;

                Debug.Log($"Bot({name}): {botReply}");

                if (botReply != null)
                {
                    GameObject replyMessageInstance = Instantiate(ReplyMessageTemplate);
                    replyMessageInstance.transform.SetParent(ContentTransform, false);
                    replyMessageInstance.GetComponentInChildren<TextMeshProUGUI>().text = botReply;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ChatGPTError: {e.Message}");
            }
        }
    }
}
