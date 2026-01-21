using UnityEngine;

public class DeliverPaperPoint : MonoBehaviour
{
    public string taskName = "Deliver Paper";

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        PlayerTaskManager tm = col.GetComponent<PlayerTaskManager>();
        if (tm == null) return;

        tm.CompleteTask(taskName);
        Debug.Log("Deliver Paper Task Complete");
    }
}
