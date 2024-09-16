using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosionProjectile : LinearProjectile
{
    private ParticleSystem particle;

    protected override void Awake()
    {
        base.Awake();
        particle = GetComponentInChildren<ParticleSystem>();
    }

    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        base.Init(owner, moveDirection, target);
    }

    protected override void AfterAttack(Collider collider)
    {
        //base.AfterAttack(collider);   // Prevent to deactive this game object after first attck.
    }
}
