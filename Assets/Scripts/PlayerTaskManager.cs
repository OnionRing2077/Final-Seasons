using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerTaskManager : MonoBehaviourPun
{
    public List<PlayerTask> myTasks = new List<PlayerTask>();
    public int tasksPerPlayer = 2;

    PlayerIdentity myId;

    IEnumerator Start()
    {
        myId = GetComponent<PlayerIdentity>();

        // ⏳ รอ role มาก่อน (สำคัญกับ Photon)
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("role"))
            yield return null;

        AssignTasks();
    }

    void AssignTasks()
    {
        myTasks.Clear();

        bool isImpostor = myId.Role == PlayerRole.Impostor;

        List<PlayerTask> pool =
            new List<PlayerTask>(TaskDatabase.allTasks);

        for (int i = 0; i < tasksPerPlayer && pool.Count > 0; i++)
        {
            int rand = Random.Range(0, pool.Count);

            myTasks.Add(new PlayerTask
            {
                taskName = pool[rand].taskName,
                completed = false,
                isFake = isImpostor // 👈 Impostor = fake task
            });

            pool.RemoveAt(rand);
        }

        Debug.Log(isImpostor
            ? "FAKE TASKS ASSIGNED (IMPOSTOR)"
            : "REAL TASKS ASSIGNED");

        OnTaskUpdate?.Invoke();

        if (!isImpostor && RoomTaskManager.Instance != null)
        {
            RoomTaskManager.Instance.RegisterTasks(myTasks.Count);
        }
    }

    public void CompleteTask(string taskName)
    {
        PlayerTask t = myTasks.Find(x => x.taskName == taskName);
        if (t == null || t.completed) return;

        t.completed = true;

        if (t.isFake)
        {
            Debug.Log($"FAKE TASK COMPLETE: {taskName}");
            OnTaskUpdate?.Invoke(); // Update UI
            return; // ❌ ไม่อัปเดต progress ห้อง
        }

        Debug.Log($"REAL TASK COMPLETE: {taskName}");

        OnTaskUpdate?.Invoke(); // Update UI

        // ✅ เชื่อมต่อ RoomTaskManager
        RoomTaskManager.Instance?.TaskCompleted();
    }

    public System.Action OnTaskUpdate;
}
