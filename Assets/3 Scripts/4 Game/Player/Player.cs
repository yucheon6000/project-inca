using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : Character
{
    public static Player Instance { get; private set; }

    public new PlayerStatus Status => (PlayerStatus)status;

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

    protected override void OnDeath()
    {
        // throw new System.NotImplementedException();
    }
}
