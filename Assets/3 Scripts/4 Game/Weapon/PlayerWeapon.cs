using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    protected override bool CanShoot()
    {
        return base.CanShoot() && IncaInput.GetButton(IncaButtonCode.RightTrigger);
    }

    protected override void Shoot()
    {
        base.Shoot();

        if (IncaInput.TargetGameObject != null && IncaInput.TargetGameObject.TryGetComponent<DamagableEnemy>(out DamagableEnemy enemy))
        {
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.UseDirectAttack())
                {
                    Projectile clone = Instantiate(projectile, IncaData.UserRightHandPosition, Quaternion.LookRotation(IncaData.UserRightHandTrasnform.forward));
                    clone.DirectAttack(this, enemy, IncaInput.HitPoint);
                    Destroy(clone.gameObject);
                }
                else
                    SpawnProjectile(projectile, enemy);
            }
        }
        else
            foreach (Projectile projectile in projectiles)
                SpawnProjectile(projectile, null);
    }

    protected void SpawnProjectile(Projectile projectile, Character target)
    {
        Projectile clone = Instantiate(projectile, IncaData.UserRightHandPosition, Quaternion.LookRotation(IncaData.UserRightHandTrasnform.forward));
        clone.transform.SetParent(IncaData.UserCarTransform);
        clone.Init(this, IncaData.UserRightHandTrasnform.forward, target);
    }
}
