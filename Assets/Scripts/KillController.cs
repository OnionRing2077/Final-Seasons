using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KillController : MonoBehaviourPun
{
    public float killRange = 1.5f;

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryKill();
        }
    }

    void TryKill()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, killRange);

        foreach (var h in hits)
        {
            if (!h.CompareTag("Player")) continue;

            PhotonView pv = h.GetComponent<PhotonView>();
            if (pv == null || pv == photonView) continue;

            PlayerHealth hp = h.GetComponent<PlayerHealth>();
            if (hp == null || hp.IsDead) continue;

            // ส่งคำสั่งไป MasterClient
            photonView.RPC(nameof(RequestKill), RpcTarget.MasterClient, pv.ViewID);
            return;
        }
    }

    [PunRPC]
    void RequestKill(int targetViewID, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonView target = PhotonView.Find(targetViewID);
        if (target == null) return;

        target.RPC(nameof(DoKill), RpcTarget.All);
    }

    [PunRPC]
    void DoKill()
    {
        GetComponent<PlayerHealth>().Kill();
    }
}
