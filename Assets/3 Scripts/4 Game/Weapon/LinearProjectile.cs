using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class LinearProjectile : PlayerProjectile
{
    [SerializeField]
    protected Trail trailPrefab;

    protected Trail trail;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        base.Init(owner, moveDirection, target);
        rigidbody.velocity = IncaData.UserCarVelocity + moveDirection * Status.CurrentMoveSpeed;

        if (trailPrefab != null)
        {
            trail = Instantiate(trailPrefab);
            trail.Init(this);
        }
    }

    protected override void AfterAttack(Collider collider)
    {
        base.AfterAttack(collider);
        Destroy(this.gameObject);

        if (trail != null)
            trail.Finish();
    }
}
