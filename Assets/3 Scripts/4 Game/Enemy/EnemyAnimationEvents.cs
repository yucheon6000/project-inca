using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : CharacterAnimationEvents
{
    public override void Init()
    {
        ((Enemy)target).Init(null);
    }

    public void CanAttack()
    {
        ((Enemy)target).CanAttack(true);
    }

    public void FinishCurrentAnimation()
    {
        // ((Enemy)target).FinishCurrentAnimation();
    }
}
