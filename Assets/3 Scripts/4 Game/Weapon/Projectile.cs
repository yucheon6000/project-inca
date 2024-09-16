using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ProjectileStatus))]
public abstract class Projectile : MonoBehaviour
{
    protected Weapon Owner { get; private set; }
    public ProjectileStatus Status { get; private set; }

    protected Character Target { get; private set; }
    public Vector3 MoveDirection { get; protected set; }
    protected float MoveSpeed => Status.CurrentMoveSpeed;

    [Header("[Direct Attack (Hitscan)]")]
    [SerializeField]
    private bool useDirectAttack;
    public bool UseDirectAttack() => useDirectAttack;
    [SerializeField]
    private GameObject directAttackEffect;

    [Header("[Effect]")]
    [SerializeField]
    private GameObject attackEffect;

    protected new Rigidbody rigidbody;

    public float Speed => rigidbody.velocity.magnitude;
    public Vector3 Velocity => rigidbody.velocity;

    protected virtual void Awake()
    {
        Status = GetComponent<ProjectileStatus>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        Owner = owner;
        Target = target;
        MoveDirection = moveDirection;

        Status.Init(owner.Status);
    }

    public virtual void DirectAttack(Weapon owner, Character target, Vector3 hitPoint)
    {
        if (useDirectAttack == false) return;

        Status.Init(owner.Status);

        target.TakeDamage(Status.CurrentAttack);
        Instantiate(directAttackEffect, hitPoint, Quaternion.identity);
    }

    protected abstract bool CheckIfMyEnemy(Character other);

    protected virtual void Attack(Character other)
    {
        other.TakeDamage(Status.CurrentAttack);
    }

    protected virtual void AfterAttack(Collider collider)
    {
        Vector3 effectSpawnPoint = GetEffectSpawnPoint(collider);
        SpawnAttackEffect(effectSpawnPoint);
    }

    protected Vector3 GetEffectSpawnPoint(Collider collider)
    {
        return collider != null ? collider.ClosestPointOnBounds(transform.position) : transform.position;
    }

    protected void SpawnAttackEffect(Vector3 spawnPoint)
    {
        if (attackEffect)
            Instantiate(attackEffect, spawnPoint, Quaternion.identity);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character character) == false) return;

        if (CheckIfMyEnemy(character) == false) return;

        Attack(character);
        AfterAttack(other);
    }
}
