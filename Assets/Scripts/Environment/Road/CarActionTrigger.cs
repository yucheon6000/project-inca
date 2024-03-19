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

        if (other.TryGetComponent<CarStateDrive>(out CarStateDrive carStateDrive) && triggerSpeedChange)
        {
            float targetMoveSpeed = Random.Range(minTargetSpeed, maxTargetSpeed);
            carStateDrive.ChangeMoveSpeed(targetMoveSpeed);
        }

        if (other.TryGetComponent<Car>(out Car car) && triggerLaneChange)
        {
            print("aaa");
            int targetLaneIndex = Random.Range(minTargetLaneIndex, maxTargetLaneIndex);
            car.ChangeTargetLane(targetLaneIndex);
        }
    }
}
