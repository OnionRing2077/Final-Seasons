using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class WinManager : MonoBehaviourPun
{
    public static WinManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Check immediately when scene loads (e.g. returning from Meeting)
        if (PhotonNetwork.IsMasterClient)
        {
            CheckWinConditions();
        }
    }

    // ==============================
    // เรียกหลังมีคนตาย / โหวต
    // ==============================
    public void CheckWinConditions()
    {
        int impostorAlive = 0;
        int goodAlive = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.TryGetValue("IsDead", out object d) ||
                (bool)d == true)
                continue;

            PlayerRole role = (PlayerRole)(int)p.CustomProperties["role"];

            if (role == PlayerRole.Impostor)
                impostorAlive++;
            else
                goodAlive++;
        }

        // 🔴 Impostor ชนะ
        if (impostorAlive >= goodAlive && impostorAlive > 0)
        {
            EndGame(GameResult.ImpostorWin);
            return;
        }

        // 🟢 Civilian ชนะ (ถ้าไม่มี Impostor เหลือแล้ว)
        if (impostorAlive == 0)
        {
            EndGame(GameResult.CivilianWin);
            return;
        }
    }

    // ==============================
    // เรียกจาก VoteManager
    // ==============================
    public void OnPlayerVotedOut(Player votedPlayer)
    {
        PlayerRole role = (PlayerRole)(int)votedPlayer.CustomProperties["role"];

        // 🟣 Madman ชนะ เฉพาะโดนโหวต
        if (role == PlayerRole.Madman)
        {
            EndGame(GameResult.MadmanWin);
            return;
        }

        CheckWinConditions();
    }

    // ==============================
    // คนดีชนะจาก Task
    // ==============================
    public void OnTasksCompleted()
    {
        EndGame(GameResult.CivilianWin);
    }

    // ==============================
    void EndGame(GameResult result)
    {
        photonView.RPC(nameof(RPC_EndGame), RpcTarget.All, result);
    }

    [PunRPC]
    void RPC_EndGame(GameResult result)
    {
        Debug.Log("GAME OVER: " + result);
        
        if (WinScreenUI.Instance != null)
        {
            WinScreenUI.Instance.ShowWin(result);
        }
        else
        {
            // Fallback if no UI in scene (create one dynamically or just log)
            Debug.LogError("No WinScreenUI found! Creating temporary...");
            GameObject go = new GameObject("WinScreenUI");
            go.AddComponent<WinScreenUI>().ShowWin(result);
        }
    }
}
