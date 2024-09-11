using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    [Header("[[Player Weapon]]")]
    [SerializeField]
    protected bool setUserCarAsParent = true;

    [SerializeField]
    private Trail trailPrefab;

    [SerializeField]
    private Sprite aimSprite;

    protected override bool IsShootInputReceived()
        => IncaInput.GetButton(IncaButtonCode.RightTrigger);

    protected override void Shoot()
    {
        base.Shoot();

        // When the player is aiming at an enemy, it attacks the enemy directly without projectiles.
        if (IncaInput.TargetGameObject != null && IncaInput.TargetGameObject.TryGetComponent(out DamagableEnemy enemy))
        {
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.UseDirectAttack())
                {
                    Projectile clone = Instantiate(projectile, IncaData.UserRightHandPosition, Quaternion.LookRotation(IncaData.UserRightHandTrasnform.forward));
                    clone.DirectAttack(this, enemy, IncaInput.HitPoint);
                    Destroy(clone.gameObject);
                }
                // If this projectile doesn't support direct attack, it will be created.
                else
                    SpawnProjectile(projectile, enemy);
            }
        }

        // When the player is not aiming at an enemy, projectiles will be created.
        else
            foreach (Projectile projectile in projectiles)
                SpawnProjectile(projectile, null);
    }

    protected virtual void SpawnProjectile(Projectile projectile, Character target)
    {
        // Create new projectile game object.
        Projectile clone = Instantiate(projectile, IncaData.UserRightHandPosition, Quaternion.LookRotation(IncaData.UserRightHandTrasnform.forward));

        // Set the user car transform as parent of the projectile.
        if (setUserCarAsParent)
            clone.transform.SetParent(IncaData.UserCarTransform);

        // Initialize the projectile.
        clone.Init(this, IncaData.UserRightHandTrasnform.forward, target);
    }

    public void SetAim()
    {
        IncaInputManager.Instance.SetCursorSprite(aimSprite);
    }
}
