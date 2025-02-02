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