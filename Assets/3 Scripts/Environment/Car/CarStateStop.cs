using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStateStop : State<Car>
{
    [SerializeField]
    private float startDriveWaitTime;
    private float startDriveWaitTimer;

    public override void Enter(Car car)
    {
        startDriveWaitTimer = 0;
    }

    public override void Execute(Car car)
    {
        if (car.ShouldStop)
        {
            startDriveWaitTimer = 0;
            return;
        }

        startDriveWaitTimer += Time.deltaTime;

        if (startDriveWaitTimer >= startDriveWaitTime)
            car.ChangeState(CarStates.Drive);
    }

    public override void Exit(Car car) { }
}
