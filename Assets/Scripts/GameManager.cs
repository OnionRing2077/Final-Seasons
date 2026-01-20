using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Not connected to Photon");
            return;
        }

        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        if (index < 0 || index >= spawnPoints.Length)
            index = 0;

        Vector3 spawnPos = spawnPoints[index].position;
        spawnPos.z = 0;

        GameObject player = PhotonNetwork.Instantiate(
            "Player",
            spawnPos,
            Quaternion.identity
        );

        PhotonNetwork.LocalPlayer.TagObject = player;
    }
}
