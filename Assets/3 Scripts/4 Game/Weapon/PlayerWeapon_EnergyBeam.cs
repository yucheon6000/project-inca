using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class PlayerWeapon_EnergyBeam : PlayerWeapon
{
    protected override bool IsShootInputReceived()
        => Input.GetKeyDown(KeyCode.Z); // && IncaInput.GetButtonDown(IncaButtonCode.B);

    protected override void SpawnProjectile(Projectile projectile, Character target)
    {
        // base.SpawnProjectile(projectile, target);
        // Create new projectile game object.
        Projectile clone = Instantiate(projectile, IncaData.UserCarTransform.position, Quaternion.identity);

        // Set the user car transform as parent of the projectile.
        if (setUserCarAsParent)
            clone.transform.SetParent(IncaData.UserCarTransform);

        // Initialize the projectile.
        clone.Init(this, IncaData.UserRightHandTrasnform.forward, target);
    }
}
