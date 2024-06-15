using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterStatus))]
public abstract class Character : MonoBehaviour
{
    protected CharacterStatus status;
    public CharacterStatus Status => status;

    /// <summary>OnChangeCurrentHp(int currentHp, int previousHp)</summary>
    public UnityEvent<int, int> OnChangeCurrentHp => status.OnChangeCurrentHp;
    /// <summary>OnChangeCurrentAttack(int currentAttack, int previousAttack)</summary>
    public UnityEvent<int, int> OnChangeCurrentAttack => status.OnChangeCurrentAttack;
    public UnityEvent OnDeathEvent => status.OnDeath;

    [SerializeField]
    protected bool initOnAwake = false;

    protected virtual void Awake()
    {
        status = GetComponent<CharacterStatus>();

        if (initOnAwake)
            Init();
    }

    public virtual void Init()
    {
        status.Init();

        OnDeathEvent.AddListener(OnDeath);
    }

    public virtual int Hit(int attckAmount)
    {
        return status.IncreaseHp(-attckAmount);
    }

    protected abstract void OnDeath();
}
