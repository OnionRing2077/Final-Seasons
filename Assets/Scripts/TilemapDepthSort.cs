using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDepthSort : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    void LateUpdate()
    {
        // ถ้าอยากให้เรียงตามตำแหน่งของ Tilemap object เอง
        tilemapRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
