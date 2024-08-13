using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterStatus))]
public abstract class Character : MonoBehaviour
{
    protected static readonly string AUDIOCLIP_HIT = "Hit";

    protected CharacterStatus status;
    public CharacterStatus Status => status;

    public virtual bool IsAlive => status.CurrentHp > 0;
    public virtual bool IsDead => !IsAlive;

    /// <summary>OnChangeCurrentHp(float currentHp, float previousHp)</summary>
    public UnityEvent<float, float> OnChangeCurrentHp => status.OnChangeCurrentHp;
    /// <summary>OnChangeCurrentAttack(float currentAttack, float previousAttack)</summary>
    public UnityEvent<float, float> OnChangeCurrentAttack => status.OnChangeCurrentAttack;
    public UnityEvent OnDeathEvent => status.OnDeath;

    [Header("[Init]")]
    [SerializeField]
    protected bool initOnAwake = false;
    [SerializeField]
    protected bool initOnStart = false;

    [Header("[Animation]")]
    [SerializeField]
    protected Animator animator;

    [Header("[Audio]")]
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    private AudioStorage audioStorage;

    protected virtual void Awake()
    {
        status = GetComponent<CharacterStatus>();

        if (initOnAwake) Init();
    }

    protected virtual void Start()
    {
        if (initOnStart) Init();
    }

    public virtual void Init()
    {
        status.Init();

        OnDeathEvent.AddListener(OnDeath);
    }

    protected virtual void Attack() { }

    public virtual float TakeDamage(float damageAmount)
    {
        return status.IncreaseHp(-damageAmount);
    }

    protected abstract void OnDeath();

    protected virtual void PlayAnimationByValue(int animationValue) { }

    protected virtual void PlayAudioClip(AudioType audioType)
    {
        if (audioSource == null) return;

        AudioClip clip = audioStorage.FindAudioClip(audioType);
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }
}
