using Photon.Pun;
using UnityEngine;

public class RoomTaskManager : MonoBehaviourPun
{
    public static RoomTaskManager Instance;

    int totalCompleted = 0;
    int totalTasks = 0;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterTasks(int amount)
    {
        photonView.RPC(nameof(RPC_UpdateRoomTask), RpcTarget.AllBuffered, 0, amount);
    }

    public void TaskCompleted()
    {
        photonView.RPC(nameof(RPC_UpdateRoomTask), RpcTarget.AllBuffered, 1, 0);
    }

    [PunRPC]
    void RPC_UpdateRoomTask(int completed, int total)
    {
        totalCompleted += completed;
        totalTasks += total;

        Debug.Log($"ROOM TASK: {totalCompleted}/{totalTasks}");

        if (totalTasks > 0 && totalCompleted >= totalTasks)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                WinManager.Instance?.OnTasksCompleted();
            }
        }
    }

    public float GetProgress01()
    {
        if (totalTasks == 0) return 0;
        return (float)totalCompleted / totalTasks;
    }
}
