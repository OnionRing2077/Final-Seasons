using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ExitActions : MonoBehaviourPunCallbacks
{
    public string startScene = "StartScene";

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(startScene);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(startScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
