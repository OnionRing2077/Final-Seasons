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

public PlayerRole GetRole()
{
    if (photonView.Owner.CustomProperties.TryGetValue("role", out object r))
        return (PlayerRole)(int)r;

    return PlayerRole.Civilian;
}

    [PunRPC]
public void RPC_Die(int killerActorNumber)
{
    if (IsDead) return;
    IsDead = true;

    if (deadBodyPrefab != null)
    {
        GameObject bodyObj = PhotonNetwork.Instantiate(
            deadBodyPrefab.name,
            transform.position,
            Quaternion.identity
        );

        DeadBodyVisual body = bodyObj.GetComponent<DeadBodyVisual>();

        if (body != null &&
            photonView.Owner.CustomProperties.TryGetValue("color", out object v))
        {
            int colorIndex = (int)v;

            // 🔥 ส่ง RPC ให้ทุกคน
            body.photonView.RPC(
                "RPC_SetColor",
                RpcTarget.AllBuffered,
                colorIndex
            );
        }
    }

    if (ghost != null)
        ghost.EnterGhost();
}

}
