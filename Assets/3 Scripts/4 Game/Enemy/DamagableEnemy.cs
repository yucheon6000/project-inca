using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamagableEnemy : Enemy, InteractableObject
{
    public virtual bool IsInteractableType(InteractableType type)
    {
        if (IsDead)
            return false;

        return type == InteractableType.Hitable;
    }
}
