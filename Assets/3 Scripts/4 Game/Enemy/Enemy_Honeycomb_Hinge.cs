using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Honeycomb_Hinge : DamagableEnemy
{
    [SerializeField]
    private Enemy bear;

    [SerializeField]
    private HingeJoint hingeJoint;

    public override bool IsAlive => bear.IsDead && status.CurrentHp > 0;

    private void OnEnable()
    {
        Init();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        hingeJoint.breakForce = 0;
    }
}
