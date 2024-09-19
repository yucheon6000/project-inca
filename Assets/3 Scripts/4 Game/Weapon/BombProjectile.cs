using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class BombProjectile : LinearProjectile
{
    [SerializeField]
    private Transform modelTransform;

    [Header("[Explosion]")]
    [SerializeField]
    private BombExplosionProjectile bombExplosionProjectilePrefab;
    [SerializeField]
    private float groundY = 0.3f;
    private bool hasHitGound = false;

    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        base.Init(owner, moveDirection, target);
        hasHitGound = false;
    }

    private void Update()
    {
        modelTransform.rotation = Quaternion.LookRotation(rigidbody.velocity);

        if (transform.position.y < groundY)
        {
            hasHitGound = true;
            AfterAttack(null);
        }
    }

    protected override void AfterAttack(Collider collider)
    {
        Vector3 moveDir = rigidbody.velocity;
        if (hasHitGound)
            moveDir.y = 0;

        MemoryPool.Instance(MemoryPoolType.Weapon).ActivatePoolItem(bombExplosionProjectilePrefab.gameObject, transform.position, Quaternion.identity)
            .GetComponent<BombExplosionProjectile>()
            .Init(Owner, moveDir);

        base.AfterAttack(collider);
    }
}
