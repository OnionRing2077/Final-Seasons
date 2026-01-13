using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class LobbyPlayerList : MonoBehaviourPunCallbacks
{
    public Transform contentParent;
    public GameObject playerItemPrefab;

    void Start()
    {
        if (PhotonNetwork.InRoom)
            Refresh();
    }

    public override void OnJoinedRoom()
    {
        Refresh();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Refresh();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Refresh();
    }

    void Refresh()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject item = Instantiate(playerItemPrefab, contentParent);
            item.GetComponent<TMP_Text>().text =
                p.NickName + (p.IsMasterClient ? " (HOST)" : "");
        }
    }
}
