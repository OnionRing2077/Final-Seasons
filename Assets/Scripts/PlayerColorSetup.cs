using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerColorSetup : MonoBehaviourPunCallbacks
{
    private CharacterColor characterColor;

    void Start()
    {
        characterColor = GetComponent<CharacterColor>();

        if (photonView.IsMine)
        {
            int colorIndex = GetFreeColorIndex();

            Hashtable props = new Hashtable();
            props["color"] = colorIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        ApplyColor();
    }

    public override void OnPlayerPropertiesUpdate(
        Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("color"))
        {
            ApplyColor();
        }
    }

    void ApplyColor()
    {
        if (photonView.Owner.CustomProperties.TryGetValue("color", out object value))
        {
            int index = (int)value;
            characterColor.SetColor(PlayerColors.Colors[index]);
        }
    }

    int GetFreeColorIndex()
    {
        bool[] used = new bool[PlayerColors.Colors.Length];

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("color", out object v))
            {
                used[(int)v] = true;
            }
        }

        for (int i = 0; i < used.Length; i++)
        {
            if (!used[i]) return i;
        }

        return 0; // fallback (ไม่ควรเกิด)
    }
}
