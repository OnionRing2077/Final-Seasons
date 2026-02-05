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

        // 1. Only the owner spawns the dead body to prevent duplicates
        if (photonView.IsMine && deadBodyPrefab != null)
        {
            GameObject bodyObj = PhotonNetwork.Instantiate(
                deadBodyPrefab.name,
                transform.position,
                Quaternion.identity
            );

            DeadBody deadBody = bodyObj.GetComponent<DeadBody>();
            if (deadBody != null)
            {
                deadBody.Init(photonView.Owner.ActorNumber);
            }

            // Visual Sync using RPC on the *Body* object
            DeadBodyVisual bodyVisual = bodyObj.GetComponent<DeadBodyVisual>();
            if (bodyVisual != null &&
                photonView.Owner.CustomProperties.TryGetValue("color", out object v))
            {
                int colorIndex = (int)v;
                bodyVisual.photonView.RPC("RPC_SetColor", RpcTarget.AllBuffered, colorIndex);
            }
        }

        // 2. Local visual updates (Ghost mode, disable movement) - Run for everyone
        if (ghost != null)
            ghost.EnterGhost();

        // 3. Only the owner updates their own custom properties
        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["IsDead"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    [PunRPC]
    public void RPC_Eject()
    {
        if (IsDead) return;
        IsDead = true;

        // Ejected players turn into ghosts immediately, but DO NOT spawn a dead body.
        if (ghost != null)
            ghost.EnterGhost();

        // Set Custom Property so other scenes know this player is dead
        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["IsDead"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        
        Debug.Log("Player EJECTED (Ghost Mode entered, No Body spawned)");
    }
}
