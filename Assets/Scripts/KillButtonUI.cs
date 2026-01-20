using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class KillButtonUI : MonoBehaviour
{
    [Header("UI")]
    public Button killButton;
    public Image cooldownFill;        // วง cooldown
    public TMP_Text cooldownText;     // ตัวเลข cooldown

    [Header("Refs")]
    public PlayerKill playerKill;

    void Awake()
    {
        if (killButton == null)
            killButton = GetComponent<Button>();
    }

    void Start()
    {
        // 🔎 หา PlayerKill อัตโนมัติ (กันลืมลากใส่)
        if (playerKill == null)
            playerKill = FindObjectOfType<PlayerKill>();

        if (killButton != null)
            killButton.onClick.AddListener(OnKillPressed);
    }

    void Update()
    {
        // ❗ กัน null ทุกกรณี
        if (playerKill == null) return;

        bool canKill = playerKill.CanKill();

        // เปิด/ปิดปุ่ม
        if (killButton)
            killButton.interactable = canKill;

        // ⏱ Cooldown UI
        float cd01 = playerKill.GetCooldown01();
        float cdLeft = playerKill.GetCooldownLeft();

        if (cooldownFill)
            cooldownFill.fillAmount = cd01;

        if (cooldownText)
            cooldownText.text = cdLeft > 0.01f
                ? Mathf.CeilToInt(cdLeft).ToString()
                : "";
    }

    void OnKillPressed()
    {
        if (playerKill == null) return;

        Debug.Log("KILL BUTTON CLICKED");
        playerKill.TryKill();
    }
}
