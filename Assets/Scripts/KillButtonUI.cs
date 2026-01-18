using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class KillButtonUI : MonoBehaviour
{
    [Header("UI")]
    public Button killButton;
    public Image cooldownFill;     // optional (ภาพ fill)
    public TMP_Text cooldownText;      // optional (ถ้าใช้ Text ธรรมดา)
    // ถ้าใช้ TMP ให้เปลี่ยนเป็น TMP_Text ได้

    [Header("Refs")]
    public PlayerKill playerKill;  // ตัวที่มี cooldown + request kill

    void Awake()
    {
        if (killButton == null) killButton = GetComponent<Button>();
    }

    void Start()
    {
        if (killButton != null)
            killButton.onClick.AddListener(OnKillPressed);
    }

    void Update()
{
    

    bool canKill = playerKill.CanKill();
    if (killButton) killButton.interactable = canKill;//canKill

    float cd01 = playerKill.GetCooldown01();
    float cdLeft = playerKill.GetCooldownLeft();

    if (cooldownFill) cooldownFill.fillAmount = cd01;
    if (cooldownText) cooldownText.text =
        cdLeft > 0.01f ? Mathf.CeilToInt(cdLeft).ToString() : "";
}


    void OnKillPressed()
    {   
        Debug.Log("KILL BUTTON CLICKED");
        if (playerKill == null) return;
        playerKill.TryKill(); // ให้ตัวระบบ PlayerKill เป็นคนจัดการ
    }

    

}
