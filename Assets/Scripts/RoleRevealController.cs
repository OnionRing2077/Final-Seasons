using Photon.Pun;
using TMPro;
using UnityEngine;
using System.Collections;

public class RoleRevealController : MonoBehaviour
{
    public TMP_Text roleText;
    public TMP_Text descriptionText;
    public float showTime = 5f; // เวลาโชว์ Role

    void Start()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("role", out object v))
        {
            roleText.text = "NO ROLE";
            return;
        }

        PlayerRole role = (PlayerRole)(int)v;

        roleText.text = role.ToString();
        descriptionText.text = GetDescription(role);

        StartCoroutine(AutoStart());
    }

    IEnumerator AutoStart()
    {
        yield return new WaitForSeconds(showTime);
        PhotonNetwork.LoadLevel("GameScene");
    }

    string GetDescription(PlayerRole role)
    {
        switch (role)
        {
            case PlayerRole.Impostor:
                return "Eliminate everyone without being caught.";
            case PlayerRole.Sheriff:
                return "Find and shoot the Impostor.";
            case PlayerRole.Madman:
                return "Cause chaos and mislead others.";
            default:
                return "Complete tasks and survive.";
        }
    }
}
