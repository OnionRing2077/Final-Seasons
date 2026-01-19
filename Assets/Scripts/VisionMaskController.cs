using UnityEngine;

public class VisionMaskController : MonoBehaviour
{
    public Transform player;
    public float radius = 150f;     // pixel
    public float softness = 30f;

    Material mat;
    Camera cam;
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        mat = GetComponent<UnityEngine.UI.Image>().material;
        cam = Camera.main;
    }

    void Update()
    {
        if (!player || !cam) return;

        // 1️⃣ world → screen (pixel)
        Vector2 screenPos = cam.WorldToScreenPoint(player.position);

        // 2️⃣ ส่งค่าเข้า shader
        mat.SetVector("_Center", new Vector4(screenPos.x, screenPos.y, 0, 0));
        mat.SetFloat("_Radius", radius);
        mat.SetFloat("_Softness", softness);
    }
}
