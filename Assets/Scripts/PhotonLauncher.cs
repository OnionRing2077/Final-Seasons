using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private string lobbySceneName = "LobbyScene";

    private static bool hasStarted = false;

    void Awake()
    {
        if (hasStarted)
        {
            Destroy(gameObject);
            return;
        }

        hasStarted = true;
        DontDestroyOnLoad(gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
{
    Debug.Log("Joined Lobby");
    // ❌ ลบ SceneManager.LoadScene ออก
}


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected: " + cause);
    }
}
