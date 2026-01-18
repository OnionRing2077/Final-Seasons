using UnityEngine;
using Photon.Pun;

public class BodyReport : MonoBehaviourPun
{
    public float reportRange = 1.2f;
    public string meetingScene = "MeetingScene";

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReport();
        }
    }

    void TryReport()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, reportRange);

        foreach (var hit in hits)
        {
            PlayerHealth body = hit.GetComponent<PlayerHealth>();
            if (body != null && body.IsDead)
            {
                photonView.RPC("RPC_Report", RpcTarget.MasterClient);
                break;
            }
        }
    }

    [PunRPC]
    void RPC_Report()
    {
        PhotonNetwork.LoadLevel(meetingScene);
    }
}
