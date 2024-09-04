using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearProjectile : PlayerProjectile
{
    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        base.Init(owner, moveDirection, target);
        rigidbody.velocity = transform.forward * Status.CurrentMoveSpeed;
    }

    protected override void AfterAttack(Collider collider)
    {
        base.AfterAttack(collider);
        Destroy(this.gameObject);
    }
}
