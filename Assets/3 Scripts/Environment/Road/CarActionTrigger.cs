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

    [Header("Traffic Light")]
    [SerializeField]
    private bool triggerTrafficLightWait;
    [SerializeField]
    private bool trafficLightGo;
    [SerializeField]
    private bool trafficLightStop;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarStateDrive>(out CarStateDrive carStateDrive) && triggerSpeedChange)
        {
            float targetMoveSpeed = Random.Range(minTargetSpeed, maxTargetSpeed);
            carStateDrive.ChangeMoveSpeed(targetMoveSpeed);
        }

        if (other.TryGetComponent<Car>(out Car car))
        {
            if (triggerLaneChange)
            {
                int targetLaneIndex = Random.Range(minTargetLaneIndex, maxTargetLaneIndex);
                car.ChangeTargetLane(targetLaneIndex);
            }

            if (triggerTrafficLightWait)
            {
                if (trafficLightGo)
                    car.IsWaitingTrafficLight = false;
                else if (trafficLightStop)
                    car.IsWaitingTrafficLight = true;
            }
        }
    }
}
