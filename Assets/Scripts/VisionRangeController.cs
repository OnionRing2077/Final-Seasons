using UnityEngine;
using Photon.Pun;

public class VisionRangeController : MonoBehaviourPun
{
    [Header("Vision")]
    public float visionRadius = 5f;
    public LayerMask obstacleMask;   // VisionBlock

    [Header("Refs")]
    public Transform visionCenter;

    void Awake()
    {
        if (!visionCenter)
            visionCenter = transform;
    }

    /// <summary>
    /// ใช้ถามว่า "เห็น target ไหม"
    /// </summary>
    public bool CanSee(Transform target)
    {
        if (!photonView.IsMine) return false;

        Vector2 dir = target.position - visionCenter.position;
        float dist = dir.magnitude;

        if (dist > visionRadius)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            visionCenter.position,
            dir.normalized,
            dist,
            obstacleMask
        );

        // โดนกำแพงก่อน
        if (hit.collider != null)
            return false;

        return true;
    }
    // VisionRangeController.cs
public void ConfigureVision(
    float radius,
    LayerMask target,
    LayerMask obstacle
)
{
    visionRadius = radius;
    targetMask = target;
    obstacleMask = obstacle;
}


    // ===== API =====
    public void SetVisionRadius(float radius)
    {
        visionRadius = radius;
    }

    public void DisableVisionLimit()
    {
        visionRadius = 999f;
        obstacleMask = 0;
    }
}
