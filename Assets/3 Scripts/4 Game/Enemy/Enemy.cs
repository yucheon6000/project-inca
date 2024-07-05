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

    /// <summary>
    /// When this enemy is spawned by EnemySpawner, this method is called firstly.
    /// </summary>
    /// <param name="detectedObject"></param>
    public virtual void Init(DetectedObject detectedObject = null)
    {
        // If the detectedObject is hiden, call OnHideDetectedObject method.
        // Basically, OnHideDetectedObject call ForceKill method.
        if (detectedObject != null)
            detectedObject.RegisterOnHideAction(OnHideDetectedObject);

        base.Init();
    }

    public override int TakeDamage(int attckAmount)
    {
        int curHp = base.TakeDamage(attckAmount);

        if (IsDead) return 0;

        PlayAudioClip(AudioType.TakeDamage0);

        if (animator != null)
            animator.Play(Constants.animation_enemy_hit);

        return curHp;
    }

    public void ForceKill()
    {
        base.TakeDamage(status.CurrentHp);     // => Call OnDeath method
    }

    protected override void OnDeath()
    {
        PlayAudioClip(AudioType.Die);

        if (animator != null)
            animator.Play(Constants.animation_enemy_death);
    }

    protected void DeactivateGameObject()
    {
        MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
    }

    protected virtual void OnHideDetectedObject()
    {
        ForceKill();
        // MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
    }

    public virtual bool IsInteractableType(InteractableType type)
    {
        if (IsDead)
            return false;

        return type == InteractableType.Hitable;
    }

    public virtual void OnHoverStart() { }
    public virtual void OnHoverEnd() { }
}
