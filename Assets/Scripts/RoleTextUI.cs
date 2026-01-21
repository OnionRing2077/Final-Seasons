using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoleTextUI : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_Text roleText;

    const string ROLE_KEY = "role";

    void Awake()
    {
        if (roleText == null)
            roleText = GetComponent<TMP_Text>();
    }

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.LocalPlayer == null) return;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ROLE_KEY, out object v))
        {
            PlayerRole role = (PlayerRole)(int)v;

            roleText.text = RoleToText(role);

            // ถ้าอยากเปลี่ยนสีตาม role (เลือกใช้)
            // roleText.color = RoleToColor(role);
        }
        else
        {
            roleText.text = "Getting role...";
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // อัปเดตเฉพาะตอน role ของ "เรา" ถูกเปลี่ยน
        if (targetPlayer != PhotonNetwork.LocalPlayer) return;

        if (changedProps.ContainsKey(ROLE_KEY))
            Refresh();
    }

    string RoleToText(PlayerRole role)
    {
        switch (role)
        {
            case PlayerRole.Impostor: return "DarkWizard";
            case PlayerRole.Sheriff:  return "LightMagician";
            case PlayerRole.Madman:   return "ChaoticConjurer";
            default:                 return "Magician";
        }
    }

    // optional
    // Color RoleToColor(PlayerRole role)
    // {
    //     switch (role)
    //     {
    //         case PlayerRole.Impostor: return Color.red;
    //         case PlayerRole.Sheriff:  return Color.yellow;
    //         case PlayerRole.Madman:   return new Color(1f, 0.3f, 1f);
    //         default:                  return Color.white;
    //     }
    // }
}
