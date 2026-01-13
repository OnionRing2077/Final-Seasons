using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class RoleRevealUI : MonoBehaviour
{
    public TMP_Text roleText;
    public TMP_Text descriptionText;   // ⭐ เพิ่มตัวนี้
    public float showTime = 5f;
    public string gameScene = "GameScene";

    void Start()
    {
        ShowRole();
        StartCoroutine(LoadGameAfterDelay());
    }

    void ShowRole()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("role", out object v))
        {
            roleText.text = "UNKNOWN";
            return;
        }

        int role = (int)v;
        PlayerRole r = (PlayerRole)role;

        roleText.text = r.ToString();

        if (descriptionText != null)
            descriptionText.text = GetDescription(r);
    }

    string GetDescription(PlayerRole role)
    {
        switch (role)
        {
            case PlayerRole.Impostor:
                return "Kill everyone without getting caught";
            case PlayerRole.Sheriff:
                return "Shoot the Impostor, but miss = death";
            case PlayerRole.Madman:
                return "Help the Impostor secretly";
            default:
                return "Complete tasks and survive";
        }
    }

    IEnumerator LoadGameAfterDelay()
    {
        yield return new WaitForSeconds(showTime);
        PhotonNetwork.LoadLevel(gameScene);
    }
}
