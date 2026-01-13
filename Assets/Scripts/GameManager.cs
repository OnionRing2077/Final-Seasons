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

        // 🔥 Spawn Player
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);

        // 🔥 สร้างสีให้ผู้เล่นคนนี้
        Color color = Random.ColorHSV(
            0f, 1f,   // Hue
            0.7f, 1f,// Saturation
            0.7f, 1f // Value
        );

        // 🔥 ส่งชื่อ + สี เข้า PlayerIdentity
        PlayerIdentity id = player.GetComponent<PlayerIdentity>();
        if (id != null)
        {
            id.photonView.RPC(
                "RPC_Setup",
                RpcTarget.AllBuffered,
                PhotonNetwork.NickName,
                color.r, color.g, color.b
            );
        }

        Debug.Log("Spawned player at " + spawnPos);
    }
}
