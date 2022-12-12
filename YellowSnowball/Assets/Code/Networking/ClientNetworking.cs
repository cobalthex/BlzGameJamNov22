
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
public enum NetworkingState
{
    NotConnected,
    Connecting,
    InLobby,
    JoiningRoom,
    InRoom,
    Disconnected
}

public class ClientNetworking : MonoBehaviourPunCallbacks
{
    private bool m_attemptingConnection = false;
    private bool m_connectedToServer = false;
    private NetworkingState m_networkingState = NetworkingState.NotConnected;
    private static RoomOptions m_roomOptions = new RoomOptions() { MaxPlayers = 2 };

    public bool IsConnectedToRoom()
    {
        return m_networkingState == NetworkingState.InRoom;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (m_networkingState == NetworkingState.NotConnected)
        {
            InitializeClient();
        }
    }

    public override void OnDisable()
    {
        PhotonNetwork.Disconnect();
        m_networkingState = NetworkingState.NotConnected;
        Debug.Log("Disconnected.");
        base.OnDisable();
    }

    public void InitializeClient()
    {
        m_networkingState = NetworkingState.Connecting;
        Debug.Log("Initializing client.");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Client is connected to the server.");
        m_networkingState = NetworkingState.InLobby;
        JoinRoom();
    }

    public void JoinRoom()
    {
        Debug.Log("Attempting to join room");
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom()
    {
        Debug.Log("Creating room");
        PhotonNetwork.CreateRoom(null, m_roomOptions);
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room.");
        CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room.");
        CreateRoom();
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        m_connectedToServer = false;
        Debug.Log($"Disconnected due to {cause}");
        m_networkingState = NetworkingState.Disconnected;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created: " + PhotonNetwork.CurrentRoom.Name);
        m_networkingState = NetworkingState.InRoom;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined: " + PhotonNetwork.CurrentRoom.Name);
        m_networkingState = NetworkingState.InRoom;
    }
}