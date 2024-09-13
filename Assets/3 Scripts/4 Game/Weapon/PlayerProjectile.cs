using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    private ScaleEffector scaleEffector;

    protected override void Awake()
    {
        base.Awake();
        scaleEffector = GetComponent<ScaleEffector>();
    }

    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        base.Init(owner, moveDirection, target);

        if (scaleEffector)
            scaleEffector.PlayFromZeroToOriginalScale(0.3f, ScaleEffector.EaseOutCurve);
    }

    protected override bool CheckIfMyEnemy(Character other)
    {
        return other.TryGetComponent(out Enemy _);
    }
}
