using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_InputField nameInput;
    public TMP_InputField roomInput;
    public TMP_Text statusText;

    [Header("Settings")]
    public string gameSceneName = "GameScene";
    public byte maxPlayers = 8;

    bool lobbyReady = false;

    public override void OnJoinedLobby()
    {
        lobbyReady = true;
        statusText.text = "Connected. Ready!";
    }

    // ---------- NAME ----------
    public void SetPlayerName()
    {
        string n = nameInput.text.Trim();
        if (string.IsNullOrEmpty(n))
            n = "Player" + Random.Range(1000, 9999);

        PhotonNetwork.NickName = n;
        statusText.text = "Name: " + n;
    }

    // ---------- CREATE ----------
    public void CreateRoom()
    {
        if (!lobbyReady)
        {
            statusText.text = "Connecting...";
            return;
        }

        string roomName = roomInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
            roomName = "Room_" + Random.Range(1000, 9999);

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = maxPlayers,
            IsOpen = true,
            IsVisible = true
        };

        statusText.text = "Creating room...";
        PhotonNetwork.CreateRoom(roomName, options);
    }

    // ---------- JOIN ----------
    public void JoinRoom()
    {
        if (!lobbyReady)
        {
            statusText.text = "Connecting...";
            return;
        }

        string roomName = roomInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            statusText.text = "Enter room name!";
            return;
        }

        statusText.text = "Joining...";
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Joined Room!";
        PhotonNetwork.LoadLevel(gameSceneName);
    }

    public override void OnJoinRoomFailed(short code, string msg)
    {
        statusText.text = "Join failed: " + msg;
    }

    public override void OnCreateRoomFailed(short code, string msg)
    {
        statusText.text = "Create failed: " + msg;
    }
}
