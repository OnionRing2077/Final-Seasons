using UnityEngine;
using Photon.Pun;

public class CameraFollow2D : MonoBehaviour
{
    public float smoothSpeed = 10f;
    public Vector3 offset;

    private Transform target;

    void LateUpdate()
    {
        if (target == null)
        {
            FindLocalPlayer();
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10f; // กล้อง 2D ต้องอยู่ Z ลบ

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    void FindLocalPlayer()
    {
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = p.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                target = p.transform;
                Debug.Log("Camera locked to local player");
                break;
            }
        }
    }
}
