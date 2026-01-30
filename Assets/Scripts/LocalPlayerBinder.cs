using UnityEngine;
using Photon.Pun;

public class LocalPlayerBinder : MonoBehaviourPun
{
    void Start()
    {
        if (!photonView.IsMine) return;

        // ✅ Important: Bind TagObject so we can find this player from PhotonPlayer.TagObject
        PhotonNetwork.LocalPlayer.TagObject = gameObject;


        // 🔥 Bind Vision Mask ให้ Local Player
        VisionMaskController mask = FindObjectOfType<VisionMaskController>();
        if (mask != null)
        {
            mask.player = transform;
            Debug.Log("VisionMask bound to local player");
        }

        BindKillUI();
        ConfigureVisionByRole();
    }

    void BindKillUI()
    {
        KillButtonUI ui = FindObjectOfType<KillButtonUI>();
        if (ui != null)
        {
            ui.playerKill = GetComponent<PlayerKill>();
            Debug.Log("Local PlayerKill bound to UI");
        }
    }

    void ConfigureVisionByRole()
    {
        VisionRangeController vision = GetComponent<VisionRangeController>();
        if (vision == null) return;

        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("role", out object v))
            return;

        PlayerRole role = (PlayerRole)(int)v;

        switch (role)
        {
            case PlayerRole.Impostor:
                vision.SetVisionRadius(7f);
                break;

            case PlayerRole.Sheriff:
                vision.SetVisionRadius(5.5f);
                break;

            case PlayerRole.Madman:
                vision.SetVisionRadius(6f);
                break;

            default: // Civilian
                vision.SetVisionRadius(5f);
                break;
        }
    }
}
