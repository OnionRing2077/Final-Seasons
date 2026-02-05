using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BackToMainMenu : MonoBehaviourPunCallbacks
{
    public string mainMenuScene = "MainMenu";

    public void GoBack()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    // Photon callback
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
