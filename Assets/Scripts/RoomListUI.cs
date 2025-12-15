using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class RoomListUI : MonoBehaviourPunCallbacks
{
    public GameObject roomButtonPrefab;
    public Transform roomListContainer;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContainer)
            Destroy(child.gameObject);

        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList) continue;
            GameObject btn = Instantiate(roomButtonPrefab, roomListContainer);
            btn.GetComponentInChildren<Text>().text = info.Name + " (" + info.PlayerCount + "/" + info.MaxPlayers + ")";
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(info.Name);
            });
        }
    }
}
