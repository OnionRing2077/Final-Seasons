using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerKill : MonoBehaviourPun
{   
    [Header("UI")]
    public Button killButton;
    public TMP_Text cooldownText;

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

    // ⏱️ เช็คคูลดาวน์
    if (Time.time < lastKillTime + killCooldown)
        return false;

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
    
    void Update()
{
    if (!photonView.IsMine) return;

    float cdLeft = GetCooldownLeft();

    // ⏱️ กำลัง cooldown
    if (cdLeft > 0f)
    {
        if (killButton) killButton.interactable = false;

        if (cooldownText)
        {
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = Mathf.CeilToInt(cdLeft).ToString();
        }
    }
    // ✅ ฆ่าได้แล้ว
    else
    {
        if (killButton) killButton.interactable = CanKill();

        if (cooldownText)
        {
            cooldownText.text = "";
            cooldownText.gameObject.SetActive(false);
        }
    }
}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
}
