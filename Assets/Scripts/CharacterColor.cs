using UnityEngine;

public class CharacterColor : MonoBehaviour
{
    private SpriteRenderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        foreach (var r in renderers)
        {
            r.color = color;
        }
    }
}
