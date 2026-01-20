using UnityEngine;
using Photon.Pun;

public class PlayerKill : MonoBehaviourPun
{
    [Header("Kill Settings")]
    public float killCooldown = 20f;
    public float killRange = 1.5f;
    public LayerMask playerLayer;

    float lastKillTime = -999f;

    PlayerHealth myHealth;
    GhostMode ghostMode;
    VisionRangeController vision;
    PlayerIdentity myId;

    void Awake()
    {
        myHealth = GetComponent<PlayerHealth>();
        ghostMode = GetComponent<GhostMode>();
        vision = GetComponent<VisionRangeController>();
        myId = GetComponent<PlayerIdentity>();
    }

    // =========================
// ⏱ Cooldown (สำหรับ UI)
// =========================
public float GetCooldownLeft()
{
    float left = (lastKillTime + killCooldown) - Time.time;
    return Mathf.Max(0f, left);
}

public float GetCooldown01()
{
    if (Time.time >= lastKillTime + killCooldown)
        return 1f;

    return 1f - (GetCooldownLeft() / killCooldown);
}

    // =========================
    // ✅ เช็คว่ากดปุ่มฆ่าได้ไหม
    // =========================
    public bool CanKill()
{
    if (!photonView.IsMine) return false;
    if (myHealth.IsDead) return false;
    if (ghostMode.IsGhost) return false;

    Debug.Log($"CanKill? role={myId.Role} mine={photonView.IsMine}");

    return myId.Role == PlayerRole.Impostor ||
           myId.Role == PlayerRole.Sheriff;
}

    // =========================
    // 🔪 กดฆ่า
    // =========================
    public void TryKill()
    {   
        Debug.Log($"[{photonView.Owner.NickName}] TRY KILL | Role={myId.Role}");

        if (!CanKill()) return;

        PlayerHealth target = FindKillTarget();
        if (target == null) return;

        PlayerIdentity targetId = target.GetComponent<PlayerIdentity>();
        if (targetId == null) return;

        lastKillTime = Time.time;

        // =====================
        // 🔴 IMPOSTOR
        // =====================
        if (myId.Role == PlayerRole.Impostor)
        {
            target.photonView.RPC(
                "RPC_Die",
                RpcTarget.All,
                photonView.Owner.ActorNumber
            );
            return;
        }

        // =====================
        // 🟡 SHERIFF
        // =====================
        if (myId.Role == PlayerRole.Sheriff)
        {
            // ยิง Impostor → Impostor ตาย
            if (targetId.Role == PlayerRole.Impostor)
            {
                target.photonView.RPC(
                    "RPC_Die",
                    RpcTarget.All,
                    photonView.Owner.ActorNumber
                );
                return;
            }

            // ยิง Madman → Madman ตายคนเดียว ✅
            if (targetId.Role == PlayerRole.Madman)
            {
                target.photonView.RPC(
                    "RPC_Die",
                    RpcTarget.All,
                    photonView.Owner.ActorNumber
                );
                return;
            }

            // ยิงคนดี / Sheriff → ตายคู่ (Sheriff พลาด)
            target.photonView.RPC(
                "RPC_Die",
                RpcTarget.All,
                photonView.Owner.ActorNumber
            );

            photonView.RPC(
                "RPC_Die",
                RpcTarget.All,
                photonView.Owner.ActorNumber
            );
        }
    }

    // =========================
    // 🔍 หาเป้าหมาย
    // =========================
    PlayerHealth FindKillTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            killRange,
            playerLayer
        );

        foreach (var hit in hits)
        {
            if (hit.GetComponentInParent<PlayerHealth>() == myHealth)
            continue;

            PlayerHealth health = hit.GetComponentInParent<PlayerHealth>();
            if (health == null || health.IsDead) continue;

            if (vision != null && !vision.CanSee(health.transform))
                continue;

            return health;
        }

        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
}
