using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player instance;
    public static Player Instance => instance;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip hitAudioClip;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Hit()
    {
        audioSource.PlayOneShot(hitAudioClip);
    }
}
