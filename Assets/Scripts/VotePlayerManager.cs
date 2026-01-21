using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; 
using ExitGames.Client.Photon;
using TMPro;

public class VotePlayerManager : MonoBehaviourPunCallbacks
{
    public static VotePlayerManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject _emergencyMeetingWindow;
    [SerializeField] private Button _skipVoteBtn;
    [SerializeField] private GameObject _votePlayerItemPrefab; // Changed from VotePlayerItem to GameObject
    [SerializeField] private Transform _votePlayerItemContainer;
    [SerializeField] private TMP_Text _resultText; 
    [SerializeField] private TMP_Text _timerText; 

    [Header("Voter Icons")]
    [SerializeField] private Image _voterIconPrefab; 
    [SerializeField] private Transform _skipVoterContainer; 

    [Header("Settings")]
    [SerializeField] private float _votingDuration = 30f;
    [SerializeField] private float _resultsDuration = 5f;
    [SerializeField] private bool _forceDebugMode = true; // Added for testing

    // State Tracking
    private bool _hasAlreadyVoted = false;
    private Dictionary<int, VotePlayerItem> _createdVoteItems = new Dictionary<int, VotePlayerItem>();
    
    private Dictionary<int, int> _allVotes = new Dictionary<int, int>();
    
    private float _currentTimer;
    private bool _isInResultsPhase = false;

    private void Awake()
    {
        Instance = this;
        if (_emergencyMeetingWindow) _emergencyMeetingWindow.SetActive(false);
    }

    private void Update()
    {
        if (_emergencyMeetingWindow == null) return;

        if (_emergencyMeetingWindow.activeSelf && !_isInResultsPhase)
        {
            _currentTimer -= Time.deltaTime;
            if (_timerText) _timerText.text = Mathf.CeilToInt(_currentTimer).ToString();

            if (PhotonNetwork.IsMasterClient && _currentTimer <= 0)
            {
                ForceEndVoting();
            }
        }
    }

    private void Start()
    {
        // Auto-start the meeting when this scene loads
        StartMeeting();
    }

    public void StartMeeting()
    {
        Debug.Log("VotePlayerManager: StartMeeting() CALLED!");
        if(_emergencyMeetingWindow) _emergencyMeetingWindow.SetActive(true);
        else Debug.LogError("VotePlayerManager: Emergency Window is NULL!");

        _hasAlreadyVoted = false;
        _isInResultsPhase = false;
        _allVotes.Clear();
        _currentTimer = _votingDuration;

        bool amIDead = IsLocalPlayerDead();
        
        if(_skipVoteBtn) {
            _skipVoteBtn.interactable = !amIDead;
            _skipVoteBtn.onClick.RemoveAllListeners();
            _skipVoteBtn.onClick.AddListener(() => CastVote(-1));
        }

        if (_skipVoterContainer)
        {
            foreach(Transform child in _skipVoterContainer) Destroy(child.gameObject);
        }

        PopulatePlayerList();
    }
    
    // (Removed [PunRPC] from StartMeetingRPC since we use Start() now)
    // Kept helper for reference if needed, but the main entry is Start()

    private void PopulatePlayerList()
    {
        Debug.Log("VotePlayerManager: PopulatePlayerList called.");
        
        // Safety Check
        if (_votePlayerItemContainer == null) {
            Debug.LogError("VotePlayerManager: Player Item Container is NOT assigned in Inspector!");
            return;
        }
        if (_votePlayerItemPrefab == null) {
            Debug.LogError("VotePlayerManager: Vote Player Item Prefab is NOT assigned in Inspector!");
            return;
        }

        // 1. Clear existing items (Placeholders)
        foreach (Transform child in _votePlayerItemContainer)
        {
            Destroy(child.gameObject);
        }
        _createdVoteItems.Clear();

        // 2. Offline Mode (Debug) - If testing scene directly OR Force Debug
        if (_forceDebugMode || PhotonNetwork.CurrentRoom == null)
        {
            Debug.LogWarning("VotePlayerManager: No Photon Room found. Usage Offline/Debug Mode. Creating Mock Players...");
            CreateMockPlayers();
            return;
        }
        
        Debug.Log("VotePlayerManager: Online Mode detected."); // Trace Online path

        // 3. Online Mode
        int reporterId = -1;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("ReporterActorNumber", out object rId))
            reporterId = (int)rId;

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            bool isDead = false;
            // Use safe checking for custom properties
            if (player.CustomProperties != null && player.CustomProperties.TryGetValue("IsDead", out object d)) 
                isDead = (bool)d;

