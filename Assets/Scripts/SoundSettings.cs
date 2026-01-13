using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSettings : MonoBehaviour
{
    public Slider masterSlider;
    public TMP_Text masterValue;

    void Start()
    {
        float v = PlayerPrefs.GetFloat("masterVol", 1f);
        masterSlider.value = v;
        ApplyMaster(v);
    }

    public void OnMasterChanged(float v)
    {
        ApplyMaster(v);
        PlayerPrefs.SetFloat("masterVol", v);
    }

    void ApplyMaster(float v)
    {
        AudioListener.volume = v;
        if (masterValue) masterValue.text = Mathf.RoundToInt(v * 100).ToString();
    }

    public void ResetDefaults()
    {
        masterSlider.value = 1f;
        OnMasterChanged(1f);
    }
}
