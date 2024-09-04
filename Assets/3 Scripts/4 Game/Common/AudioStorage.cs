using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioStorage
{
    [SerializeField]
    private List<AudioInformation> audios;

    public AudioClip FindAudioClip(AudioType type)
    {
        foreach (AudioInformation audio in audios)
        {
            if (audio.type == type) return audio.clip;
        }

        return null;
    }
}

public enum AudioType
{
    Spawn,
    Idle,
    Move,
    Attack0, Attack1, Attack2, Attack3,
    TakeDamage0, TakeDamage1, TakeDamage2, TakeDamage3,
    Die,
}

[System.Serializable]
public class AudioInformation
{
    public AudioType type;
    public AudioClip clip;
}
