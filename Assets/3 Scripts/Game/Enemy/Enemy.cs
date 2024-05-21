using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyState { Idle, Move, Attack, Die }

public abstract class Enemy : MonoBehaviour, InteractableObject
{
    protected EnemyState state = EnemyState.Idle;

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

    /// <summary>
    /// When this enemy is spawned by EnemySpawner, this method is called firstly.
    /// </summary>
    /// <param name="detectedObject"></param>
    public virtual void Setup(DetectedObject detectedObject)
    {
        detectedObject?.RegisterOnHideAction(() => DeactivateGameObject());
        currentHp = maxHp;
    }

    public virtual void Hit(int power)
    {
        if (isDead) return;

        currentHp -= power;

        if (audioSource != null)
            audioSource.PlayOneShot(hitAudioClip);

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (animator != null)
            animator.Play(Constants.animation_enemy_hit);
    }

    protected virtual void Die()
    {
        isDead = true;

        if (animator != null)
            animator.Play(Constants.animation_enemy_death);

        if (audioSource != null)
            audioSource.PlayOneShot(dieAudioClip);

        onDie.Invoke();
    }

    protected void DeactivateGameObject()
    {
        MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
    }

    public virtual bool IsInteractableType(InteractableType type)
    {
        return type == InteractableType.Hitable;
    }

    public virtual void OnHoverStart() { }
    public virtual void OnHoverEnd() { }
}
