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
    [SerializeField]
    private bool lookAtUserCar = false;

    public void Look(bool value) => look = value;

    public void LookImmediate()
    {
        Look();
    }

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        if (look == false) return;

        if (lookAtPlayerTargetPoint)
            transform.LookAt(GGData.PlayerPosition);
        else if (lookAtPlayerHead)
            transform.LookAt(IncaData.UserHeadPosition);
        else if (lookAtUserCar)
            transform.LookAt(IncaData.UserCarPosition);
    }
}
