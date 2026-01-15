using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BackToMainMenu : MonoBehaviour
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
    public void OnLeftRoom()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
