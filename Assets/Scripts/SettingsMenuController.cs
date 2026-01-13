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
        settingUI.SetActive(false);   // เริ่มเกมซ่อน Settings
        ShowSound();
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
    }

    // ---------- Tabs ----------
    public void ShowSound()
    {
        panelSound.SetActive(true);
        panelControl.SetActive(false);
        panelExit.SetActive(false);
    }

    public void ShowControl()
    {
        panelSound.SetActive(false);
        panelControl.SetActive(true);
        panelExit.SetActive(false);
    }

    public void ShowExit()
    {
        panelSound.SetActive(false);
        panelControl.SetActive(false);
        panelExit.SetActive(true);
    }
}
