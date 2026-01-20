using UnityEngine;
using Photon.Pun;

public class GhostMode : MonoBehaviourPun
{
    [Header("Layers")]
    public string aliveLayerName = "Player";
    public string ghostLayerName = "Ghost";

    [Header("Visual")]
    [Range(0f, 1f)] public float ghostAlpha = 0.45f;

    public bool IsGhost { get; private set; }

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer[] renderers;

    PlayerKill kill;
    BodyReport report;
    VisionController vision;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>(true);

        kill = GetComponent<PlayerKill>();
        report = GetComponent<BodyReport>();
        vision = GetComponent<VisionController>();
    }

    public void EnterGhost()
    {      
        var visibility = GetComponent<PlayerVisibilityController>();
        if (visibility) visibility.enabled = false;

        if (IsGhost) return;
        IsGhost = true;

        // 1) Disable systems that should not work when dead
        if (kill) kill.enabled = false;
        if (report) report.enabled = false;

        // 2) Make pass-through (ง่ายสุด: ปิด collider)
        if (col) col.enabled = false;

        // 3) Change layer -> Ghost (ซ่อนจากคนเป็นด้วย culling mask)
        SetLayerRecursively(gameObject, LayerMask.NameToLayer(ghostLayerName));

        // 4) Make semi-transparent
        foreach (var r in renderers)
        {
            if (!r) continue;
            var c = r.color;
            c.a = ghostAlpha;
            r.color = c;
        }

        // 5) Switch camera view for local player only
        if (photonView.IsMine && vision != null)
            vision.SetGhostView();
            
    }

    public void EnterAlive()
    {   
        var visibility = GetComponent<PlayerVisibilityController>();
        if (visibility) visibility.enabled = true;

        IsGhost = false;

        if (kill) kill.enabled = true;
        if (report) report.enabled = true;

        if (col) col.enabled = true;

        SetLayerRecursively(gameObject, LayerMask.NameToLayer(aliveLayerName));

        foreach (var r in renderers)
        {
            if (!r) continue;
            var c = r.color;
            c.a = 1f;
            r.color = c;
        }

        if (photonView.IsMine && vision != null)
            vision.SetAliveView();
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (layer < 0) return;
        obj.layer = layer;

        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }
}
