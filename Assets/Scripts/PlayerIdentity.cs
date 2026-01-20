using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using System;

public class PlayerIdentity : MonoBehaviourPunCallbacks
{
    public PlayerRole Role { get; private set; } = PlayerRole.Civilian;

    public event Action<PlayerRole> OnRoleChanged;

    void Start()
    {
        TrySyncRole();
    }

    public override void OnPlayerPropertiesUpdate(
        Photon.Realtime.Player target, Hashtable changedProps)
    {
        if (target == photonView.Owner && changedProps.ContainsKey("role"))
        {
            TrySyncRole();
        }
    }

    void TrySyncRole()
    {
        if (photonView.Owner.CustomProperties.TryGetValue("role", out object r))
        {
            Role = (PlayerRole)(int)r;
            Debug.Log($"[PlayerIdentity] ROLE SYNCED → {Role}");
            OnRoleChanged?.Invoke(Role);
        }
    }
}
