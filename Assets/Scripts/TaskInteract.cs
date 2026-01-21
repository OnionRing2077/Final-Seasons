using UnityEngine;

public class TaskInteract : MonoBehaviour
{
    public string taskId;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerTaskManager tm = other.GetComponent<PlayerTaskManager>();
        if (tm != null)
        {
            tm.CompleteTask(taskId);
            gameObject.SetActive(false); // ภารกิจหาย
        }
    }
}
