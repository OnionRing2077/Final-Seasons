using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class RoomTaskManager : MonoBehaviourPunCallbacks
{
    public static RoomTaskManager Instance;

    private struct TaskData
    {
        public int total;
        public int completed;
    }

    private Dictionary<int, TaskData> playerTasks = new Dictionary<int, TaskData>();

    void Awake()
    {
        Instance = this;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        playerTasks.Clear();
    }

    public void RegisterTasks(int total, int completedSoFar)
    {
        photonView.RPC(nameof(RPC_SetPlayerTasks), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, total, completedSoFar);
    }

    public void TaskCompleted()
    {
        if (playerTasks.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
        {
            TaskData data = playerTasks[PhotonNetwork.LocalPlayer.ActorNumber];
            data.completed++;
            // Update local immediately for responsiveness? OR just RPC. RPC is safer for sync.
            photonView.RPC(nameof(RPC_SetPlayerTasks), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, data.total, data.completed);
        }
    }

    [PunRPC]
    void RPC_SetPlayerTasks(int actorNumber, int total, int completed)
    {
        TaskData data = new TaskData { total = total, completed = completed };
        if (playerTasks.ContainsKey(actorNumber))
            playerTasks[actorNumber] = data;
        else
            playerTasks.Add(actorNumber, data);

        CheckTaskWin();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (playerTasks.ContainsKey(otherPlayer.ActorNumber))
        {
            playerTasks.Remove(otherPlayer.ActorNumber);
            CheckTaskWin();
        }
    }

    void CheckTaskWin()
    {
        int total = 0;
        int completed = 0;

        foreach (var kvp in playerTasks)
        {
            total += kvp.Value.total;
            completed += kvp.Value.completed;
        }

        Debug.Log($"ROOM TASK: {completed}/{total}");

        // Win Condition
        if (total > 0 && completed >= total)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                WinManager.Instance?.OnTasksCompleted();
            }
        }
    }

    public float GetProgress01()
    {
        int total = 0;
        int completed = 0;
        foreach (var kvp in playerTasks)
        {
            total += kvp.Value.total;
            completed += kvp.Value.completed;
        }

        if (total == 0) return 0;
        return (float)completed / total;
    }
}
