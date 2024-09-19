using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialAudioType { None }

public class TutorialManager : MonoBehaviour
{
    [Header("[Animation]")]
    [SerializeField]
    private string animationParamName;

    [Header("[Audio]")]
    [SerializeField]
    private AudioStorage<TutorialAudioType> audioStorage;
    [SerializeField]
    private int currentAudioIndex = -1;

    private Animator animator;
    private AudioSource audioSource;

    [Header("[Enemy]")]
    [SerializeField]
    private List<Transform> enemySapwnTransforms = new List<Transform>();

    [Header("[Event]")]
    [SerializeField]
    private List<EventWithDescription> events = new List<EventWithDescription>();
    [SerializeField]
    private int currentEventIndex = -1;

    [Space]
    [SerializeField]
    private UnityEvent onKillOneDummyEnemy = new UnityEvent();
    [SerializeField]
    private UnityEvent onSelectAbilityCard = new UnityEvent();
    [SerializeField]
    private UnityEvent onKillMultiDummyEnemies = new UnityEvent();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayNextAudio()
    {
        currentAudioIndex++;
        AudioClip clip = audioStorage.GetAudioClip(currentAudioIndex);
        audioSource.PlayOneShot(clip);
    }

    public void PlayNextAnimation()
    {
        int value = animator.GetInteger(animationParamName) + 1;
        animator.SetInteger(animationParamName, value);
    }

    public void InvokeNextEvent()
    {
        currentEventIndex++;
        if (currentEventIndex >= events.Count) return;

        events[currentEventIndex].events.Invoke();
    }

    public void SpawnOneDummyEnemy()
    {

    }

    public void SpawnMultiDummyEnemies()
    {

    }
}

[Serializable]
public class EventWithDescription
{
    public string description;
    public UnityEvent events;
}
