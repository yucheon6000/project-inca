using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    protected override bool CheckIfMyEnemy(Character other)
    {
        return other.TryGetComponent(out Enemy _);
    }
}
