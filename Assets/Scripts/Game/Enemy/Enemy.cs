using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyState { Idle, Attack, Die }

public abstract class Enemy : MonoBehaviour, InteractableObject
{
    private EnemyState state = EnemyState.Idle;

    [SerializeField]
    private int maxHp;
    [SerializeField]
    private int currentHp;
    [SerializeField]
    protected bool isDead = false;

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    private UnityEvent onDie = new UnityEvent();

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip hitAudioClip;
    [SerializeField]
    private AudioClip dieAudioClip;

    public virtual void Setup(DetectedObject detectedObject)
    {
        detectedObject?.RegisterOnHideAction(() => Destroy(gameObject));
    }

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    public virtual void Hit(int power)
    {
        if (isDead) return;

        currentHp -= power;

        audioSource.PlayOneShot(hitAudioClip);

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        animator?.Play(Constants.animation_enemy_hit);
    }

    protected virtual void Die()
    {
        isDead = true;
        if (animator != null) animator?.Play(Constants.animation_enemy_death);
        audioSource?.PlayOneShot(dieAudioClip);

        onDie.Invoke();
    }

    public virtual bool IsInteractableType(InteractableType type)
    {
        return type == InteractableType.Hitable;
    }

    public virtual void OnHoverStart() { }
    public virtual void OnHoverEnd() { }
}
