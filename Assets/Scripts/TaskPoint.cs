using UnityEngine;
using Photon.Pun;

public class TaskPoint : MonoBehaviour
{
    public string taskName;
    public float interactRange = 1.5f;
    public GameObject indicator;

    GameObject localPlayer;
    PlayerTaskManager taskManager;

    void Start()
    {
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            var pv = p.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                localPlayer = p;
                taskManager = p.GetComponent<PlayerTaskManager>();
                break;
            }
        }

        UpdateIndicator();
    }

    void Update()
    {
        if (localPlayer == null || taskManager == null) return;

        float dist = Vector2.Distance(
            localPlayer.transform.position,
            transform.position
        );

        bool hasTask = taskManager.myTasks.Exists(
            t => t.taskName == taskName && !t.completed
        );

        if (dist <= interactRange && hasTask)
        {
            TaskPromptUI.Instance?.Show($"Press E to {taskName}");

            if (Input.GetKeyDown(KeyCode.E))
            {
                taskManager.CompleteTask(taskName);
                TaskPromptUI.Instance?.Hide();
                UpdateIndicator();
            }
        }
        else
        {
            TaskPromptUI.Instance?.Hide();
        }
    }

    void UpdateIndicator()
    {
        if (indicator == null || taskManager == null) return;

        bool hasTask = taskManager.myTasks.Exists(
            t => t.taskName == taskName && !t.completed
        );

        indicator.SetActive(hasTask);
    }
}
