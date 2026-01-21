using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class ReportButtonUI : MonoBehaviour
{
    [Header("UI")]
    public Button reportButton;

    [Header("Refs")]
    public BodyReport localBodyReport;

    void Awake()
    {
        if (reportButton == null)
            reportButton = GetComponent<Button>();
    }

    void Start()
    {
        FindLocalBodyReport();

        if (reportButton != null)
            reportButton.onClick.AddListener(OnReportPressed);
    }

    void FindLocalBodyReport()
    {
        var allReports = FindObjectsOfType<BodyReport>();
        foreach (var report in allReports)
        {
            if (report.photonView.IsMine)
            {
                localBodyReport = report;
                break;
            }
        }
    }

    void Update()
    {
        // try finding again if null (player might spawn late)
        if (localBodyReport == null) 
        {
            FindLocalBodyReport();
            if (localBodyReport == null)
            {
                if(reportButton) reportButton.interactable = false;
                return;
            }
        }

        // Update Button State
        if (reportButton)
        {
            bool canReport = localBodyReport.CanReport;
            if (reportButton.interactable != canReport)
            {
                // Debug log status change only
                // Debug.Log($"Report Button State Changed: {canReport}");
                reportButton.interactable = canReport;
            }
        }
    }

    void OnReportPressed()
    {
        Debug.Log("Report Button Pressed");
        
        // 🔊 Sound Feedback Immediately
        SFXManager.Instance?.PlayVote();

        if (localBodyReport != null)
        {
            localBodyReport.TryReport();
        }
    }
}
