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

        string txt = "";
        Color color = Color.white;

        switch (result)
        {
            case GameResult.CivilianWin:
                // User requested: Civilian -> Magician, Sheriff -> Light Magician
                // Since this is a team win, we use "Magician" (representing the good team).
                txt = "Magician Win"; 
                color = Color.green;
                break;
            case GameResult.ImpostorWin:
                txt = "Dark Wizard Win";
                color = Color.red;
                break;
            case GameResult.MadmanWin:
                txt = "Chaotic Conjurer Win";
                color = new Color(0.5f, 0, 1f); // Purple
                break;
            case GameResult.SheriffWin:
                txt = "Light Magician Win";
                color = Color.yellow; // Sheriff usually yellow/gold
                break;
        }

        // Check Local Player Role & Result
        string myRoleStr = "Civilian";
        PlayerRole myRole = PlayerRole.Civilian;
        
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("role", out object r))
        {
             myRole = (PlayerRole)(int)r;
             myRoleStr = myRole.ToString();
        }

        bool iWon = false;
        if (result == GameResult.ImpostorWin && myRole == PlayerRole.Impostor) iWon = true;
        else if (result == GameResult.MadmanWin && myRole == PlayerRole.Madman) iWon = true;
        else if ((result == GameResult.CivilianWin || result == GameResult.SheriffWin) && 
                 (myRole == PlayerRole.Civilian || myRole == PlayerRole.Sheriff)) iWon = true;

        string outcome = iWon ? "YOU WON!" : "YOU LOST";
        
        if (winnerText)
        {
            winnerText.text = $"{txt}\n\nYour Role: {myRoleStr}\n{outcome}";
            winnerText.color = iWon ? Color.green : Color.red; // Green for win, Red for loss (override global color?)
            // Or keep global color for the title? Let's generic white or keep global color.
            // Let's use the team color defined above for consistency, but maybe add outcome text separately?
            // For now, simpler is better.
            winnerText.color = color; 
        }

        if (winPanel) winPanel.SetActive(true);

        Debug.Log($"SHOW WIN: {txt} | MyRole: {myRole} | I Won: {iWon}");
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
