using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class BoomerangProjectile2 : LinearProjectile
{
    [SerializeField]
    private int bounceMaxCount = 1;
    private int bounceCurrentCount = 0;

    private Transform targetTf;

    TypeFinder enemyFinder;
    private List<GameObject> attackedGameObjects;

    protected override void Awake()
    {
        enemyFinder = GetComponentInChildren<TypeFinder>();
        attackedGameObjects = new List<GameObject>();

        base.Awake();
    }

    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        rigidbody.isKinematic = false;

        bounceCurrentCount = 0;
        attackedGameObjects.Clear();

        targetTf = null;

        base.Init(owner, moveDirection, target);
    }

    protected override void Attack(Character other)
    {
        attackedGameObjects.Add(other.gameObject);

        base.Attack(other);
    }

    private void Update()
    {
        if (bounceCurrentCount == 0) return;

        if (targetTf.gameObject.activeSelf == false) DeactivateGameObject();

        MoveDirection = (targetTf.position - transform.position).normalized;

        transform.position += MoveDirection * Status.CurrentMoveSpeed / 10f
                                * Vector3.Dot(IncaData.UserCarVelocity, MoveDirection)
                                * Time.deltaTime;
    }

    protected override void AfterAttack(Collider collider)
    {
        // base.AfterAttack(collider);

        SpawnAttackEffect(GetEffectSpawnPoint(collider));

        if (bounceCurrentCount == 0)
        {
            rigidbody.isKinematic = true;
        }

        bounceCurrentCount++;
        if (bounceCurrentCount > bounceMaxCount)
            DeactivateGameObject();

        GameObject target = enemyFinder.GetRandomGameObejectWithout(attackedGameObjects);

        if (target == null)
        {
            DeactivateGameObject();
            return;
        }

        targetTf = target.transform;
    }
}
