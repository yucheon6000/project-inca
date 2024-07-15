using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyState { Idle, Move, Attack, Die }

public abstract class Enemy : Character
{
    protected EnemyState state = EnemyState.Idle;

    protected override void Awake()
    {
        base.Awake();

        if (initOnAwake)
            Init(null);
    }

    [ContextMenu("Init")]
    private void InitWithoutDetectedObject()
    {
        Init(null);
    }

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

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_IDLE);

        base.Init();
    }

    public override void Attack()
    {
        base.Attack();

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_ATTACK);
    }

    public override int TakeDamage(int attckAmount)
    {
        int curHp = base.TakeDamage(attckAmount);

        if (IsDead)
        {
            return 0;
        }
        else
        {
            PlayAudioClip(AudioType.TakeDamage0);
            PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_TAKE_DAMAGE);
        }

        return curHp;
    }

    public void ForceKill()
    {
        base.TakeDamage(status.CurrentHp);     // => Call OnDeath method
    }

    protected override void OnDeath()
    {
        PlayAudioClip(AudioType.Die);

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_DIE);
    }

    protected override void PlayAnimationByValue(int animationValue)
    {
        if (animator == null) return;
        animator.SetInteger(Constants.Animation.ENEMY_ANIMATION_ID, -1);
        animator.SetInteger(Constants.Animation.ENEMY_ANIMATION_ID, animationValue);
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

    public virtual void OnHoverStart() { }
    public virtual void OnHoverEnd() { }
}
