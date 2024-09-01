using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class BoomerangProjectile : Projectile
{
    private enum State { Go, Back };

    private Transform target;

    private State state = State.Go;

    [SerializeField]
    private float moveSpeed;
    private Vector3 moveDirection;
    [SerializeField]
    private float defaultAttackDistance = 10f;

    private void OnEnable()
    {
        target = IncaInput.TargetGameObject.transform;
        moveDirection = IncaData.UserRightHandTrasnform.forward;
    }

    private void Update()
    {
        if (state == State.Go)
        {
            if (target != null && target.gameObject.activeSelf == true)
            {
                transform.position += (target.position - transform.position).normalized * moveSpeed * Time.deltaTime;
                if (Vector3.Distance(transform.position, target.position) <= 0.1f)
                    state = State.Back;
            }
            else
            {
                transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
                if (Vector3.Distance(transform.position, IncaData.UserRightHandPosition) >= defaultAttackDistance)
                    state = State.Back;
            }
        }

        else
        {
            transform.position += (IncaData.UserRightHandPosition - transform.position).normalized * moveSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, IncaData.UserRightHandPosition) <= 1)
                gameObject.SetActive(false);
        }
    }

    protected override void Attack(Character target)
    {
        target.TakeDamage(1);
    }

    protected override bool ThisIsMyEnemy(Character character)
    {
        return true;
    }
}
