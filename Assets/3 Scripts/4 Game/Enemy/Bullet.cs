using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;

public class Bullet : NonDamagableEnemy
{
    [SerializeField]
    protected float moveSpeed;
    protected Vector3 dir;

    protected float attack;

    protected virtual void OnEnable()
    {
        dir = (GGData.PlayerPosition - transform.position).normalized;
        // Vector3 desiredVelocity = (GGData.PlayerPosition - transform.position).normalized * 30;
        // dir = desiredVelocity;
        transform.SetParent(GGData.PlayerTransform);
    }

    public virtual void Setup(Vector3 dir)
    {
        this.dir = dir.normalized;
    }

    public void SetAttack(float attack) => Status.SetAttack(attack);

    protected virtual void FixedUpdate()
    {

        // Vector3 steerForce = desiredVelocity - IncaData.PlayerVelocity;

        dir = (GGData.PlayerPosition - transform.position);
        dir.Normalize();

        transform.position += dir * moveSpeed * Time.deltaTime;

        if (hasReachedPlayer())
            HitPlayer();
    }

    protected bool hasReachedPlayer()
    {
        return Vector3.Distance(GGData.PlayerPosition, transform.position) <= 1;
    }

    protected virtual void HitPlayer()
    {
        // MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
        Player.Instance.TakeDamage(attack);
        Destroy(this.gameObject);
    }
}
