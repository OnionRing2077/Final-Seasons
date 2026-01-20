using Photon.Pun;
using UnityEngine;

public class MeetingManager : MonoBehaviourPun
{
    public static MeetingManager Instance;

    public GameObject meetingUI;

    void Awake()
    {
        Instance = this;
    }

    public void StartMeeting(int reporterActor, int deadActor)
    {
        Debug.Log($"MEETING START | Reporter={reporterActor} Dead={deadActor}");

        meetingUI.SetActive(true);

        // ⛔ ปิดระบบเล่น
        Time.timeScale = 0f;
    }

    public void EndMeeting()
    {
        meetingUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
