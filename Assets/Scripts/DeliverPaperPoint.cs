using UnityEngine;
using Photon.Pun;
using System.Collections;   // ✅ ต้องมี


public class DeliverPaperPoint : MonoBehaviour
{
    public enum PointType { Pickup, Drop }
    public PointType type;

    public string taskId;          // "DELIVER_DOC"
    public string displayName;     // "Deliver Document"
    public float interactRange = 1.5f;
    public GameObject indicator;

    GameObject localPlayer;
    PlayerTaskManager taskMgr;

    IEnumerator Start()
    {
        while (localPlayer == null)
        {
            foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
            {
                var pv = p.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    localPlayer = p;
                    taskMgr = p.GetComponent<PlayerTaskManager>();
                    break;
                }
            }
            yield return null;
        }

        UpdateIndicator();
    }

    void Update()
    {
        if (localPlayer == null || taskMgr == null) return;

        if (!taskMgr.HasActiveTask(taskId)) return;

        float dist = Vector2.Distance(
            localPlayer.transform.position,
            transform.position
        );

        if (dist > interactRange) return;

        if (type == PointType.Pickup && !taskMgr.hasDocument)
        {
            TaskPromptUI.Instance?.Show("Press E to pick up document");

            if (Input.GetKeyDown(KeyCode.E))
            {
                taskMgr.PickDocument(taskId);

                var drop = GameObject.Find("PaperDropPoint");
                if (drop != null)
                    taskMgr.ShowArrow(drop.transform);
            }
        }

        if (type == PointType.Drop && taskMgr.hasDocument)
        {
            TaskPromptUI.Instance?.Show("Press E to deliver document");

            if (Input.GetKeyDown(KeyCode.E))
            {
                taskMgr.DeliverDocument(taskId);
                taskMgr.HideArrow();
                UpdateIndicator();
            }
        }
    }

    void UpdateIndicator()
    {
        if (indicator == null || taskMgr == null) return;
        indicator.SetActive(taskMgr.HasActiveTask(taskId));
    }
}
