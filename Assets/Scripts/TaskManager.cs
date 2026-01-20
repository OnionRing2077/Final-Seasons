using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TaskManager : MonoBehaviourPun
{
    public static TaskManager Instance;

    public int totalTasks = 10;
    int completedTasks = 0;

    void Awake()
    {
        Instance = this;
    }

    // ==============================
    // เรียกเมื่อผู้เล่นทำ task เสร็จ
    // ==============================
    public void OnTaskCompleted(Player player)
    {
        PlayerRole role = (PlayerRole)(int)player.CustomProperties["role"];

        // ❌ คนร้าย + Madman ไม่ดันหลอด
        if (role == PlayerRole.Impostor || role == PlayerRole.Madman)
            return;

        photonView.RPC(nameof(RPC_AddTaskProgress), RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_AddTaskProgress()
    {
        completedTasks++;
        UpdateTaskBar();

        if (completedTasks >= totalTasks)
        {
            WinManager.Instance.OnTasksCompleted();
        }
    }

    void UpdateTaskBar()
    {
        Debug.Log($"Task Progress: {completedTasks}/{totalTasks}");
        // 👉 TODO: update UI task bar
    }
}
