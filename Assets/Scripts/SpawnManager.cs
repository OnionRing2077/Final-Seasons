using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints; // ใส่จุดเกิดไว้ 8 จุด

    void Start()
    {
        if (!PhotonNetwork.InRoom) return;

        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
        Vector3 pos = spawnPoints[index].position;

        PhotonNetwork.Instantiate("Dark_Oracle", pos, Quaternion.identity);
    }
}
