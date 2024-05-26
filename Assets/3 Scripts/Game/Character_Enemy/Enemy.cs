using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyState { Idle, Move, Attack, Die }

public abstract class Enemy : Character, InteractableObject
{
    protected EnemyState state = EnemyState.Idle;

    [SerializeField]
    protected bool isDead = false;

    [SerializeField]
    protected Animator animator;

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
    public virtual void Init(DetectedObject detectedObject = null)
    {
        if (detectedObject != null)
            detectedObject.RegisterOnHideAction(() => DeactivateGameObject());

        base.Init();
    }

    public override int Hit(int attckAmount)
    {
        int curHp = base.Hit(attckAmount);

        if (isDead) return 0;

        if (audioSource != null)
            audioSource.PlayOneShot(hitAudioClip);

        if (animator != null)
            animator.Play(Constants.animation_enemy_hit);

        return curHp;
    }

    protected override void OnDeath()
    {
        isDead = true;

        if (animator != null)
            animator.Play(Constants.animation_enemy_death);

        if (audioSource != null)
            audioSource.PlayOneShot(dieAudioClip);
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
