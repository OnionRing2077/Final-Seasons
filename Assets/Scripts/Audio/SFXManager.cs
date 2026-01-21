using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Kill")]
    public AudioClip killSFX;

    [Header("Vote")]
    public AudioClip voteSFX;

    [Header("Task")]
    public AudioClip taskSFX;

    AudioSource source;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    public void PlayKill()
    {
        if (killSFX != null)
            source.PlayOneShot(killSFX);
    }

    public void PlayVote()
    {
        if (voteSFX != null)
            source.PlayOneShot(voteSFX);
    }

    public void PlayTask()
    {
        if (taskSFX != null)
            source.PlayOneShot(taskSFX);
    }
}

