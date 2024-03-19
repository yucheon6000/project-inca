using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStateStop : CarState
{
    [SerializeField]
    private float startDriveWaitTime;
    private float startDriveWaitTimer;

    public override void Enter(Car car)
    {
        startDriveWaitTimer = 0;
    }

    public override void Excute(Car car)
    {
        if (car.ShouldStop)
        {
            startDriveWaitTimer = 0;
            return;
        }

        startDriveWaitTimer += Time.deltaTime;

        if (startDriveWaitTimer >= startDriveWaitTime)
            car.ChangeState(Car.CarStates.Drive);
    }

    public override void Exit(Car car) { }
}
