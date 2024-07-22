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

    /// <summary>OnChangeCurrentHp(int currentHp, int previousHp)</summary>
    public UnityEvent<int, int> OnChangeCurrentHp => status.OnChangeCurrentHp;
    /// <summary>OnChangeCurrentAttack(int currentAttack, int previousAttack)</summary>
    public UnityEvent<int, int> OnChangeCurrentAttack => status.OnChangeCurrentAttack;
    public UnityEvent OnDeathEvent => status.OnDeath;

    [SerializeField]
    protected bool initOnAwake = false;

    [Header("Components")]
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected AudioSource audioSource;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioStorage audioStorage;

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

    public virtual int TakeDamage(int damageAmount)
    {
        return status.IncreaseHp(-damageAmount);
    }

    protected abstract void OnDeath();

    protected virtual void PlayAudioClip(AudioType audioType)
    {
        if (audioSource == null) return;

        AudioClip clip = audioStorage.FindAudioClip(audioType);
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }

    protected virtual void PlayAnimationByValue(int animationValue) { }

    public virtual void Attack() { }
}
