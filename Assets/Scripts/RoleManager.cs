using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;

public class RoleManager : MonoBehaviourPun
{
    [Header("Optional Roles")]
    public bool allowSheriff = true;
    public bool allowMadman = true;

    public void AssignRoles()
    {   
        if (PhotonNetwork.PlayerList.Length < 2)
{
    Debug.LogError("Need at least 2 players");
    return;
}

        if (!PhotonNetwork.IsMasterClient) return;

        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        if (players.Count == 0) return;

        // =========================
        // 1) IMPOSTOR (ต้องมี)
        // =========================
        Player impostor = players[Random.Range(0, players.Count)];
        SetRole(impostor, PlayerRole.Impostor);
        players.Remove(impostor);

        // =========================
        // 2) SHERIFF (สุ่มมี / ไม่มี)
        // =========================
        if (allowSheriff && players.Count > 0 && Random.value < 0.5f)
        {
            Player sheriff = players[Random.Range(0, players.Count)];
            SetRole(sheriff, PlayerRole.Sheriff);
            players.Remove(sheriff);
        }

        // =========================
        // 3) MADMAN (สุ่มมี / ไม่มี)
        // =========================
        if (allowMadman && players.Count > 0 && Random.value < 0.5f)
        {
            Player madman = players[Random.Range(0, players.Count)];
            SetRole(madman, PlayerRole.Madman);
            players.Remove(madman);
        }
        if (!players.Any(p =>
            p.CustomProperties.ContainsKey("role") &&
            (PlayerRole)(int)p.CustomProperties["role"] == PlayerRole.Impostor))
        {
            Debug.LogError("NO IMPOSTOR ASSIGNED ❌");
        }

        // =========================
        // 4) ที่เหลือ = CIVILIAN
        // =========================
        foreach (var p in players)
        {
            SetRole(p, PlayerRole.Civilian);
        }

        Debug.Log("Roles assigned");
    }

    void SetRole(Player p, PlayerRole role)
{
    Hashtable props = new Hashtable();
    props["role"] = (int)role;
    p.SetCustomProperties(props);

    Debug.Log($"ROLE ASSIGNED → {p.NickName} = {role}");
}


}
