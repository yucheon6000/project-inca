using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    [SerializeField]
    protected Character target;

    public virtual void Init()
    {
        target.Init();
    }
}
