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
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"VotePlayerManager: Duplicate detected! Destroying {gameObject.name}");
            Destroy(gameObject);
            return;
        }

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

            // Fix: Allow offline mode/debug mode to trigger end as well
            bool shouldEnd = PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null || _forceDebugMode;

            if (shouldEnd && _currentTimer <= 0)
            {
                ForceEndVoting();
            }
        }
    }

    private void Start()
    {
        // Auto-start the meeting when this scene loads
        ValidateReferences();
        StartMeeting();
    }

    private void ValidateReferences()
    {
        if (_emergencyMeetingWindow == null)
        {
            Debug.LogWarning("VotePlayerManager: Emergency Window is NULL! Attempting to find 'EmergencyMeetingWindow'...");
            Debug.LogWarning("VotePlayerManager: Emergency Window is NULL! Attempting to find 'EmergencyMeetingWindow'...");
            _emergencyMeetingWindow = GameObject.Find("EmergencyMeetingWindow");
            if (_emergencyMeetingWindow == null) _emergencyMeetingWindow = GameObject.Find("Emergency Meeting Window"); // Added space
            if (_emergencyMeetingWindow == null) _emergencyMeetingWindow = FindDeep(transform.root, "EmergencyMeetingWindow")?.gameObject;
            if (_emergencyMeetingWindow == null) _emergencyMeetingWindow = FindDeep(transform.root, "Emergency Meeting Window")?.gameObject; // Added space
            if (_emergencyMeetingWindow == null) _emergencyMeetingWindow = transform.Find("Canvas/EmergencyMeetingUI/EmergencyMeetingWindow")?.gameObject;
            
            // Fallback: Check MeetingManager
            if (_emergencyMeetingWindow == null && MeetingManager.Instance != null)
            {
                _emergencyMeetingWindow = MeetingManager.Instance.meetingUI;
                Debug.Log("VotePlayerManager: Found Emergency Window via MeetingManager!");
            }

        }

        if (_votePlayerItemContainer == null)
        {
            if (_votePlayerItemContainer == null) _votePlayerItemContainer = FindDeep(transform.root, "VotePlayerItemContainer");
            if (_votePlayerItemContainer == null) _votePlayerItemContainer = FindDeep(transform.root, "Content"); // Common name
            if (_votePlayerItemContainer == null) _votePlayerItemContainer = FindDeep(transform.root, "Voting"); // Found in screenshot
            if (_votePlayerItemContainer == null) _votePlayerItemContainer = FindDeep(transform.root, "PlayerList");
        }

        if (_skipVoteBtn == null)
        {
             Debug.LogWarning("VotePlayerManager: Skip Vote Button is NULL! Searching recursively...");
             var obj = FindDeep(transform.root, "SkipVoteButton");
             if (obj) _skipVoteBtn = obj.GetComponent<Button>();
        }

        if (_skipVoterContainer == null)
        {
            var obj = FindDeep(transform.root, "SkipVoterContainer");
            if (obj) _skipVoterContainer = obj;
        }

        if (_resultText == null)
        {
             var obj = FindDeep(transform.root, "ResultText");
             if (obj) _resultText = obj.GetComponent<TMP_Text>();
        }

        if (_timerText == null)
        {
             var obj = FindDeep(transform.root, "TimerText");
             if (obj == null) obj = FindDeep(transform.root, "Timer Text"); // Spaces
             if (obj == null) obj = FindDeep(transform.root, "TimeText");
             if (obj == null) obj = FindDeep(transform.root, "Timer");
             if (obj == null) obj = FindDeep(transform.root, "Time");
             
             if (obj) _timerText = obj.GetComponent<TMP_Text>();
        }

        // Try to find the Prefab in Resources if it's completely null? 
        // No, that's too dangerous. But we can warn louder.
        if (_votePlayerItemPrefab == null)
        {
            Debug.LogWarning("VotePlayerManager: Prefab is NULL. Attempting to load 'VotePlayerItem' from Resources folder...");
            _votePlayerItemPrefab = Resources.Load<GameObject>("VotePlayerItem");
            
            if (_votePlayerItemPrefab == null)
            {
                 Debug.LogError("VotePlayerManager: CRITICAL ERROR - Could not find 'VotePlayerItem' in Resources folder! Please move 'VotePlayerItem.prefab' into a folder named 'Resources'.");
            }
        }


    }

    public void StartMeeting()
    {
        Debug.Log("VotePlayerManager: StartMeeting() CALLED!");
        
        // Ensure game is not paused
        Time.timeScale = 1f; 

        if(_emergencyMeetingWindow) _emergencyMeetingWindow.SetActive(true);
        else Debug.LogError("VotePlayerManager: Emergency Window is NULL!");

        _hasAlreadyVoted = false;
        _isInResultsPhase = false;
        _allVotes.Clear();
        _currentTimer = _votingDuration;

        // Force initial update
        if (_timerText) 
        {
            _timerText.gameObject.SetActive(true);
            _timerText.text = Mathf.CeilToInt(_currentTimer).ToString();
            Debug.Log($"VotePlayerManager: Timer Initialized to {_currentTimer}");
        }
        else
        {
             Debug.LogError("VotePlayerManager: TimerText is NULL! Countdown will not be visible.");
        }

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
        
        // Final sanity check for container
        if (_skipVoterContainer == null)
            Debug.LogWarning("VotePlayerManager: SkipVoterContainer is NULL! Skip votes will not show icons.");


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
        if (_votePlayerItemContainer != null)
        {
            foreach (Transform child in _votePlayerItemContainer)
            {
                Destroy(child.gameObject);
            }
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
                Debug.LogError($"VotePlayerManager: Prefab missing VotePlayerItem script! Attempting to add it...");
                newItem = itemObj.AddComponent<VotePlayerItem>();
            }
            
            // For mock, we pass null player. The Item script handles null and shows "Mock Player".
            // We can set name manually if we expose it or use a trick.
            // Since we can't easily set NickName on null player, let's just use Initialize(null) 
            // and maybe modify Initialize to take a string name override? 
            // Or just let it be generic for now to prove it works.
            
            bool isDead = fakeNames[i].Contains("Dead");
            // Use negative IDs for mocks to distinguish from real actors (usually > 0)
            int mockId = -100 - i; 
            newItem.Initialize(null, isDead, mockId);
            
            _createdVoteItems.Add(mockId, newItem); // Track it so we can show results
            Debug.Log($"VotePlayerManager: Created Mock Player {i} ({fakeNames[i]})");
            
            // Manual text override for testing (if fields are public, but they are private serialized)
            // So we rely on Initialize handling null.
        }
    }
    
    private void CreatePlayerItem(Photon.Realtime.Player player, bool isDead, bool isReporter)
    {
        if (_votePlayerItemPrefab == null)
        {
             Debug.LogError("VotePlayerManager: Cannot create player item because Prefab is NULL!");
             return;
        }

        GameObject itemObj = Instantiate(_votePlayerItemPrefab, _votePlayerItemContainer);
        VotePlayerItem newItem = itemObj.GetComponent<VotePlayerItem>();
        
        if (newItem == null) 
        {
             Debug.LogError($"VotePlayerManager: Prefab missing VotePlayerItem script! Attempting to add it...");
             newItem = itemObj.AddComponent<VotePlayerItem>();
        }
        
        // Ensure proper scaling/parenting
        itemObj.transform.localScale = Vector3.one;
        
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
        
        // Debug/Local Override: If we have mock items and NO real players (or debug mode)
        if (livingCount == 0 && _createdVoteItems.Count > 0)
        {
             livingCount = _createdVoteItems.Count(x => !x.Value.IsDeadForLogic()); 
             // Need to expose IsDead helper in Item or just count manually
             // For simplicity, let's just count all items minus known dead
             livingCount = 0;
             foreach(var kvp in _createdVoteItems)
             {
                 // If ID < -1, it's mock
                 if (kvp.Key <= -100) 
                 {
                     // Check dead status from name or stored flag? 
                     // We passed isDead to Initialize. Let's assume alive for test or stored in item.
                     // Actually, we can just check if item interactable? No.
                     livingCount++; 
                 }
             }
             if (livingCount == 0) livingCount = 1; // Fallback
        }

        if (_allVotes.Count >= livingCount)
        {
            // ForceEndVoting(); // CHANGED: Wait for timer to expire instead of ending immediately
            Debug.Log("VotePlayerManager: All votes received. Waiting for timer...");
        }
    }

    private void ForceEndVoting()
    {
        if (_isInResultsPhase) return;
        _isInResultsPhase = true; // Fix: Stop update loop immediately
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

        // Transition Logic: Set Properties -> Load Scene
        if (PhotonNetwork.IsMasterClient)
        {
            bool isSkip = (ejectedPlayerId < 0);
            
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["Vote_IsSkip"] = isSkip;
            props["Vote_EjectedID"] = ejectedPlayerId;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            // Handle Ejection (Data Logic)
            if (!isSkip)
            {
                Photon.Realtime.Player target = PhotonNetwork.CurrentRoom.GetPlayer(ejectedPlayerId);
                if (target != null)
                {
                    ExitGames.Client.Photon.Hashtable deadProp = new ExitGames.Client.Photon.Hashtable();
                    deadProp["IsDead"] = true;
                    target.SetCustomProperties(deadProp);
                }
            }

            PhotonNetwork.LoadLevel("EndVoteScene");
        }
        else if (PhotonNetwork.CurrentRoom == null)
        {
            // Offline Fallback
            Debug.Log($"VotePlayerManager Offline: Result Ejected={ejectedPlayerId}. Loading EndVoteScene...");
             // Note: Offline mode won't have properties synced, so EndVoteScene might need mock data logic
             // But for now we just load.
             UnityEngine.SceneManagement.SceneManager.LoadScene("EndVoteScene");
        }
    }

    /* 
    // OLD LOGIC - COMMENTED OUT FOR NEW SCENE FLOW
    [PunRPC]
    public void ShowResultsPhaseRPC(int ejectedPlayerId)
    {
        // ... (Original Code)
    }

    private System.Collections.IEnumerator EndMeetingCoroutine(int ejectedPlayerId)
    {
        // ... (Original Code)
    }
    */
    
    // Recursive Find Helper
    private Transform FindDeep(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var result = FindDeep(child, name);
            if (result != null) return result;
        }
        return null;
    }

    // Helper for debugging logs
    public void Log(string msg) { Debug.Log(msg); }
}
