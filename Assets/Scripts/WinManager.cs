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

    // ==============================
    // เรียกหลังมีคนตาย / โหวต
    // ==============================
    public void CheckWinConditions()
    {
        int impostorAlive = 0;
        int goodAlive = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.TryGetValue("dead", out object d) ||
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
        // 👉 TODO: เปิด Win Screen UI
        // SceneManager.LoadScene("WinScene");
    }
}
