using UnityEngine;

public class CharacterColor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] renderers;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        foreach (var r in renderers)
        {
            if (r != null)
                r.color = color;
        }
    }
}
