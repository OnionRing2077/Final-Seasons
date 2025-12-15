using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManagerNew : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomInputField;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
        }
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomInputField.text))
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 8 };
            PhotonNetwork.CreateRoom(roomInputField.text, roomOptions);
            Debug.Log("Creating room: " + roomInputField.text);
        }
        else
        {
            Debug.LogWarning("Room name is empty!");
        }
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrEmpty(roomInputField.text))
        {
            PhotonNetwork.JoinRoom(roomInputField.text);
            Debug.Log("Joining room: " + roomInputField.text);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
{
    Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
    PhotonNetwork.LoadLevel("GameScene"); // ต้องมีชื่อ Scene ตรงนี้
}
}
