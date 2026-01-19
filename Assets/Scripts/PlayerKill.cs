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


    void Awake()
    {
        myHealth = GetComponent<PlayerHealth>();
        ghostMode = GetComponent<GhostMode>();
        vision = GetComponent<VisionRangeController>();
    }

    public bool CanKill()
    {
        if (!photonView.IsMine) return false;

        // ✅ คนตาย/เป็นผี ฆ่าไม่ได้
        if (myHealth != null && myHealth.IsDead) return false;
        if (ghostMode != null && ghostMode.IsGhost) return false;

        if (Time.time < lastKillTime + killCooldown) return false;

        return FindKillTarget() != null;
    }

    public void TryKill()
    {
        if (!photonView.IsMine) return;

        if (myHealth != null && myHealth.IsDead) return;
        if (ghostMode != null && ghostMode.IsGhost) return;

        PlayerHealth target = FindKillTarget();
        if (target == null) return;

        lastKillTime = Time.time;

        target.photonView.RPC(
            "RPC_Die",
            RpcTarget.All,
            PhotonNetwork.LocalPlayer.ActorNumber
        );
    }

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

    PlayerHealth FindKillTarget()
{
    Collider2D[] hits = Physics2D.OverlapCircleAll(
        transform.position,
        killRange,
        playerLayer
    );

    foreach (var hit in hits)
    {
        if (hit.gameObject == gameObject) continue;

        PlayerHealth health = hit.GetComponentInParent<PlayerHealth>();
        if (health == null || health.IsDead) continue;

        // ✅ เช็ค Vision (ไม่ทะลุกำแพง)
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
