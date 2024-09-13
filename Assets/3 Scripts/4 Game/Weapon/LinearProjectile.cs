using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class LinearProjectile : PlayerProjectile
{
    protected Trail trail;

    protected override void Awake()
    {
        base.Awake();
        trail = GetComponentInChildren<Trail>();
    }

    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        base.Init(owner, moveDirection, target);
        rigidbody.velocity = IncaData.UserCarVelocity + moveDirection * Status.CurrentMoveSpeed;

        if (trail)
            trail.Init(this);

        Invoke(nameof(DeactivateGameObject), 10);
    }

    private void DeactivateGameObject()
    {
        MemoryPool.Instance(MemoryPoolType.Weapon).DeactivatePoolItem(this.gameObject);
    }

    protected override void AfterAttack(Collider collider)
    {
        base.AfterAttack(collider);
        MemoryPool.Instance(MemoryPoolType.Weapon).DeactivatePoolItem(this.gameObject);

        if (trail)
            trail.Finish();
    }
}
