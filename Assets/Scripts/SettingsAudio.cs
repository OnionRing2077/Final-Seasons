using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsAudio : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public TMP_Text masterValue;
    public TMP_Text musicValue;
    public TMP_Text sfxValue;

    void Start()
    {
        Load();
    }

    public void SetMaster(float value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        masterValue.text = Mathf.RoundToInt(value * 100).ToString();
        PlayerPrefs.SetFloat("Master", value);
    }

    public void SetMusic(float value)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        musicValue.text = Mathf.RoundToInt(value * 100).ToString();
        PlayerPrefs.SetFloat("Music", value);
    }

    public void SetSFX(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        sfxValue.text = Mathf.RoundToInt(value * 100).ToString();
        PlayerPrefs.SetFloat("SFX", value);
    }

    void Load()
    {
        float m = PlayerPrefs.GetFloat("Master", 1);
        float mu = PlayerPrefs.GetFloat("Music", 1);
        float s = PlayerPrefs.GetFloat("SFX", 1);

        masterSlider.value = m;
        musicSlider.value = mu;
        sfxSlider.value = s;

        SetMaster(m);
        SetMusic(mu);
        SetSFX(s);
    }

    public void ResetAll()
    {
        masterSlider.value = 1;
        musicSlider.value = 1;
        sfxSlider.value = 1;
        Load();
    }
}
