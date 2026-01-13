using UnityEngine;

public class NameTagFix : MonoBehaviour
{
    void LateUpdate()
    {
        // ล็อค Scale โลก (world) ให้เป็นบวกเสมอ
        Vector3 s = transform.lossyScale;
        if (s.x < 0)
        {
            Vector3 local = transform.localScale;
            local.x *= -1;
            transform.localScale = local;
        }
    }
}
