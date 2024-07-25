using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : Character
{
    private DetectedObject detectedObject;

    [Header("Effects")]
    [SerializeField]
    protected GameObject spawnEffectPrefab;
    [SerializeField]
    protected GameObject dieEffectPrefab;

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
        {
            this.detectedObject = detectedObject;
            detectedObject.OnHideDetectedObject.AddListener(OnHideDetectedObject);
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0);
        }

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_IDLE);

        SpawnEffect(spawnEffectPrefab, detectedObject);

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

    int lastAnimationUpdateFrameCount = -1;
    Coroutine playDefaultAnimationRoutine = null;
    protected override void PlayAnimationByValue(int animationId)
    {
        if (animator == null) return;
        // if (lastAnimationUpdateFrameCount == Time.frameCount && animationId == Constants.Animation.ENEMY_ANIMATION_IDLE) return;

        animator.SetInteger(Constants.Animation.ENEMY_ANIMATION_ID, animationId);

        if (animationId != defaultAnimationId)
        {
            if (playDefaultAnimationRoutine != null)
                StopCoroutine(playDefaultAnimationRoutine);

            playDefaultAnimationRoutine = StartCoroutine(PlayDefaultAnimationRoutine());
        }

        // lastAnimationUpdateFrameCount = Time.frameCount;
    }

    int defaultAnimationId = 1;
    protected void SetDefaultAnimation(int animationId)
    {
        defaultAnimationId = animationId;
        PlayAnimationByValue(animationId);
    }

    private IEnumerator PlayDefaultAnimationRoutine()
    {
        yield return null;
        PlayAnimationByValue(defaultAnimationId);
    }

    protected void DeactivateGameObject()
    {
        MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
        SpawnEffect(dieEffectPrefab, detectedObject);
    }

    protected virtual void OnHideDetectedObject()
    {
        ForceKill();
        // MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
    }

    protected void SpawnEffect(GameObject effecPrefab, DetectedObject detectedObject = null)
    {
        if (effecPrefab == null) return;

        if (detectedObject == null)
        {
            if (transform.parent == null)
            {
                Instantiate(effecPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                GameObject clone = Instantiate(effecPrefab, transform.position, Quaternion.identity);
                clone.transform.SetParent(transform.parent.transform);
            }
        }
        else
        {
            Vector3 pos = detectedObject.Position;
            pos.y += detectedObject.Scale.y;
            GameObject clone = Instantiate(effecPrefab, pos, Quaternion.identity);
            clone.transform.SetParent(detectedObject.transform);
        }
    }

    public virtual void OnHoverStart() { }
    public virtual void OnHoverEnd() { }
}
