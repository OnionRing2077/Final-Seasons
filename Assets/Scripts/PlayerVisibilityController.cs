using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerVisibilityController : MonoBehaviourPun
{
    SpriteRenderer[] spriteRenderers;
    TMP_Text[] nameTexts;
    Canvas[] canvases;

    VisionRangeController vision;
    PlayerHealth myHealth;
    GhostMode myGhost;

    void Awake()
    {
        // 🔹 ตัวละคร
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // 🔹 NameTag (TMP)
        nameTexts = GetComponentsInChildren<TMP_Text>(true);

        // 🔹 World Space Canvas (NameTag ใช้ตัวนี้)
        canvases = GetComponentsInChildren<Canvas>(true);

        vision = GetComponent<VisionRangeController>();
        myHealth = GetComponent<PlayerHealth>();
        myGhost = GetComponent<GhostMode>();
    }

    void LateUpdate()
    {
        // 🔸 ตัวเราเองไม่ต้องซ่อน
        if (photonView.IsMine) return;

        PlayerVisibilityController local = FindLocalPlayer();
        if (local == null) return;

        // 👻 ถ้า local player เป็นผี → เห็นทุกอย่าง
        if (local.myGhost != null && local.myGhost.IsGhost)
        {
            SetVisible(true);
            return;
        }

        // 💀 ถ้า object นี้เป็นศพ → ใช้ vision เช็ค
        if (myHealth != null && myHealth.IsDead)
        {
            bool canSeeBody =
                local.vision != null &&
                local.vision.CanSee(transform);

            SetVisible(canSeeBody);
            return;
        }

        // 🙂 Player ปกติ
        bool canSeePlayer =
            local.vision != null &&
            local.vision.CanSee(transform);

        SetVisible(canSeePlayer);
    }

    void SetVisible(bool visible)
    {
        // 🔹 ตัวละคร
        foreach (var r in spriteRenderers)
            if (r) r.enabled = visible;

        // 🔹 NameTag (TextMeshPro)
        foreach (var t in nameTexts)
            if (t) t.enabled = visible;

        // 🔹 Canvas (กันกรณี TMP ยังขึ้น)
        foreach (var c in canvases)
            if (c) c.enabled = visible;
    }

    PlayerVisibilityController FindLocalPlayer()
    {
        foreach (var p in FindObjectsOfType<PlayerVisibilityController>())
        {
            if (p.photonView.IsMine)
                return p;
        }
        return null;
    }
}
