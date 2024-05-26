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

        animator.Play("Jump");
        attackTimer = 0;

        onInit.Invoke();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (Vector3.Distance(transform.position, IncaData.PlayerPosition) > attackDistance) return;

        transform.LookAt(IncaData.PlayerPosition, Vector3.up);

        attackTimer += Time.deltaTime;
        if (attackTimer > attackTime)
        {
            attackTimer = 0;
            animator.Play("Attack");
        }
    }

    public void Attack()
    {
        GameObject bulletClone = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bulletPrefab);
        bulletClone.transform.SetPositionAndRotation(transform.position, Quaternion.LookRotation(IncaData.PlayerPosition));
        Vector3 dir = (IncaData.PlayerPosition - bulletSpawnTransform.position);
        bulletClone.GetComponent<Bullet>().Setup(dir);
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        Invoke(nameof(DeactivateGameObject), 1);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
