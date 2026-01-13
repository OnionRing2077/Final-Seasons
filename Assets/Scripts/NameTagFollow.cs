using TMPro;
using UnityEngine;
using Photon.Pun;

public class NameTagFollow : MonoBehaviourPun
{
    public Vector3 offset = new Vector3(0, 1.4f, 0);

    private Transform target;
    private TextMeshProUGUI nameText;

    void Awake()
    {
        target = transform.parent;
        nameText = GetComponentInChildren<TextMeshProUGUI>();

        if (nameText == null)
        {
            Debug.LogError("NameTagFollow: No TextMeshProUGUI found in children!");
            return;
        }
    }

    public void SetName(string name)
    {
        if (nameText != null)
            nameText.text = name;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;

        // กัน name กลับด้าน
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
