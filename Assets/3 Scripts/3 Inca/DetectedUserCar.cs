using UnityEngine;
using Environment;
using Inca;

public class DetectedUserCar : DetectedObject
{
    [SerializeField]
    private BoxCollider boxCollider;

    private void Awake()
    {
        SyncUserCar();
    }

    private void SyncUserCar()
    {
        EnvironmentObject userCar = GameObject.Find("Car For Player").GetComponent<EnvironmentObject>();

        boxCollider.center = userCar.ColliderCenter;
        boxCollider.size = userCar.ColliderSize;

        Initialize(userCar);
    }
}
