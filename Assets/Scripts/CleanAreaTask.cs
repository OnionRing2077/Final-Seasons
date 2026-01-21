using UnityEngine;

public class CleanAreaTask : MonoBehaviour
{
    public string taskName = "Clean Area";
    public float holdTime = 2f;
    float hold;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKey(KeyCode.E))
        {
            hold += Time.deltaTime;
            if (hold >= holdTime)
            {
                PlayerTaskManager tm = other.GetComponent<PlayerTaskManager>();
                if (tm != null)
                    tm.CompleteTask(taskName);

                gameObject.SetActive(false);
                hold = 0;
            }
        }
        else hold = 0;
    }
}
