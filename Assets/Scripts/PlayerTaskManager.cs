using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerTaskManager : MonoBehaviourPun
{
    public List<PlayerTask> myTasks = new List<PlayerTask>();
    public int tasksPerPlayer = 2;

    public GameObject arrowPrefab;
    GameObject currentArrow;

    PlayerIdentity myId;
    public bool hasDocument;

    IEnumerator Start()
    {
        myId = GetComponent<PlayerIdentity>();

        // รอ role จาก Photon
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("role"))
            yield return null;

        AssignTasks();
    }

    void AssignTasks()
    {
        Debug.Log("TASK DB COUNT = " + TaskDatabase.allTasks.Count);
        myTasks.Clear();

        bool isImpostor = myId.Role == PlayerRole.Impostor;
        List<PlayerTask> pool = new List<PlayerTask>(TaskDatabase.allTasks);

        for (int i = 0; i < tasksPerPlayer && pool.Count > 0; i++)
        {
            int rand = Random.Range(0, pool.Count);

            myTasks.Add(new PlayerTask
            {
                taskName = pool[rand].taskName,
                completed = false,
                isFake = isImpostor
            });

            pool.RemoveAt(rand);
        }

        OnTaskUpdate?.Invoke();

        if (!isImpostor && RoomTaskManager.Instance != null)
            RoomTaskManager.Instance.RegisterTasks(myTasks.Count);
    }

    // ================= TASK LOGIC =================

    public bool HasActiveTask(string taskName)
    {
        return myTasks.Exists(t => t.taskName == taskName && !t.completed);
    }

    public void PickDocument(string taskName)
    {
        if (!HasActiveTask(taskName)) return;

        hasDocument = true;
        Debug.Log("Picked Document");
        OnTaskUpdate?.Invoke();
    }

    public void DeliverDocument(string taskName)
    {
        if (!HasActiveTask(taskName)) return;
        if (!hasDocument) return;

        hasDocument = false;
        CompleteTask(taskName);
    }

    public void CompleteTask(string taskName)
    {
        var t = myTasks.Find(x => x.taskName == taskName);
        if (t == null || t.completed) return;

        t.completed = true;
        OnTaskUpdate?.Invoke();

        // 🔊 Sound
        SFXManager.Instance?.PlayTask();

        if (!t.isFake)
            RoomTaskManager.Instance?.TaskCompleted();
    }

    // ================= ARROW =================

    public void ShowArrow(Transform target)
    {
        if (currentArrow != null)
            Destroy(currentArrow);

        currentArrow = Instantiate(
            arrowPrefab,
            transform.position + Vector3.up * 1.5f,
            Quaternion.identity,
            transform
        );

        currentArrow.GetComponent<TaskArrow>().target = target;
    }

    public void HideArrow()
    {
        if (currentArrow != null)
            Destroy(currentArrow);
    }

    public System.Action OnTaskUpdate;
}
