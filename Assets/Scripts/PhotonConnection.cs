using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings(); // เชื่อมต่อเซิร์ฟเวอร์
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server!");
        SceneManager.LoadScene("LobbyScene"); // ไปหน้า Lobby ทันที เมื่อเชื่อมได้
    }
}
