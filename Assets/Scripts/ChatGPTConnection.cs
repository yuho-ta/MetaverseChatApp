using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AAA.OpenAI
{
    public class ChatGPTConnection
    {
        private readonly string _apiKey;
        private readonly List<ChatGPTMessageModel> _messageList = new();

        public ChatGPTConnection(string apiKey)
        {
            _apiKey = apiKey;
            _messageList.Add(
                new ChatGPTMessageModel(){role = "system",content = "語尾に「にゃ」をつけて"});
        }

        public async UniTask<ChatGPTResponseModel> RequestAsync(string userMessage)
        {
            var apiUrl = "https://api.openai.com/v1/chat/completions";

            _messageList.Add(new ChatGPTMessageModel {role = "user", content = userMessage});
            
            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + _apiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };

            var options = new ChatGPTCompletionRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = _messageList
            };
            var jsonOptions = JsonUtility.ToJson(options);

            Debug.Log("Me:" + userMessage);

            using var request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
                Debug.Log("ChatGPT:" + responseObject.choices[0].message.content);
                _messageList.Add(responseObject.choices[0].message);
                return responseObject;
            }
        }
        
    }
    [System.Serializable]
    public class ChatGPTResponseModel
    {
        public string id;
        public string @object;
        public int created;
        public Choice[] choices;
        public Usage usage;

        [System.Serializable]
        public class Choice
        {
            public int index;
            public ChatGPTMessageModel message;
            public string finish_reason;
        }

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }
    [System.Serializable]
    public class ChatGPTMessageModel
    {
        public string role;
        public string content;
    }
    [System.Serializable]
    public class ChatGPTCompletionRequestModel
    {
        public string model;
        public List<ChatGPTMessageModel> messages;
    }
}
