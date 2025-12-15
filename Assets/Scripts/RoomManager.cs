using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4));
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        }
    }
}
