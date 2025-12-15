using UnityEngine;

public class CameraFixedFollow : MonoBehaviour
{
    public Transform target;      // ตัวละครที่จะติดตาม
    public Vector3 offset = new Vector3(0, 5, -10); // ระยะห่างคงที่
    public float smoothSpeed = 0.1f;  // ความนุ่มนวลของกล้อง

    private Vector3 fixedPosition; // เก็บตำแหน่งกล้องเป้าหมาย

    void Start()
    {
        // ตั้งตำแหน่งกล้องเริ่มต้นจาก offset
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // คำนวณตำแหน่งที่ต้องการให้กล้องอยู่ (คงที่)
        fixedPosition = target.position + offset;

        // เคลื่อนกล้องแบบนุ่มนวลโดยไม่เปลี่ยน offset
        transform.position = Vector3.Lerp(transform.position, fixedPosition, smoothSpeed);
    }
}
