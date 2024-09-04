using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamagableEnemy : Enemy, IInteractable
{
    public virtual bool IsInteractable() => IsAlive;

    public virtual void OnClick() { }

    public virtual void OnHoverEnter() { }

    public virtual void OnHoverExit() { }
}
