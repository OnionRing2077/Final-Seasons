using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class BodyReport : MonoBehaviourPun
{
    public float reportRange = 1.2f;
    public string meetingScene = "MeetingScene";
    public LayerMask targetLayers; // ✅ เลือก Layer ได้เอง

    public bool CanReport { get; private set; }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Check for reportables every frame (or optimized)
        CanReport = CheckIfBodyNearby();

        if (Input.GetKeyDown(KeyCode.R) && CanReport)
        {
            TryReport();
        }
    }

    private bool CheckIfBodyNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, reportRange, targetLayers);
        
        // Debug Log to see what we are hitting (remove later)
        if (hits.Length > 0) {
             // Uncomment if you want to see everything around:
             // Debug.Log($"BodyReport: Hitting {hits.Length} objects");
        }

        foreach (var hit in hits)
        {
            // Debug specific hit
            // Debug.Log("Checking collision: " + hit.gameObject.name);

            // Check for PlayerHealth (Legacy)
            PlayerHealth body = hit.GetComponent<PlayerHealth>();
            if (body != null && body.IsDead) 
            {
                Debug.Log("Found Dead PlayerHealth: " + hit.name);
                return true;
            }

            // Check for DeadBody (New)
            if (hit.GetComponent<DeadBody>()) 
            {
                Debug.Log("Found DeadBody Component: " + hit.name);
                return true;
            }
        }
        return false;
    }

    public void TryReport()
    {
        if (!CanReport) 
        {
            Debug.Log("TryReport Failed: CanReport is false");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, reportRange, targetLayers);

        foreach (var hit in hits)
        {
            bool foundBody = false;

            // Check legacy
            PlayerHealth body = hit.GetComponent<PlayerHealth>();
            if (body != null && body.IsDead) foundBody = true;

            // Check new
            if (hit.GetComponent<DeadBody>()) 
            {
                Debug.Log("Found DeadBody component: " + hit.name);
                foundBody = true;
            }

            if (foundBody)
            {
                Debug.Log("Reporting Body: " + hit.name);
                photonView.RPC("RPC_Report", RpcTarget.MasterClient);
                break;
            }
        }
    }

    [PunRPC]
    void RPC_Report(PhotonMessageInfo info)
    {
        Debug.Log("RPC_Report Received! Loading Scene: " + meetingScene);
        // Save who reported
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["ReporterActorNumber"] = info.Sender.ActorNumber;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        PhotonNetwork.LoadLevel(meetingScene);
    }
}
