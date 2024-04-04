using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : MonoBehaviour, InteractableObject
{
    [SerializeField]
    private int maxHp;
    [SerializeField]
    private int currentHp;
    [SerializeField]
    private bool isDead = false;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private UnityEvent onDie = new UnityEvent();

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

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        animator.Play(Constants.animation_enemy_hit);
    }

    protected virtual void Die()
    {
        isDead = true;
        animator.Play(Constants.animation_enemy_death);

        onDie.Invoke();
    }

    public virtual bool IsInteractableType(InteractableType type)
    {
        throw new NotImplementedException();
    }
}
