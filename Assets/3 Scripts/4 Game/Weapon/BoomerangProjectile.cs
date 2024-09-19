using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class BoomerangProjectile : PlayerProjectile
{
    private enum State { Go, Back };
    private State state = State.Go;

    private new float MoveSpeed => state == State.Go ? Status.CurrentMoveSpeed * 2 : Status.CurrentMoveSpeed;

    [Header("[[Boomerang Projectile]]")]
    [SerializeField]
    private float defaultAttackDistance = 10f;

    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {
        base.Init(owner, moveDirection, target);
        state = State.Go;
    }

    private void Update()
    {
        if (state == State.Go)
        {
            // if (Target != null && Target.gameObject.activeSelf == true )
            // {
            //     transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, MoveSpeed * Time.deltaTime);
            //     if (Vector3.Distance(transform.position, Target.transform.position) <= 0.1f )
            //         state = State.Back;
            // }
            // else
            // {
            transform.position += MoveDirection.normalized * MoveSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, IncaData.UserRightHandPosition) >= defaultAttackDistance)
                state = State.Back;

            // }
        }

        else
        {
            transform.position = Vector3.MoveTowards(transform.position, IncaData.UserRightHandPosition, MoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, IncaData.UserRightHandPosition) <= 0.1f)
                MemoryPool.Instance(MemoryPoolType.Weapon).DeactivatePoolItem(this.gameObject);
            // transform.LookAt(IncaData.UserRightHandPosition, Vector3.up);
        }
    }
}
