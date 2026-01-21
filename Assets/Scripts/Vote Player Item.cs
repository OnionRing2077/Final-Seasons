using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class VotePlayerItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _playerStatusText; // Status (Alive/Dead)
    [SerializeField] private Transform _characterContainer; // จุดที่จะให้ตัวละครไปเกิด
    [SerializeField] private GameObject _characterPrefab;   // ตัวละคร (Prefab) ที่จะให้โชว์
    [SerializeField] private Button _voteButton;
    [SerializeField] private GameObject _votedBadge; // รูปที่ขึ้นว่า "VOTED"
    [SerializeField] private GameObject _reporterBadge; // รูป Megaphone
    [SerializeField] private Transform _voterContainer; // ที่วาง icon คนโหวต
    [SerializeField] private GameObject _voterIconPrefab; // Prefab icon เล็กๆ (แก้เป็น GameObject ให้ลากง่ายขึ้น)

    private Photon.Realtime.Player _targetPlayer;

    void Awake()
    {
        // Auto-find references if missing (Robustness)
        if (_playerNameText == null)
        {
            var t = transform.Find("[PLAYER NAME]");
            if(t) _playerNameText = t.GetComponent<TMP_Text>();
        }

        if (_playerStatusText == null)
        {
            var t = transform.Find("[PLAYER STATUS]");
            if(t) _playerStatusText = t.GetComponent<TMP_Text>();
        }
    }

    public void Initialize(Photon.Realtime.Player player, bool isDead)
    {
        try
        {
            Debug.Log($"VotePlayerItem: Initialize called. Player={(player==null?"null":player.NickName)}, Dead={isDead}");
            _targetPlayer = player;
            if (player != null)
            {
                if(_playerNameText) _playerNameText.text = player.NickName;
                else Debug.LogWarning("VotePlayerItem: PlayerNameText is MISSING!");
            }
            else
            {
                 // Mock Data handling if needed
                 if(_playerNameText) 
                 {
                     _playerNameText.text = "Mock Player";
                     Debug.Log($"VotePlayerItem: Set NameText to 'Mock Player' on {_playerNameText.gameObject.name}");
                 }
                 else Debug.LogWarning("VotePlayerItem: PlayerNameText is MISSING (Mock Mode)!");
            }
    
            if(_playerStatusText) 
            {
                _playerStatusText.text = isDead ? "Dead" : "Alive";
                _playerStatusText.color = isDead ? Color.red : Color.white;
            }
            else Debug.LogWarning("VotePlayerItem: PlayerStatusText is MISSING!");
            
            // Update: Enable Mock Character (Logics are stripped below)
            if (_characterPrefab && _characterContainer)
            {
                // Clear old children if any
                foreach(Transform child in _characterContainer) Destroy(child.gameObject);
    
                GameObject charVisual = Instantiate(_characterPrefab, _characterContainer);
                charVisual.transform.localPosition = Vector3.zero;
                charVisual.transform.localScale = Vector3.one;
    
                // Strip Gameplay Scripts to prevent UI errors
                MonoBehaviour[] scriptsToDelete = charVisual.GetComponentsInChildren<MonoBehaviour>();
                foreach(var script in scriptsToDelete) 
                {
                    if (script == null) continue; // Skip "Missing Script" components preventing crash
                    
                    string typeName = script.GetType().Name;
                    // Destroy Gameplay Scripts AND Photon Scripts
                    if (typeName == "PlayerIdentity" || 
                        typeName == "PlayerColorSetup" || 
                        typeName == "PlayerMovement" ||
                        typeName == "PlayerMovement2D" || // <--- Added from log
                        typeName == "PlayerHealth" ||
                        typeName == "PlayerKill" ||       // <--- Added from log
                        typeName == "BodyReport" ||   
                        typeName == "DeadBody" ||     
                        typeName == "PlayerVisibilityController" || // <--- Added from log
                        typeName.Contains("Photon")) // Nuke all Photon views
                    {
                        script.enabled = false; // Stop Update() IMMEDIATELY
                        Destroy(script); // Remove at end of frame
                    }
                }
                
                // Solution: We don't really need to destroy RB for UI to work, just setting it to Simulated = false is enough.
                var rb = charVisual.GetComponent<Rigidbody2D>();
                if(rb) rb.simulated = false; // Disable physics simulation
                
                var col = charVisual.GetComponent<Collider2D>();
                if(col) col.enabled = false; // Disable collider
            }
            
            // เมื่อกดปุ่ม ให้เรียกฟังก์ชันที่ Manager
            _voteButton.onClick.AddListener(() => {
                if (_targetPlayer != null)
                    VotePlayerManager.Instance.CastVote(_targetPlayer.ActorNumber);
                else
                    VotePlayerManager.Instance.Log("Clicked Mock Player (No ID)");
            });
            
            if(_votedBadge) _votedBadge.SetActive(false);
            if(_reporterBadge) _reporterBadge.SetActive(false);
            
            // Clear old voters
            foreach(Transform child in _voterContainer) {
                Destroy(child.gameObject);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"VotePlayerItem: CRASH during Initialize! Error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void SetInteractable(bool canVote)
    {
        VotePlayerManager.Instance.Log("Setting Interactable: " + canVote);
        if(_voteButton) _voteButton.interactable = canVote;
    }
    
    public void ShowVotedStatus()
    {
        if(_votedBadge) _votedBadge.SetActive(true);
    }

    public void ShowReporterStatus(bool isReporter)
    {
        if (_reporterBadge) _reporterBadge.SetActive(isReporter);
    }

    public void AddVoter(Color voterColor)
    {
        if (_voterIconPrefab && _voterContainer)
        {
            GameObject iconObj = Instantiate(_voterIconPrefab, _voterContainer);
            Image icon = iconObj.GetComponent<Image>();
            if (icon == null) icon = iconObj.GetComponentInChildren<Image>(); 

            if (icon != null) 
            {
                icon.color = voterColor;
            }
            iconObj.SetActive(true);
        }
    }
}
