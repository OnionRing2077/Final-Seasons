using UnityEngine;
using Spriter2UnityDX;   // 🔥 สำคัญมาก

public class DepthSortByY : MonoBehaviour
{
    SpriteRenderer sprite;
    EntityRenderer spriter;

    void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        spriter = GetComponentInChildren<EntityRenderer>();
    }

    void LateUpdate()
    {
        int order = Mathf.RoundToInt(-transform.position.y * 100);

        if (sprite != null)
            sprite.sortingOrder = order;

        if (spriter != null)
            spriter.SortingOrder = order;   // 🔥 Spriter depth
    }
}
