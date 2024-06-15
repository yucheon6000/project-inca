using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Fly : Enemy
{
    private StateMachine<Enemy_Fly> stateMachine;

    [SerializeField]
    private State<Enemy_Fly> flyingState;

    [SerializeField]
    private Transform modelTransform;

    private Vector3 originLocalPosition;
    private Quaternion originLocalRotation;

    protected override void Awake()
    {
        base.Awake();

        originLocalPosition = transform.localPosition;
        originLocalRotation = transform.localRotation;
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);

        transform.localPosition = originLocalPosition;
        transform.localRotation = originLocalRotation;

        gameObject.SetActive(true);

        stateMachine = new StateMachine<Enemy_Fly>();
        stateMachine.Setup(this, flyingState);
        animator.SetInteger("animation", 2);
    }

    private void FixedUpdate()
    {
        stateMachine.Execute();
    }

    public void AttackPlayer()
    {
        Player.Instance.Hit(Status.CurrentAttack);
        ForceKill();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        gameObject.SetActive(false);
    }

    public void LookAt(Vector3 direction)
    {
        modelTransform.rotation = Quaternion.LookRotation(direction);
    }
}
