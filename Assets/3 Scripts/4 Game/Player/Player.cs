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

    [Header("[Glass Effect]")]
    [SerializeField]
    private AlphaEffector glassColorEffector;
    private EmissionEffector glassEmissionEffector;
    [SerializeField]
    private float glassEffectTime;
    [SerializeField]
    private float glassEffectStartAlpha;
    [SerializeField]
    private float glassEffectEndAlpha;
    [SerializeField]
    private float glassEffectStartEmission;
    [SerializeField]
    private float glassEffectEndEmission;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;

        glassEmissionEffector = glassColorEffector.GetComponent<EmissionEffector>();
    }

    public override float TakeDamage(float attackAmount)
    {
        if (audioSource != null)
            audioSource.PlayOneShot(hitAudioClip);

        glassColorEffector.gameObject.SetActive(true);
        glassEmissionEffector.Play(glassEffectTime, glassEffectStartEmission, glassEffectEndEmission);
        glassColorEffector.Play(glassEffectTime, glassEffectStartAlpha, glassEffectEndAlpha, () =>
        {
            glassColorEffector.gameObject.SetActive(false);
        });


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
