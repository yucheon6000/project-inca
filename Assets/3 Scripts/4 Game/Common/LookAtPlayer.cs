using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private bool look = true;
    [SerializeField]
    private bool lookAtPlayerTargetPoint = true;
    [SerializeField]
    private bool lookAtPlayerHead = false;

    public void Look(bool value) => look = value;

    private void FixedUpdate()
    {
        if (look == false) return;

        if (lookAtPlayerTargetPoint)
            transform.LookAt(GGData.PlayerPosition, Vector3.up);
        else if (lookAtPlayerHead)
            transform.LookAt(IncaData.UserHeadPosition, Vector3.up);
    }
}
