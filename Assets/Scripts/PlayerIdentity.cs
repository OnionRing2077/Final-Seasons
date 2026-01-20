using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerIdentity : MonoBehaviourPunCallbacks
{
    public PlayerRole Role { get; private set; } = PlayerRole.Civilian;

    public override void OnPlayerPropertiesUpdate(
        Photon.Realtime.Player target,
        Hashtable changedProps)
    {
        if (target == photonView.Owner && changedProps.ContainsKey("role"))
        {
            ApplyRole(changedProps["role"]);
        }
    }

    void ApplyRole(object r)
    {
        Role = (PlayerRole)(int)r;
        Debug.Log($"[PlayerIdentity] ROLE SYNCED → {Role}");
    }
}

