using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    [Header("[[Player Weapon]]")]
    [SerializeField]
    protected bool setUserCarAsParentOfProjectiles = true;

    [SerializeField]
    protected float jitterRadius = 0;

    [SerializeField]
    private Sprite aimSprite;

    [SerializeField]
    private float vibrationTime = 0.1f;

    [SerializeField]
    private AudioClip shootClip;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected override bool IsShootInputReceived()
        => IncaInput.GetButton(IncaButtonCode.RightTrigger);

    protected override void Shoot()
    {
        base.Shoot();

        // When the player is aiming at an enemy, it attacks the enemy directly without projectiles.
        if (IncaInput.HasTarget && IncaInput.TargetGameObject.TryGetComponent(out DamagableEnemy enemy))
        {
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.UseDirectAttack())
                    DirectAttack(projectile, enemy);

                // If this projectile doesn't support direct attack, it will be created.
                else
                    SpawnProjectile(projectile, enemy);
            }
        }

        // When the player is not aiming at an enemy, projectiles will be created.
        else
            foreach (Projectile projectile in projectiles)
                SpawnProjectile(projectile, null);

        AfterShoot();
    }

    protected virtual void AfterShoot()
    {
        audioSource.PlayOneShot(shootClip);
        OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RHand);
        StartCoroutine(StopVibrationAfterTime(vibrationTime));
    }

    IEnumerator StopVibrationAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RHand); // 진동 중지
    }

    protected virtual void DirectAttack(Projectile projectile, Character target)
    {
        // Activate new projectile game object.
        Projectile clone = MemoryPool.Instance(MemoryPoolType.Weapon)
                                        .ActivatePoolItem(projectile.gameObject, IncaData.UserRightHandPosition, Quaternion.LookRotation(IncaData.UserRightHandTrasnform.forward))
                                        .GetComponent<Projectile>();
        // Attack target directly.
        clone.DirectAttack(this, target, IncaInput.HitPoint);

        // Deactivate the projectile game object.
        MemoryPool.Instance(MemoryPoolType.Weapon).DeactivatePoolItem(clone.gameObject);
    }

    protected virtual void SpawnProjectile(Projectile projectile, Character target)
    {
        // Create new projectile game object.
        Projectile clone = MemoryPool.Instance(MemoryPoolType.Weapon)
                                            .ActivatePoolItem(projectile.gameObject, IncaData.UserRightHandPosition, Quaternion.LookRotation(IncaData.UserRightHandTrasnform.forward))
                                            .GetComponent<Projectile>();

        // Set the user car transform as parent of the projectile.
        if (setUserCarAsParentOfProjectiles)
            clone.transform.SetParent(IncaData.UserCarTransform);

        // Initialize the projectile.
        clone.Init(this, GetJitteredDirection(), target);
    }

    public Vector3 GetJitteredDirection()
    {
        Vector3 targetPoint = IncaData.UserRightHandPosition
                                + IncaData.UserRightHandTrasnform.forward.normalized * IncaInputManager.Instance.DefaultCursorDistance
                                + Random.insideUnitSphere * jitterRadius;

        Vector3 result = (targetPoint - IncaData.UserRightHandPosition).normalized;

        return result;
    }

    private void SetAim()
    {
        IncaInputManager.Instance.SetCursorSprite(aimSprite);
    }

    public override void Install()
    {
        base.Install();
        SetAim();
    }

    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.color = Color.yellow;
            Vector3 targetPoint = IncaData.UserRightHandPosition
                                    + IncaData.UserRightHandTrasnform.forward.normalized * IncaInputManager.Instance.DefaultCursorDistance;
            Gizmos.DrawWireSphere(targetPoint, jitterRadius);
        }
        catch { }
    }
}
