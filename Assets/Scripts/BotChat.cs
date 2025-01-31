using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace AAA.Gemini
{
    public class BotChat : MonoBehaviour
    {
        private GeminiConnection gemini;
        private string apiKey = "AIzaSyB_0v8PkkzByBg9VHS7t9q9yPp8U2uqwsc";
        public Button BackButton;
        public Button SendButton;
        public GameObject ChatPanel;
        public GameObject SentMessageTemplate;
        public GameObject ReplyMessageTemplate;
        public TMP_InputField MessageInput;
        public RectTransform ContentTransform;

        private void Start()
        {
            gemini = new GeminiConnection(apiKey);
            SentMessageTemplate.SetActive(false);
            ReplyMessageTemplate.SetActive(false);
            BackButton.onClick.AddListener(() => ChatPanel.SetActive(false));
            SendButton.onClick.AddListener(SendMessageToGemini);
        }

        public async void SendMessageToGemini()
        {
            string userMessage = MessageInput.text;

            GameObject sentMessageInstance = Instantiate(SentMessageTemplate);
            sentMessageInstance.transform.SetParent(ContentTransform, false);
            sentMessageInstance.GetComponentInChildren<TextMeshProUGUI>().text = "Me: " + userMessage;
            sentMessageInstance.SetActive(true);

            try
            {
                var botReply = await gemini.RequestAsync(userMessage);
                Debug.Log($"Bot({name}): {botReply}");

                if (!string.IsNullOrEmpty(botReply))
                {
                    GameObject replyMessageInstance = Instantiate(ReplyMessageTemplate);
                    replyMessageInstance.transform.SetParent(ContentTransform, false);
                    replyMessageInstance.GetComponentInChildren<TextMeshProUGUI>().text = "Gemini: " + botReply;
                    replyMessageInstance.SetActive(true);
                }

                Canvas.ForceUpdateCanvases();
                ContentTransform.anchoredPosition = new Vector2(0, Mathf.Min(0, ContentTransform.sizeDelta.y));
            }
            catch (Exception e)
            {
                Debug.LogError($"Gemini API Error: {e.Message}");
            }
        }

    }
}
