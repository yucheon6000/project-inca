using UnityEngine;
using UnityEngine.Events;

public class CharacterStatus : MonoBehaviour
{
    [Header("Hp")]
    [SerializeField]
    private float maxHp;
    public float MaxHp => maxHp;
    [SerializeField]
    private float defaultHp;
    public float DefaultHp => defaultHp;
    [SerializeField]
    private float currentHp;
    public float CurrentHp => currentHp;

    [Header("Attack Power")]
    [SerializeField]
    private float maxAttack;
    [SerializeField]
    private float defaultAttack;
    [SerializeField]
    private float currentAttack;
    public float CurrentAttack => currentAttack;

    /// <summary>OnChangeCurrentHp(float currentHp, float previousHp)</summary>
    public UnityEvent<float, float> OnChangeCurrentHp { get; private set; }
    /// <summary>OnChangeCurrentAttack(float currentAttack, float previousAttack)</summary>
    public UnityEvent<float, float> OnChangeCurrentAttack { get; private set; }
    public UnityEvent OnDeath { get; private set; }

    public virtual void Init()
    {
        // Set current values
        currentHp = Mathf.Min(maxHp, defaultHp);
        currentAttack = Mathf.Min(maxAttack, defaultAttack);

        // Reset UnityEvents.
        OnChangeCurrentHp = new UnityEvent<float, float>();
        OnChangeCurrentAttack = new UnityEvent<float, float>();
        OnDeath = new UnityEvent();
    }

    /// <returns>maxHp</returns>
    public float IncreaseMaxHp(float amount)
    {
        maxHp += amount;

        return maxHp;
    }

    /// <returns>currentHp</returns>
    public float IncreaseHp(float amout)
    {
        float prevHp = currentHp;

        currentHp = Mathf.Min(currentHp + amout, maxHp);

        // When the character dies, call the character's method
        if (currentHp <= 0)
        {
            currentHp = 0;
            OnDeath.Invoke();
        }

        OnChangeCurrentHp.Invoke(currentHp, prevHp);

        return currentHp;
    }

    public float SetAttack(float value)
    {
        float prevAttack = currentAttack;

        currentAttack = Mathf.Clamp(value, 0, maxAttack);

        OnChangeCurrentAttack.Invoke(currentAttack, prevAttack);

        return currentAttack;
    }

    /// <returns>currentAttack</returns>
    public float IncreaseAttack(float amout)
    {
        float prevAttack = currentAttack;

        currentAttack = Mathf.Clamp(currentAttack + amout, 0, maxAttack);

        OnChangeCurrentAttack.Invoke(currentAttack, prevAttack);

        return currentAttack;
    }
}
