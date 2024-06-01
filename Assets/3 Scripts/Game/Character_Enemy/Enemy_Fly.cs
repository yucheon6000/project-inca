using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Fly : Enemy
{
    private StateMachine<Enemy_Fly> stateMachine;

    [SerializeField]
    private State<Enemy_Fly> flyingState;

    private void Start()
    {
        stateMachine = new StateMachine<Enemy_Fly>();
        stateMachine.Setup(this, flyingState);
    }

    private void FixedUpdate()
    {
        stateMachine.Execute();
    }
}
