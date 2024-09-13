using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class BoomProjectile : LinearProjectile
{
    [SerializeField]
    private LayerMask targetLayerMask;
    [SerializeField]
    private Transform modelTransform;

    [Header("[Explosion]")]
    [SerializeField]
    private float explosionRadius = 5f;
    [SerializeField]
    private float explosionTime;
    [SerializeField]
    private AnimationCurve explosionAttackCurve;
    [SerializeField]
    private GameObject explosionEffectPrefab;
    [SerializeField]
    private float explosionEffectVelocityScale = 0.1f;
    [SerializeField]
    private float groundY = 0.3f;

    private void Update()
    {
        modelTransform.rotation = Quaternion.LookRotation(rigidbody.velocity);

        if (transform.position.y < groundY)
        {
            AfterAttack(null);
        }
    }

    protected override void AfterAttack(Collider collider)
    {
        GameObject effectClone = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        effectClone.GetComponent<Rigidbody>().velocity = this.Velocity * explosionEffectVelocityScale;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, targetLayerMask);

        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out Enemy enemy) == false) continue;

            if (enemy.CanTakeDamage())
                enemy.TakeDamage(Status.CurrentAttack / 2f);
        }

        base.AfterAttack(collider);
    }
}
