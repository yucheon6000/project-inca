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
        if (IncaInput.Target != null)
            target = IncaInput.TargetGameObject.transform;

        moveDirection = IncaData.UserRightHandTrasnform.forward;
    }

    private void Update()
    {
        if (state == State.Go)
        {
            if (target != null && target.gameObject.activeSelf == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, target.position) <= 0.1f)
                    state = State.Back;
                // transform.LookAt(target.position, Vector3.up);
            }
            else if (target != null && target.gameObject.activeSelf == false)
            {
                state = State.Back;
            }
            else
            {
                transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
                if (Vector3.Distance(transform.position, IncaData.UserRightHandPosition) >= defaultAttackDistance)
                    state = State.Back;

                // transform.LookAt(transform.position + moveDirection, Vector3.up);
            }
        }

        else
        {
            transform.position = Vector3.MoveTowards(transform.position, IncaData.UserRightHandPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, IncaData.UserRightHandPosition) <= 0.1f)
                gameObject.SetActive(false);
            // transform.LookAt(IncaData.UserRightHandPosition, Vector3.up);
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
