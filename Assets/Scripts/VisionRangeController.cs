using UnityEngine;
using Photon.Pun;

public class VisionRangeController : MonoBehaviourPun
{
    [Header("Vision")]
    public float visionRadius = 5f;
    public LayerMask obstacleMask;

    [Header("Refs")]
    public Transform visionCenter;

    void Awake()
    {
        if (!visionCenter)
            visionCenter = transform;
    }

    // ===== ใช้ให้ Kill / Vision เช็ค =====
    public bool CanSee(Transform target)
    {
        if (!photonView.IsMine) return false;

        // ✅ ประกาศตัวแปรก่อนใช้
        Vector2 dir = target.position - visionCenter.position;
        float dist = dir.magnitude;

        if (dist > visionRadius)
            return false;

        // ✅ ยืนติดกัน = เห็นแน่นอน (กัน Raycast พลาด)
        if (dist < 0.2f)
            return true;

        // ✅ Raycast เช็คกำแพง
        RaycastHit2D hit = Physics2D.Raycast(
            visionCenter.position,
            dir.normalized,
            dist,
            obstacleMask
        );

        // Debug ช่วยดู
        Debug.DrawRay(
            visionCenter.position,
            dir.normalized * dist,
            hit.collider ? Color.red : Color.green
        );

        return hit.collider == null;
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
