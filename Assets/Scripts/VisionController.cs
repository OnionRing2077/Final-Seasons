using UnityEngine;
using Photon.Pun;

public class VisionController : MonoBehaviourPun
{
    Camera cam;

    void Start()
    {
        if (!photonView.IsMine) return;

        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("VisionController: Camera.main not found");
            return;
        }

        SetAliveView();
    }

    public void SetAliveView()
    {
        if (!photonView.IsMine || cam == null) return;

        cam.cullingMask = LayerMask.GetMask(
            "Default",
            "Ground",        // ✅ เพิ่ม
            "VisionBlock",
            "Player",
            "DeadBody",
            "UI"
        );

        Debug.Log("Alive view enabled");
    }

    public void SetGhostView()
    {
        if (!photonView.IsMine || cam == null) return;

        cam.cullingMask = LayerMask.GetMask(
            "Default",
            "Ground",        // ✅ เพิ่ม
            "VisionBlock",
            "Player",   // ✅ ผีต้องเห็นคนเป็น
            "Ghost",    // ✅ และเห็นผีด้วย
            "DeadBody",
            "UI"
        );

        Debug.Log("Ghost view enabled");
    }
}
