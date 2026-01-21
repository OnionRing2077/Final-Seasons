using UnityEngine;
using TMPro;

public class TaskPromptUI : MonoBehaviour
{
    public static TaskPromptUI Instance;
    public TMP_Text promptText;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string message)
    {
        promptText.text = message;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
