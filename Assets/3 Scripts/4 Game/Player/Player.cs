using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : Character
{
    public static Player Instance { get; private set; }

    public new PlayerStatus Status => (PlayerStatus)status;

    [Header("[[Player]]")]
    [Header("[Score]")]
    [SerializeField]
    private int score = 0;
    [SerializeField]
    private int deathPenaltyScore = -1000;

    [SerializeField]
    private AudioClip hitAudioClip;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;
    }

    public override float TakeDamage(float attackAmount)
    {
        if (audioSource != null)
            audioSource.PlayOneShot(hitAudioClip);

        return base.TakeDamage(attackAmount);
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        score = Mathf.Max(0, score);
    }

    protected override void OnDeath()
    {
        IncreaseScore(deathPenaltyScore);
        Status.IncreaseHp(Status.DefaultHp);
    }
}
