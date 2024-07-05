using UnityEngine;
using UnityEngine.Events;

public class CharacterStatus : MonoBehaviour
{
    [Header("Hp")]
    [SerializeField]
    private int maxHp;
    [SerializeField]
    private int defaultHp;
    [SerializeField]
    private int currentHp;
    public int CurrentHp => currentHp;

    [Header("Attack")]
    [SerializeField]
    private int maxAttack;
    [SerializeField]
    private int defaultAttack;
    [SerializeField]
    private int currentAttack;
    public int CurrentAttack => currentAttack;

    /// <summary>OnChangeCurrentHp(int currentHp, int previousHp)</summary>
    public UnityEvent<int, int> OnChangeCurrentHp { get; private set; }
    /// <summary>OnChangeCurrentAttack(int currentAttack, int previousAttack)</summary>
    public UnityEvent<int, int> OnChangeCurrentAttack { get; private set; }
    public UnityEvent OnDeath { get; private set; }

    public void Init()
    {
        // Set current values
        currentHp = Mathf.Min(maxHp, defaultHp);
        currentAttack = Mathf.Min(maxAttack, defaultAttack);

        // Reset UnityEvents.
        OnChangeCurrentHp = new UnityEvent<int, int>();
        OnChangeCurrentAttack = new UnityEvent<int, int>();
        OnDeath = new UnityEvent();
    }

    /// <returns>currentHp</returns>
    public int IncreaseHp(int amout)
    {
        int prevHp = currentHp;

        currentHp = Mathf.Clamp(currentHp + amout, 0, maxHp);

        OnChangeCurrentHp.Invoke(currentHp, prevHp);

        // When the character dies, call the character's method
        if (currentHp == 0)
            OnDeath.Invoke();

        return currentHp;
    }

    /// <returns>currentAttack</returns>
    public int IncreaseAttack(int amout)
    {
        int prevAttack = currentAttack;

        currentAttack = Mathf.Clamp(currentAttack + amout, 0, maxAttack);

        OnChangeCurrentAttack.Invoke(currentAttack, prevAttack);

        return currentAttack;
    }
}
