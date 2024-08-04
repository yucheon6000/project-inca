using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : CharacterStatus
{
    [Header("Attack Speed")]
    [SerializeField]
    private float maxAttackSpeed;
    [SerializeField]
    private float defaultAttackSpeed;
    [SerializeField]
    private float currentAttackSpeed;
    public float CurrentAttackSpeed => currentAttackSpeed;

    /// <summary>OnChangeCurrentAttackSpeed(float currentAttackSpeed, float previousAttackSpeed)</summary>
    public UnityEvent<float, float> OnChangeCurrentAttackSpeed { get; private set; }

    public override void Init()
    {
        base.Init();

        currentAttackSpeed = Mathf.Min(maxAttackSpeed, defaultAttackSpeed);

        OnChangeCurrentAttackSpeed = new UnityEvent<float, float>();
    }

    /// <returns>currentAttackSpeed</returns>
    public float IncreaseAttackSpeed(float amout)
    {
        float prevAttackSpeed = currentAttackSpeed;

        currentAttackSpeed = Mathf.Clamp(currentAttackSpeed + amout, 0, maxAttackSpeed);

        OnChangeCurrentAttackSpeed.Invoke(currentAttackSpeed, prevAttackSpeed);

        return currentAttackSpeed;
    }
}
