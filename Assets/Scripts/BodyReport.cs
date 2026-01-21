using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class BodyReport : MonoBehaviourPun
{
    public float reportRange = 1.2f;
    public string meetingScene = "MeetingScene";
    public LayerMask targetLayers; // ✅ เลือก Layer ได้เอง

    public bool CanReport { get; private set; }

    PlayerHealth myHealth;
    GhostMode ghostMode;

    void Awake()
    {
        myHealth = GetComponent<PlayerHealth>();
        ghostMode = GetComponent<GhostMode>();
    }

    void Start()
    {
        if (targetLayers.value == 0)
            targetLayers = Physics2D.AllLayers;
    }

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
        if (myHealth != null && myHealth.IsDead) return false;
        if (ghostMode != null && ghostMode.IsGhost) return false;

        // Use AllLayers to debug what we are actually hitting
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, reportRange);
        
        // Debug.Log($"Checking Nearby... Hits: {hits.Length}");

        foreach (var hit in hits)
        {
            // Debug.Log($"Hit: {hit.name} | Layer: {LayerMask.LayerToName(hit.gameObject.layer)}");

            // Check for PlayerHealth (Legacy)
            PlayerHealth body = hit.GetComponent<PlayerHealth>();
            if (body != null && body.IsDead) 
            {
                // Debug.Log("FOUND DEAD PLAYER (Health)");
                return true;
            }

            // Check for DeadBody (New)
            if (hit.GetComponent<DeadBody>()) 
            {
                // Debug.Log("FOUND DEAD BODY (Script)");
                return true;
            }
        }
        return false;
    }

    public void TryReport()
    {
        if (!CanReport) return;

        if (myHealth != null && myHealth.IsDead) return;
        if (ghostMode != null && ghostMode.IsGhost) return;
        
        // 🔊 Sound
        SFXManager.Instance?.PlayVote();

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
                photonView.RPC("RPC_TriggerMeeting", RpcTarget.MasterClient);
                break;
            }
        }
    }

    [PunRPC]
    void RPC_TriggerMeeting(PhotonMessageInfo info)
    {
        Debug.Log("RPC_TriggerMeeting Received! Loading Scene: " + meetingScene);
        // Save who reported
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["ReporterActorNumber"] = info.Sender.ActorNumber;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        PhotonNetwork.LoadLevel(meetingScene);
    }
}