            if (isDead) continue; 

            CreatePlayerItem(player, isDead, player.ActorNumber == reporterId);
        }
    }

    private void CreateMockPlayers()
    {
        // Debug Mock Data
        string[] fakeNames = { "Red", "Blue", "Green (Dead)", "Yellow" };
        Debug.Log($"VotePlayerManager: Creating {fakeNames.Length} mock players...");

        for (int i = 0; i < fakeNames.Length; i++)
        {
            GameObject itemObj = Instantiate(_votePlayerItemPrefab, _votePlayerItemContainer);
            VotePlayerItem newItem = itemObj.GetComponent<VotePlayerItem>();

            if (newItem == null) {
                Debug.LogError($"VotePlayerManager: Prefab does not have VotePlayerItem script!");
                continue;
            }
            
            // For mock, we pass null player. The Item script handles null and shows "Mock Player".
            // We can set name manually if we expose it or use a trick.
            // Since we can't easily set NickName on null player, let's just use Initialize(null) 
            // and maybe modify Initialize to take a string name override? 
            // Or just let it be generic for now to prove it works.
            
            bool isDead = fakeNames[i].Contains("Dead");
            newItem.Initialize(null, isDead);
            Debug.Log($"VotePlayerManager: Created Mock Player {i} ({fakeNames[i]})");
            
            // Manual text override for testing (if fields are public, but they are private serialized)
            // So we rely on Initialize handling null.
        }
    }
    
    private void CreatePlayerItem(Photon.Realtime.Player player, bool isDead, bool isReporter)
    {
        GameObject itemObj = Instantiate(_votePlayerItemPrefab, _votePlayerItemContainer);
        VotePlayerItem newItem = itemObj.GetComponent<VotePlayerItem>();
        
        if (newItem == null) return;
        
        // Pass isDead status
        newItem.Initialize(player, isDead);  
        
        newItem.ShowReporterStatus(isReporter);

        bool amIDead = IsLocalPlayerDead();
        newItem.SetInteractable(!amIDead); 

        _createdVoteItems.Add(player.ActorNumber, newItem);
    } 

    private bool IsLocalPlayerDead()
    {
        if (PhotonNetwork.LocalPlayer != null && 
            PhotonNetwork.LocalPlayer.CustomProperties != null &&
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsDead", out object d))
            return (bool)d;
        return false;
    }

    public void CastVote(int targetActorNumber)
    {
        // Early return removed to allow Offline/Debug logic to run below
        
        if (_hasAlreadyVoted) return;
        // ... (rest of logic)

        _hasAlreadyVoted = true;
        ToggleAllButtons(false);
        if(_skipVoteBtn) _skipVoteBtn.interactable = false;

        if (PhotonNetwork.CurrentRoom == null || photonView == null)
        {
            Debug.Log("VotePlayerManager: Offline Vote Cast - Bypassing RPC.");
            ReceiveVoteRPC(-1, targetActorNumber); // Use -1 for "Mock Yourself" as voter
        }
        else
        {
            photonView.RPC("ReceiveVoteRPC", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, targetActorNumber);
        }
    }

    private void ToggleAllButtons(bool interactable)
    {
        foreach (var item in _createdVoteItems.Values)
        {
            item.SetInteractable(interactable);
        }
    }

    [PunRPC]
    public void ReceiveVoteRPC(int voterActorNumber, int targetActorNumber)
    {
        if (!_allVotes.ContainsKey(voterActorNumber))
        {
            _allVotes.Add(voterActorNumber, targetActorNumber);
        }

        if (_createdVoteItems.ContainsKey(voterActorNumber))
        {
            _createdVoteItems[voterActorNumber].ShowVotedStatus();
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
             CheckForAllVotes();
        }
    }
    
    private void CheckForAllVotes()
    {
        int livingCount = 0;
        foreach(var p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            bool isDead = false;
            if (p.CustomProperties.TryGetValue("IsDead", out object d)) isDead = (bool)d;
            if (!isDead) livingCount++;
        }

        if (_allVotes.Count >= livingCount)
        {
            ForceEndVoting(); 
        }
    }

    private void ForceEndVoting()
    {
        if (_isInResultsPhase) return;
        CalculateVotesAndShowResults();
    }

    private void CalculateVotesAndShowResults()
    {
        Dictionary<int, int> scores = new Dictionary<int, int>();
        int skipCount = 0;

        foreach(var kvp in _allVotes)
        {
            int target = kvp.Value;
            if(target == -1) skipCount++;
            else
            {
                if(scores.ContainsKey(target)) scores[target]++;
                else scores.Add(target, 1);
            }
        }

        int maxVotes = -1;
        int ejectedPlayerId = -1; 
        bool isTie = false;

        foreach(var pair in scores)
        {
            if(pair.Value > maxVotes)
            {
                maxVotes = pair.Value;
                ejectedPlayerId = pair.Key;
                isTie = false;
            }
            else if (pair.Value == maxVotes)
            {
                isTie = true;
            }
        }

        if (skipCount > maxVotes)
        {
            ejectedPlayerId = -1; 
            isTie = false;
        }
        else if (skipCount == maxVotes)
        {
            isTie = true; 
        }

        if (isTie) ejectedPlayerId = -2; 
        
        // Offline / Mock Mode Check
        if (PhotonNetwork.CurrentRoom == null || photonView == null)
        {
             Debug.Log("VotePlayerManager: Offline Mode - Bypassing RPC for ShowResults.");
             ShowResultsPhaseRPC(ejectedPlayerId);
        }
        else
        {
             photonView.RPC("ShowResultsPhaseRPC", RpcTarget.All, ejectedPlayerId);
        }
    }

    [PunRPC]
    public void ShowResultsPhaseRPC(int ejectedPlayerId)
    {
        _isInResultsPhase = true;
        _currentTimer = 0; 
        
        foreach(var kvp in _allVotes)
        {
            int voterId = kvp.Key;
            int targetId = kvp.Value;

            Photon.Realtime.Player voter = PhotonNetwork.CurrentRoom.GetPlayer(voterId);
            if(voter == null) continue;

            // Get Voter Color
            int colorIndex = 0;
            if(voter.CustomProperties.TryGetValue("color", out object cBox)) 
                colorIndex = (int)cBox;
            
            Color voterColor = Color.white;
            if (colorIndex >= 0 && colorIndex < PlayerColors.Colors.Length)
                voterColor = PlayerColors.Colors[colorIndex];

            if (targetId == -1)
            {
                 // Skip Vote Visual
                 if(_voterIconPrefab && _skipVoterContainer)
                 {
                     Image icon = Instantiate(_voterIconPrefab, _skipVoterContainer);
                     icon.color = voterColor;
                     icon.gameObject.SetActive(true);
                 }
            }
            else if (_createdVoteItems.ContainsKey(targetId))
            {
                _createdVoteItems[targetId].AddVoter(voterColor); 
            }
        }

        if (_resultText)
        {
            if (ejectedPlayerId == -1) _resultText.text = "Skipped (No one ejected)";
            else if (ejectedPlayerId == -2) _resultText.text = "Tie (No one ejected)";
            else 
            {
                Photon.Realtime.Player ejected = PhotonNetwork.CurrentRoom.GetPlayer(ejectedPlayerId);
                _resultText.text = ejected.NickName + " was ejected.";
            }
        }

        StartCoroutine(EndMeetingCoroutine(ejectedPlayerId));
    }

    private System.Collections.IEnumerator EndMeetingCoroutine(int ejectedPlayerId)
    {
        yield return new WaitForSeconds(_resultsDuration);

        // Actual Ejection Logic
        // Actual Ejection Logic
         if (ejectedPlayerId >= 0 && PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.ActorNumber == ejectedPlayerId)
         {
             var health = PhotonNetwork.LocalPlayer.TagObject as GameObject;
             if(health) health.GetComponent<PlayerHealth>().RPC_Die(-1); 
         }
        
        _emergencyMeetingWindow.SetActive(false); // Just hide it before scene change

        Debug.Log("VotePlayerManager: Return to GameScene...");
        
        // Return to Game Scene
        if (PhotonNetwork.CurrentRoom == null || !PhotonNetwork.IsConnected)
        {
             UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameScene");
            }
            // Clients waiting for Master to load level (Assuming AutoSyncScene implies following)
        }
    }
    
    // Helper for debugging logs
    public void Log(string msg) { Debug.Log(msg); }
}
