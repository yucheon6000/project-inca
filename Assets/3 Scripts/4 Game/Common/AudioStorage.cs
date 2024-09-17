using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioTypeForChracter
{
    Spawn,
    Idle,
    Move,
    Attack0, Attack1, Attack2, Attack3,
    TakeDamage0, TakeDamage1, TakeDamage2, TakeDamage3,
    Die,
}
[Serializable]
public class AudioStorageForCharacter : AudioStorage<AudioTypeForChracter> { }

[Serializable]
public class AudioStorage<T> where T : Enum
{
    [SerializeField]
    private List<AudioInformation<T>> audios;
    public int AudioCount => audios.Count;

    public AudioClip FindAudioClip(T type)
    {
        foreach (var audio in audios)
            if (audio.IsSameType(type))
                return audio.clip;

        return null;
    }

    public AudioClip GetAudioClip(int index)
    {
        if (index < 0 || index >= AudioCount) return null;

        return audios[index].clip;
    }
}

[Serializable]
public class AudioInformation<T> where T : Enum
{
    public T type;
    public AudioClip clip;

    public bool IsSameType(T type)
        => this.type.Equals(type);
}
