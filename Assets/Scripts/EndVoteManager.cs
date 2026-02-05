using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EndVoteManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private float _waitDuration = 5f;

    private void Start()
    {
        Time.timeScale = 1f; 
        ShowResult();
        


        // เงื่อนไขการรัน Timer: เป็น Master Client หรือเล่นคนเดียว/Offline
        bool isStandalone = !PhotonNetwork.IsConnectedAndReady || 
                            PhotonNetwork.CurrentRoom == null || 
                            PhotonNetwork.CurrentRoom.PlayerCount <= 1;

        if (PhotonNetwork.IsMasterClient || isStandalone)
        {

            Debug.Log("EndVoteManager: Starting Transition Coroutine...");
            StartCoroutine(WaitAndProceed());
        }
        else
        {

             Debug.Log("EndVoteManager: Client detected. Waiting for Master Client to change scene.");
        }
    }

    private void ShowResult()
    {
        if (_resultText == null) return;
        
        if (PhotonNetwork.CurrentRoom == null)
        {
            _resultText.text = "Offline Mode: Vote Ended.";
            return; 
        }

        bool isSkip = false;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Vote_IsSkip", out object skipVal))
        {
            isSkip = (bool)skipVal;
        }

        if (isSkip)
        {
            _resultText.text = "Vote Skipped\nNo one was ejected.";
        }
        else
        {
            int ejectedId = -1;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Vote_EjectedID", out object idVal))
            {
                ejectedId = (int)idVal;
            }

            Player ejectedPlayer = (ejectedId >= 0) ? PhotonNetwork.CurrentRoom.GetPlayer(ejectedId) : null;
            if (ejectedPlayer != null)
            {
                _resultText.text = $"{ejectedPlayer.NickName} was Ejected.";
            }
            else
            {
                _resultText.text = "No one was ejected (Inconclusive).";
            }
        }
    }

    private IEnumerator WaitAndProceed()
    {
        Debug.Log($"EndVoteManager: Waiting {_waitDuration} seconds...");
        yield return new WaitForSecondsRealtime(_waitDuration);
        


        // กรณี Offline ให้กลับเข้าเกมทันที
        if (PhotonNetwork.CurrentRoom == null)
        {
             SceneManager.LoadScene("GameScene");
             yield break;
        }

        string nextScene = "GameScene"; 

        try
        {
            int ejectedId = -1;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Vote_EjectedID", out object idVal))
                ejectedId = (int)idVal;

            // 1. เช็ค Madman Win (ถ้าคนที่โดนไล่ออกคือ Madman)
            if (ejectedId >= 0)
            {
                Player ejectedPlayer = PhotonNetwork.CurrentRoom.GetPlayer(ejectedId);
                if (ejectedPlayer != null && ejectedPlayer.CustomProperties.TryGetValue("role", out object rBox))
                {
                    int roleInt = System.Convert.ToInt32(rBox);
                    if (roleInt == (int)PlayerRole.Madman)
                    {
                        SetWinnerAndGo("Madman");
                        yield break;
                    }
                }
            }

            // 2. คำนวณจำนวนคนที่เหลือ
            int impostorCount = 0;
            int goodCount = 0;
            int sheriffAlive = 0;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                // ข้ามคนที่เพิ่งโดนโหวตออก
                if (ejectedId >= 0 && p.ActorNumber == ejectedId) continue;

                // ข้ามคนที่ตายอยู่แล้ว
                bool isDead = false;
                if (p.CustomProperties.TryGetValue("IsDead", out object d)) isDead = (bool)d;
                if (isDead) continue;

                // เช็คบทบาท
                if (p.CustomProperties.TryGetValue("role", out object r))
                {
                    int role = System.Convert.ToInt32(r);
                    if (role == (int)PlayerRole.Impostor) 
                    {
                        impostorCount++;
                    }
                    else 
                    {
                        goodCount++;
                        if (role == (int)PlayerRole.Sheriff) sheriffAlive++;
                    }
                }
                else
                {
                    goodCount++; // Default เป็นฝ่ายดีถ้ายังไม่มีบท
                }
            }

            // 3. ตัดสินผลแพ้ชนะ
            if (impostorCount >= goodCount && impostorCount > 0) 
            {
                SetWinnerAndGo("Impostor");
            }
            else if (impostorCount == 0) 
            {
                // Good Team Wins: Check if Sheriff is alive
                if (sheriffAlive > 0) SetWinnerAndGo("Sheriff");
                else SetWinnerAndGo("Civilian");
            }
            else
            {
                // Game continues
                Debug.Log("EndVoteManager: Game Continues... Loading GameScene.");
                PhotonNetwork.LoadLevel("GameScene");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EndVoteManager Error: {ex.Message}");
            PhotonNetwork.LoadLevel("GameScene"); // Fallback เพื่อไม่ให้ค้าง
        }
    }

    private void SetWinnerAndGo(string winner)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["Winner"] = winner;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        
        Debug.Log($"EndVoteManager: {winner} Wins! Loading WinScene...");
        PhotonNetwork.LoadLevel("WinScene");
    }
}