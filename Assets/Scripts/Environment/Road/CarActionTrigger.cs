using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarActionTrigger : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField]
    private bool triggerSpeedChange;
    [SerializeField]
    private float minTargetSpeed;
    [SerializeField]
    private float maxTargetSpeed;

    [Header("Lane")]
    [SerializeField]
    private bool triggerLaneChange;
    [SerializeField]
    private int minTargetLaneIndex;
    [SerializeField]
    private int maxTargetLaneIndex;

    private void OnTriggerEnter(Collider other)
    {
        print(other);

        if (!other.TryGetComponent<CarMovement>(out CarMovement carMovement)) return;

        if (triggerSpeedChange)
        {
            float targetMoveSpeed = Random.Range(minTargetSpeed, maxTargetSpeed);
            carMovement.ChangeMoveSpeed(targetMoveSpeed);
        }

        if (triggerLaneChange)
        {
            int targetLaneIndex = Random.Range(minTargetLaneIndex, maxTargetLaneIndex);
            carMovement.ChangeTargetLane(targetLaneIndex);
        }
    }
}
