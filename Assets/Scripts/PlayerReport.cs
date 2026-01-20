using UnityEngine;
using Photon.Pun;

public class PlayerReport : MonoBehaviourPun
{
    [Header("Report Settings")]
    public float reportRange = 2f;
    public LayerMask deadBodyLayer;

    PlayerHealth myHealth;
    GhostMode ghostMode;

    void Awake()
    {
        myHealth = GetComponent<PlayerHealth>();
        ghostMode = GetComponent<GhostMode>();
    }

    // =========================
    // 📢 กด Report
    // =========================
    public void TryReport()
    {
        if (!photonView.IsMine) return;
        if (myHealth.IsDead) return;
        if (ghostMode.IsGhost) return;

        DeadBody body = FindDeadBody();
        if (body == null) return;

        photonView.RPC(
            "RPC_Report",
            RpcTarget.All,
            body.photonView.ViewID,
            photonView.Owner.ActorNumber
        );
    }

    // =========================
    // 🔍 หา Dead Body
    // =========================
    DeadBody FindDeadBody()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            reportRange,
            deadBodyLayer
        );

        foreach (var hit in hits)
        {
            DeadBody body = hit.GetComponent<DeadBody>();
            if (body != null && !body.isReported)
                return body;
        }
        return null;
    }

    [PunRPC]
    void RPC_Report(int bodyViewID, int reporterActor)
    {
        PhotonView bodyView = PhotonView.Find(bodyViewID);
        if (bodyView == null) return;

        DeadBody body = bodyView.GetComponent<DeadBody>();
        if (body == null || body.isReported) return;

        body.isReported = true;

        Debug.Log($"REPORT by Actor {reporterActor}");

        // ⏸ เข้าประชุม
        MeetingManager.Instance.StartMeeting(reporterActor, body.ownerActor);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, reportRange);
    }
}
