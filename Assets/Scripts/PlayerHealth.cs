using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun
{
    public bool IsDead { get; private set; }

    [Header("Prefabs (Resources)")]
    public GameObject deadBodyPrefab;

    GhostMode ghost;

    void Awake()
    {
        ghost = GetComponent<GhostMode>();
    }

    [PunRPC]
    public void RPC_Die(int killerActorNumber)
    {
        if (IsDead) return;
        IsDead = true;

        // 1) Spawn dead body (ให้ทุกคนเห็น)
        if (deadBodyPrefab != null)
        {
            PhotonNetwork.Instantiate(
                deadBodyPrefab.name,
                transform.position,
                Quaternion.identity
            );
        }

        // 2) Switch THIS SAME player into Ghost (ไม่ spawn ตัวใหม่)
        if (ghost != null)
            ghost.EnterGhost();
    }
}
