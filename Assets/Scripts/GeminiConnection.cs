using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class GeminiConnection
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;
    private const string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

    public GeminiConnection(string apiKey)
    {
        this.apiKey = apiKey;
        httpClient = new HttpClient();
    }

    public async Task<string> RequestAsync(string prompt)
    {
        try
        {
            var requestData = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            string jsonContent = JsonConvert.SerializeObject(requestData);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}?key={apiKey}")
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GeminiResponse>(jsonResponse);

            return responseObject?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "No response from Gemini";
        }
        catch (Exception e)
        {
            Debug.LogError($"Gemini API Error: {e.Message}");
            return "Error: Unable to get a response.";
        }
    }
}

public class GeminiResponse
{
    public Candidate[] Candidates { get; set; }
}

public class Candidate
{
    public Message Content { get; set; }
}

public class Message
{
    public Part[] Parts { get; set; }
}

public class Part
{
    public string Text { get; set; }
}
