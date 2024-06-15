using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Shotput : Enemy
{
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private float attackTimeMin;
    [SerializeField]
    private float attackTimeMax;
    [SerializeField]
    private float attackTime;
    private float attackTimer;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletSpawnTransform;

    [Space]
    [SerializeField]
    private UnityEvent onInit = new UnityEvent();

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);

        attackTime = Random.Range(attackTimeMin, attackTimeMax);
        attackTimer = 0;

        animator.SetInteger("animation", 1);

        onInit.Invoke();
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        transform.LookAt(IncaData.PlayerPosition, Vector3.up);

        if (Vector3.Distance(transform.position, IncaData.PlayerPosition) > attackDistance) return;

        attackTimer += Time.deltaTime;
        if (attackTimer > attackTime)
        {
            attackTime = Random.Range(attackTimeMin, attackTimeMax);
            attackTimer = 0;
            animator.Play("Attack");
            Attack();
        }
    }

    public void Attack()
    {
        if (IsDead) return;

        GameObject bulletClone = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bulletPrefab);
        bulletClone.transform.SetPositionAndRotation(transform.position, Quaternion.LookRotation(IncaData.PlayerPosition));
        Vector3 dir = (IncaData.PlayerPosition - bulletSpawnTransform.position);
        bulletClone.GetComponent<Bullet>().Setup(dir);
    }

    public override int Hit(int attckAmount)
    {
        int curHp = base.Hit(attckAmount);
        if (IsDead) return curHp;

        animator.Play("Damage");

        return curHp;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        animator.SetInteger("animation", 5);

        Invoke(nameof(DeactivateGameObject), 2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
