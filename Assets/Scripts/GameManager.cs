using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    public RoleManager roleManager;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Not connected to Photon");
            return;
        }

        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(AssignRolesDelayed), 1f);
        }
    }

    void AssignRolesDelayed()
    {
        if (roleManager == null)
        {
            Debug.LogError("RoleManager is NULL! ใส่ RoleManager ใน GameManager");
            return;
        }

        roleManager.AssignRoles();
        PhotonNetwork.LoadLevel("RoleRevealScene");
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

        // ⭐ สำคัญ: ใช้ TagObject สำหรับ RoleManager
        PhotonNetwork.LocalPlayer.TagObject = player;
    }
}
