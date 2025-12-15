using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject connectPanel;
    public GameObject lobbyPanel;
    public GameObject roomPanel;

    [Header("UI Elements")]
    public InputField playerNameInput;
    public Text statusText;
    public Transform roomListContainer;
    public GameObject roomButtonPrefab;
    public Text currentRoomName;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        statusText.text = "Connecting to Photon...";
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected! Joining Lobby...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "In Lobby";
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void CreateRoom()
    {
        string roomName = "Room_" + Random.Range(1000, 9999);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 8 });
        statusText.text = "Creating Room...";
    }

    public override void OnCreatedRoom()
    {
        statusText.text = "Room Created!";
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        currentRoomName.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        statusText.text = "Joined Room: " + PhotonNetwork.CurrentRoom.Name;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        statusText.text = "Returned to Lobby";
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContainer)
            Destroy(child.gameObject);

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue;
            GameObject button = Instantiate(roomButtonPrefab, roomListContainer);
            button.GetComponent<RoomButton>().SetRoomInfo(room);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
