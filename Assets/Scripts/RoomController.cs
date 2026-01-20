using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

// ใช้ Hashtable ของ Photon แบบชัดเจน (กันชนกับ System.Collections.Hashtable)
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomController : MonoBehaviourPunCallbacks
{
    // ===== Custom Properties Keys =====
    private const string READY_KEY = "ready";
    private const string ROLE_KEY = "role";
    private const string ROLES_ASSIGNED = "rolesAssigned";

    [Header("UI")]
    public TMP_Text roomNameText;
    public TMP_Text playerListText;
    public TMP_Text countText;
    public TMP_Text statusText;
    public GameObject startButton;   // เฉพาะ Host
    public GameObject readyButton;   // ทุกคนกดได้
    public GameObject backButton;    // ออกจากห้อง

    [Header("Scenes")]
    public string roleRevealScene = "RoleRevealScene";
    public string backSceneName = "StartScene";

    [Header("Settings")]
    public byte maxPlayers = 8;

    // ===== Internal =====
    private bool isStarting = false;     // กันเรียกซ้ำ/กันค้าง
    private bool hasLoadedRoleScene = false;

    // ================= UNITY =================
    void Start()
    {
        if (!PhotonNetwork.InRoom) return;
        Debug.Log("Photon Region = " + PhotonNetwork.CloudRegion);
        // ตั้ง Ready เริ่มต้น
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { READY_KEY, false } });
        PhotonNetwork.AutomaticallySyncScene = true;

        RefreshUI();
        statusText.text = PhotonNetwork.IsMasterClient ? "You are Host" : "Waiting for Host";
    }

    // ================= UI =================
    void RefreshUI()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        roomNameText.text = $"Room: {PhotonNetwork.CurrentRoom.Name}";
        countText.text = $"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{maxPlayers}";

        string list = "";
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            bool ready = p.CustomProperties.ContainsKey(READY_KEY) && (bool)p.CustomProperties[READY_KEY];
            string hostText = p.IsMasterClient ? " (HOST)" : "";
            list += $"- {p.NickName}{hostText} [{(ready ? "READY" : "NOT READY")}]\n";
        }
        playerListText.text = list;

        if (startButton != null)
            startButton.SetActive(PhotonNetwork.IsMasterClient && !hasLoadedRoleScene);
    }

    // ================= READY =================
    public void ToggleReady()
    {
        if (!PhotonNetwork.InRoom) return;

        bool currentReady = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(READY_KEY)
            && (bool)PhotonNetwork.LocalPlayer.CustomProperties[READY_KEY];

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { READY_KEY, !currentReady } });
    }

    bool AreAllPlayersReady()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey(READY_KEY)) return false;
            if (!(bool)p.CustomProperties[READY_KEY]) return false;
        }
        return true;
    }

    // ================= START GAME =================
    public void StartGame()
{
    if (!PhotonNetwork.IsMasterClient) return;

    if (!AreAllPlayersReady())
    {
        statusText.text = "All players must be READY!";
        return;
    }

    // 🔒 เช็กว่าห้องเคยสุ่ม role ไปแล้วหรือยัง
    if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(ROLES_ASSIGNED))
        return;

    statusText.text = "Assigning roles...";

    // 🔒 ล็อกห้องทันที (กันเรียกซ้ำ)
    PhotonNetwork.CurrentRoom.SetCustomProperties(
        new Hashtable { { ROLES_ASSIGNED, true } }
    );

    AssignRoles();
}


    // ================= ROLE ASSIGN =================
    void AssignRoles()
{
    List<Player> players = new List<Player>(PhotonNetwork.PlayerList);

    int count = players.Count;
    int impostor = Random.Range(0, count);

    int sheriff = impostor;
    if (count >= 2)
        while (sheriff == impostor)
            sheriff = Random.Range(0, count);

    int madman = impostor;
    if (count >= 3)
        while (madman == impostor || madman == sheriff)
            madman = Random.Range(0, count);

    for (int i = 0; i < count; i++)
    {
        PlayerRole role = PlayerRole.Civilian;
        if (i == impostor) role = PlayerRole.Impostor;
        else if (i == sheriff) role = PlayerRole.Sheriff;
        else if (i == madman) role = PlayerRole.Madman;

        players[i].SetCustomProperties(
            new Hashtable { { ROLE_KEY, (int)role } }
        );
    }

    Debug.Log("Roles assigned");

    // ✅ โหลดฉากครั้งเดียวตรงนี้
    PhotonNetwork.LoadLevel(roleRevealScene);
}


    bool AllPlayersHaveRole()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey(ROLE_KEY))
                return false;
        }
        return true;
    }

    // ================= CALLBACKS =================
    public override void OnJoinedRoom()
    {
        RefreshUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshUI();
        statusText.text = $"{newPlayer.NickName} joined!";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshUI();
        statusText.text = $"{otherPlayer.NickName} left!";
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        RefreshUI();
        statusText.text = PhotonNetwork.IsMasterClient ? "You are Host now" : "Host changed";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // อัปเดต UI เมื่อ READY เปลี่ยน
        if (changedProps.ContainsKey(READY_KEY))
        {
            RefreshUI();
        }

        // เฉพาะ Host: รอ role ครบทุกคนแล้วค่อย LoadLevel "ครั้งเดียว"
        if (!PhotonNetwork.IsMasterClient) return;
        if (!isStarting) return;
        if (hasLoadedRoleScene) return;

    }

    // ================= BACK / LEAVE =================
    public void LeaveRoomAndBack()
    {
        if (PhotonNetwork.InRoom)
        {
            statusText.text = "Leaving room...";
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(backSceneName);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(backSceneName);
    }
    
}
