using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class BoomProjectile : LinearProjectile
{
    [SerializeField]
    private LayerMask targetLayerMask;
    [SerializeField]
    private GameObject boomEffect;

    [Header("[Explosion]")]
    [SerializeField]
    private float explosionTime;
    [SerializeField]
    private AnimationCurve explosionAttackCurve;

    protected override void AfterAttack(Collider collider)
    {
        boomEffect.SetActive(true);
        boomEffect.transform.SetParent(null);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f, targetLayerMask);
        print(colliders.Length);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out Enemy enemy) == false) continue;
            enemy.TakeDamage(Status.CurrentAttack / 2f);
        }

        base.AfterAttack(collider);
    }
}
