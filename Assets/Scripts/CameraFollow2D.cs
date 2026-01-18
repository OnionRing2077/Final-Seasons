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
            FindLocalTarget();
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10f;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    void FindLocalTarget()
    {
        // 1️⃣ หา Player local ก่อน
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = p.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                target = p.transform;
                Debug.Log("📷 Camera locked to PLAYER");
                return;
            }
        }

        // 2️⃣ ถ้าไม่มี Player → หา Ghost local
        foreach (var g in GameObject.FindGameObjectsWithTag("Ghost"))
        {
            PhotonView pv = g.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                target = g.transform;
                Debug.Log("👻 Camera locked to GHOST");
                return;
            }
        }
    }
    void FindLocalPlayer()
{
    foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
    {
        PhotonView pv = p.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            target = p.transform;
            Debug.Log("Camera locked to PLAYER");
            return;
        }
    }

    foreach (var g in GameObject.FindGameObjectsWithTag("Ghost"))
    {
        PhotonView pv = g.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            target = g.transform;
            Debug.Log("Camera locked to GHOST");
            return;
        }
    }
}
public void SetTarget(Transform newTarget)
{
    target = newTarget;
    Debug.Log("Camera locked to GHOST");
}

}
