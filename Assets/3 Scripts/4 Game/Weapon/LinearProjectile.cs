using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class LinearProjectile : PlayerProjectile
{
    protected Trail trail;
    [SerializeField]
    protected float deactiveProjectileDelay = 0;

    protected override void Awake()
    {
        trail = GetComponentInChildren<Trail>();

        base.Awake();
    }

    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        base.Init(owner, moveDirection, target);

        rigidbody.velocity = IncaData.UserCarVelocity + moveDirection * Status.CurrentMoveSpeed;

        if (trail)
            trail.Init(this);

        if (deactiveProjectileDelay > 0)
            Invoke(nameof(DeactivateGameObject), deactiveProjectileDelay);
    }

    protected void DeactivateGameObject()
    {
        MemoryPool.Instance(MemoryPoolType.Weapon).DeactivatePoolItem(this.gameObject);
    }

    protected override void AfterAttack(Collider collider)
    {
        base.AfterAttack(collider);

        DeactivateGameObject();

        if (trail)
            trail.Finish();
    }

    protected virtual void OnDisable()
    {
        CancelInvoke();
    }
}
