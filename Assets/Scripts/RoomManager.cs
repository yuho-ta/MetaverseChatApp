using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiter;
using TMPro;
using System.Reflection;
using System.Linq;
using Sunbox.Avatars;
using Photon.Chat.Demo;
using Photon.Pun;
public class RoomManager : MonoBehaviourPunCallbacks
{
    private string roomName;
    private int BOT_LIMITS = 1;
    public GameObject BotTemplate;
    public GameObject ChatPanel;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnected) 
        {
            PhotonNetwork.ConnectUsingSettings(); 
            Debug.Log("Connecting to Photon...");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        
        // Join the default lobby first before trying to join rooms
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby successfully!");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("No random rooms available, creating a new room...");
        CreateRoom();
    }

    void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4; 
        roomName = "Room_" + PhotonNetwork.NickName; 

        PhotonNetwork.CreateRoom(roomName, roomOptions); 
        Debug.Log("Creating room...");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room successfully!");
        for (int i = 0; i < BOT_LIMITS; i++)
        {
            Vector3 randomPosition;

            do {
                randomPosition = new Vector3(
                    Random.Range(5, 8), 
                    0,                         
                    Random.Range(10, 15)      
                );
            } while (randomPosition.x == 5f && randomPosition.y == 0f && randomPosition.z == 7f);
            Quaternion spawnRotation = Quaternion.Euler(0, 180, 0);
            GameObject bot = PhotonNetwork.Instantiate(this.BotTemplate.name, randomPosition, spawnRotation);
            BotController botController = bot.transform.Find("ChatCanvas").GetComponent<BotController>();
            botController.ChatPanel = ChatPanel;
            bot.name = $"bot({i})"; 

            AvatarCustomization avatarCustomization = bot.GetComponent<AvatarCustomization>();
            if (avatarCustomization != null)
            {
                avatarCustomization.RandomizeBodyParameters();
                avatarCustomization.RandomizeClothing();
                bot.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"bot({i}) に AvatarCustomization が見つかりませんでした！");
            }
            BotManager.Instance.SetBot(bot,i);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to create room: " + message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from Photon: {cause}");
    }
}