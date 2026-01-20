using UnityEngine;
using Photon.Pun;

public class DeadBodyVisual : MonoBehaviourPun
{
    SpriteRenderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    [PunRPC]
    public void RPC_SetColor(int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= PlayerColors.Colors.Length)
            return;

        Color color = PlayerColors.Colors[colorIndex];

        foreach (var r in renderers)
        {
            if (!r) continue;

            Color c = color;
            c.a = r.color.a; // รักษา alpha เดิม
            r.color = c;
        }
    }
}
