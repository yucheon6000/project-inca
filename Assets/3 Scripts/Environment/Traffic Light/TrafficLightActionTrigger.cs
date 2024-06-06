using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class TrafficLightActionTrigger : MonoBehaviour
{
    [Header("Traffic Light")]
    [SerializeField]
    private TrafficLight targetTrafficLight;
    [SerializeField]
    private int targetPhaseIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Car>(out Car car) && car.transform == IncaData.PlayerCarTransform)
            targetTrafficLight.SetPhaseIndex(targetPhaseIndex);
    }
}
