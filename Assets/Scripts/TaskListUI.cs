using TMPro;
using UnityEngine;

public class TaskListUI : MonoBehaviour
{
    public GameObject taskPanel;
    public TMP_Text taskText;

    PlayerTaskManager taskManager;

    void Update()
    {
        // 🔄 พยายามหา taskManager ถ้ายังไม่มี
        if (taskManager == null)
        {
            FindLocalTaskManager();
        }
    }

    void FindLocalTaskManager()
    {
        var allManagers = FindObjectsOfType<PlayerTaskManager>();
        foreach (var pm in allManagers)
        {
            if (pm.photonView.IsMine)
            {
                taskManager = pm;
                taskManager.OnTaskUpdate += Refresh; // ✅ Subscribe ทันทีที่เจอ
                Refresh(); // อัปเดตครั้งแรก
                break;
            }
        }
    }

    void OnDestroy()
    {
        if (taskManager != null)
            taskManager.OnTaskUpdate -= Refresh;
    }

    public void Toggle()
    {
        if (taskPanel == null || taskText == null)
        {
            Debug.LogError("TaskPanel / TaskText NOT ASSIGNED");
            return;
        }

        bool show = !taskPanel.activeSelf;
        taskPanel.SetActive(show);

        if (show)
            Refresh();
    }

    void Refresh()
    {
        if (taskManager == null)
        {
            // ยังหาไม่เจอ (อาจจะยังไม่ spawn)
            taskText.text = "Loading tasks...";
            return;
        }

        taskText.text = "";

        foreach (var t in taskManager.myTasks)
        {
            string line = (t.completed ? "<color=green>✔</color> " : "<color=red>□</color> ") + t.taskName;
            taskText.text += line + "\n";
        }

        if (taskManager.myTasks.Count == 0)
            taskText.text = "- No Tasks -";
            
    }
}
