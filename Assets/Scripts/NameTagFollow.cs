using TMPro;
using UnityEngine;
using Photon.Pun;

public class NameTagFollow : MonoBehaviourPun
{
    public Vector3 offset = new Vector3(0, 1.4f, 0);

    private Transform target;
    private TMP_Text nameText;

    void Awake()
    {
        target = transform.parent;
        nameText = GetComponentInChildren<TMP_Text>();

        if (nameText == null)
        {
            Debug.LogError("NameTagFollow: No TMP_Text found!");
            return;
        }
    }

    void Start()
    {
        // ✅ ตั้งชื่อทันทีตอนเริ่ม
        if (photonView != null && photonView.Owner != null)
        {
            nameText.text = photonView.Owner.NickName;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;

        // กันข้อความกลับด้าน
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
