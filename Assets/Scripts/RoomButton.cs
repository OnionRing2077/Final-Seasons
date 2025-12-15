using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class RoomButton : MonoBehaviour
{
    public Text roomNameText;
    private string roomName;

    public void SetRoomInfo(RoomInfo info)
    {
        roomName = info.Name;
        roomNameText.text = info.Name + " (" + info.PlayerCount + "/" + info.MaxPlayers + ")";
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
