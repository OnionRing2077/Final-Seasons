using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Root")]
    public GameObject settingUI;   // ตัวครอบทั้งหน้าต่าง

    [Header("Panels")]
    public GameObject panelSound;
    public GameObject panelControl;
    public GameObject panelExit;

    void Start()
    {
        // 🛠 Fix: Ensure everything is hidden at start
        CloseSettings();
    }

    // ---------- OPEN / CLOSE ----------
    public void OpenSettings()
    {
        settingUI.SetActive(true);
        ShowSound();   // เปิดหน้า Sound เป็นค่าเริ่มต้น
    }

    public void CloseSettings()
    {
        settingUI.SetActive(false);
        
        // Optional: Hide all internal panels too to be safe
        if(panelSound) panelSound.SetActive(false);
        if(panelControl) panelControl.SetActive(false);
        if(panelExit) panelExit.SetActive(false);
    }

    // ---------- Tabs ----------
    public void ShowSound()
    {
        if(panelSound) panelSound.SetActive(true);
        if(panelControl) panelControl.SetActive(false);
        if(panelExit) panelExit.SetActive(false);
    }

    public void ShowControl()
    {
        if(panelSound) panelSound.SetActive(false);
        if(panelControl) panelControl.SetActive(true);
        if(panelExit) panelExit.SetActive(false);
    }

    public void ShowExit()
    {
        if(panelSound) panelSound.SetActive(false);
        if(panelControl) panelControl.SetActive(false);
        if(panelExit) panelExit.SetActive(true);
    }
}
