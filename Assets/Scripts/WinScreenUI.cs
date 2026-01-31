using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class WinScreenUI : MonoBehaviour
{
    public static WinScreenUI Instance;

    public GameObject winPanel;
    public TMP_Text winnerText;

    GameResult currentResult;
    bool isGameOver = false;

    void Awake()
    {
        Instance = this;
        // Don't use OnGUI if we have real UI, but for now we assume we might need fallback
        if (winPanel) winPanel.SetActive(false);
    }

    public void ShowWin(GameResult result)
    {
        currentResult = result;
        isGameOver = true;

        string roleTitle = "";
        Color color = Color.white;

        // 1. Determine Winner Role Name & Color
        switch (result)
        {
            case GameResult.CivilianWin:
                roleTitle = GetRoleDisplayName(PlayerRole.Civilian); 
                color = Color.green;
                break;
            case GameResult.ImpostorWin:
                roleTitle = GetRoleDisplayName(PlayerRole.Impostor);
                color = Color.red;
                break;
            case GameResult.MadmanWin:
                roleTitle = GetRoleDisplayName(PlayerRole.Madman);
                color = new Color(0.5f, 0, 1f); // Purple
                break;
            case GameResult.SheriffWin:
                roleTitle = GetRoleDisplayName(PlayerRole.Sheriff);
                color = Color.yellow; 
                break;
        }

        string finalTitle = roleTitle + " Win";

        // ==================================================
        // LIST PLAYERS & ROLES
        // ==================================================
        string listTxt = "\n\n"; 

        if (PhotonNetwork.PlayerList != null)
        {
            foreach (var p in PhotonNetwork.PlayerList)
            {
                string roleName = "Unknown";
                if (p.CustomProperties.TryGetValue("role", out object rVal))
                {
                    PlayerRole roleEnum = (PlayerRole)(int)rVal;
                    // Use the SAME helper method so strings match exactly
                    roleName = GetRoleDisplayName(roleEnum);
                }
                
                listTxt += $"{p.NickName} : {roleName}\n";
            }
        }

        if (winnerText)
        {
            winnerText.text = finalTitle + listTxt;
            winnerText.color = color;
        }

        if (winPanel) winPanel.SetActive(true);

        Debug.Log("SHOW WIN: " + finalTitle);
    }

    /// <summary>
    /// Centralized method for Role Names to ensure consistency.
    /// </summary>
    string GetRoleDisplayName(PlayerRole role)
    {
        switch (role)
        {
            case PlayerRole.Impostor: return "DarkWizard";
            case PlayerRole.Sheriff:  return "LightMagician";
            case PlayerRole.Madman:   return "ChaoticConjurer";
            default:                  return "Magician";
        }
    }

    // Fallback GUI if no UI assigned
    void OnGUI()
    {
        if (!isGameOver) return;
        if (winPanel != null && winPanel.activeSelf) return; // Use real UI if active

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 40;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        string msg = "GAME OVER";
        if (currentResult == GameResult.CivilianWin) msg = "CIVILIANS WIN";
        if (currentResult == GameResult.ImpostorWin) msg = "IMPOSTORS WIN";
        if (currentResult == GameResult.MadmanWin) msg = "MADMAN WINS";

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", style);
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), msg, style);

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 50), "Back to Menu"))
        {
            LeaveGame();
        }
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }
}
