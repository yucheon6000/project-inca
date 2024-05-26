using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : Character
{
    public static Player Instance { get; private set; }

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip hitAudioClip;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;
    }

    public override int Hit(int attckAmount)
    {
        if (audioSource != null)
            audioSource.PlayOneShot(hitAudioClip);

        return base.Hit(attckAmount);
    }

    protected override void OnDeath()
    {
        // throw new System.NotImplementedException();
    }
}
