using UnityEngine;
using Photon.Pun;

public class LocalPlayerBinder : MonoBehaviourPun
{
    void Start()
    {
        if (!photonView.IsMine) return;

        BindKillUI();
        ConfigureVisionByRole();
    }

    // =======================
    // Bind Kill Button
    // =======================
    void BindKillUI()
    {
        KillButtonUI ui = FindObjectOfType<KillButtonUI>();
        if (ui != null)
        {
            ui.playerKill = GetComponent<PlayerKill>();
            Debug.Log("Local PlayerKill bound to UI");
        }
    }

    // =======================
    // Vision by Role
    // =======================
    void ConfigureVisionByRole()
    {
        VisionRangeController vision = GetComponent<VisionRangeController>();
        if (vision == null)
        {
            Debug.LogWarning("VisionRangeController not found");
            return;
        }

        PlayerRole role = GetMyRole();

        switch (role)
        {
            case PlayerRole.Impostor:
                vision.ConfigureVision(
                    6.5f,
                    LayerMask.GetMask("Player"),
                    LayerMask.GetMask("VisionBlock")
                );
                break;

            case PlayerRole.Sheriff:
                vision.ConfigureVision(
                    5f,
                    LayerMask.GetMask("Player"),
                    LayerMask.GetMask("VisionBlock")
                );
                break;

            case PlayerRole.Madman:
                vision.ConfigureVision(
                    4.5f,
                    LayerMask.GetMask("Player"),
                    LayerMask.GetMask("VisionBlock")
                );
                break;

            default: // Civilian
                vision.ConfigureVision(
                    4f,
                    LayerMask.GetMask("Player"),
                    LayerMask.GetMask("VisionBlock")
                );
                break;
        }

        Debug.Log($"Vision configured for role: {role}");
    }

    // =======================
    // Get Role from Photon
    // =======================
    PlayerRole GetMyRole()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("role", out object v))
            return (PlayerRole)(int)v;

        return PlayerRole.Civilian;
    }
}
