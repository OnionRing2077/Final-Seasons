using UnityEngine;
using Photon.Pun;

public class PlayerVisibilityController : MonoBehaviourPun
{
    SpriteRenderer[] renderers;
    Transform localPlayer;
    VisionRangeController localVision;

    void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();

        if (photonView.IsMine)
            return;

        GameObject lp = GameObject.FindGameObjectWithTag("Player");
        if (lp != null)
        {
            localPlayer = lp.transform;
            localVision = lp.GetComponent<VisionRangeController>();
        }
    }

    void Update()
    {
        if (localPlayer == null || localVision == null) return;

        bool canSee = localVision.CanSee(transform);
        SetVisible(canSee);
    }

    void SetVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;
    }
}
