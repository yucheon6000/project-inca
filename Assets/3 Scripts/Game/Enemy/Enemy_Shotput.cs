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

    protected override void Awake()
    {
        base.Awake();
        animator.Play("Jump");
        attackTimer = 0;
    }

    public void Init()
    {
        onInit.Invoke();
    }

    private void Update()
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
        Instantiate(bulletPrefab, bulletSpawnTransform.transform.position, Quaternion.LookRotation(IncaData.PlayerPosition));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
