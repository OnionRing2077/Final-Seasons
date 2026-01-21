using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskItemUI : MonoBehaviour
{
    public TMP_Text iconText;
    public TMP_Text taskNameText;
    public Button button;

    string taskId;

    public void Setup(string id, string name, bool completed, bool isFake)
    {
        taskId = id;

        taskNameText.text = name;

        if (isFake)
        {
            iconText.text = "❌";
            iconText.color = Color.red;
            taskNameText.color = Color.gray;
        }
        else if (completed)
        {
            iconText.text = "✔";
            iconText.color = Color.green;
            taskNameText.color = Color.gray;
        }
        else
        {
            iconText.text = "□";
            iconText.color = Color.white;
            taskNameText.color = Color.white;
        }
    }

    public void OnClick()
    {
        Debug.Log($"Clicked task: {taskId}");
        // 👉 เปิด Mini-game / ชี้แผนที่ / Highlight
    }
}
