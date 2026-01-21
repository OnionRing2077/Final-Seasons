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
    StartCoroutine(WaitForRole());
}

IEnumerator WaitForRole()
{
    // ⏳ รอจนกว่า role จะมา
    while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("role"))
    {
        yield return null;
    }

    PlayerRole role =
        (PlayerRole)(int)PhotonNetwork.LocalPlayer.CustomProperties["role"];

    roleText.text = GetRoleName(role);

    descriptionText.text = GetDescription(role);

    Debug.Log($"[RoleReveal] ROLE = {role}");

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
    string GetRoleName(PlayerRole role)
{
    switch (role)
    {
        case PlayerRole.Impostor:
            return "DarkWizard";
        case PlayerRole.Sheriff:
            return "LightMagician";
        case PlayerRole.Madman:
            return "ChaoticConjurer";
        default:
            return "Magician";
    }
}

}
