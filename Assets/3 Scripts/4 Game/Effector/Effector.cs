using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effector : MonoBehaviour
{
    protected Coroutine coroutine = null;
    public bool IsPlaying => coroutine != null;
}
