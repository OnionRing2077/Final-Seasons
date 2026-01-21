using UnityEngine;
using Photon.Pun;

public class DocumentDeliverPoint : MonoBehaviour
{
    public string taskId = "deliver_document";
    public float interactRange = 1.5f;

    void Update()
    {
        var player = FindLocalPlayer();
        if (!player) return;

        float dist = Vector2.Distance(player.transform.position, transform.position);
        if (dist <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            var mgr = player.GetComponent<PlayerTaskManager>();
            if (mgr != null)
            {
                mgr.DeliverDocument(taskId); // ✅ ส่งแล้วค่อย complete
            }
        }
    }

    GameObject FindLocalPlayer()
    {
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            var pv = p.GetComponent<PhotonView>();
            if (pv && pv.IsMine) return p;
        }
        return null;
    }
}
