using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private bool look = true;

    public void Look(bool value) => look = value;

    private void FixedUpdate()
    {
        if (look == false) return;

        transform.LookAt(IncaData.PlayerPosition, Vector3.up);
    }
}
