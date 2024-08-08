using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStateGlobal : StateMonoBehaviour<Car>
{
    public override void Enter(Car car) { }

    public override void Execute(Car car)
    {
        UpdateSafety(car);
    }

    /* Update Safety */
    private void UpdateSafety(Car car)
    {
        car.IsWaitingUserOfNextLanePoint = CheckUserOfNextLanePoint(car);
        car.HasSafetyDistanceProblem = CheckSafetyDistanceProblem(car);
    }

    private bool CheckUserOfNextLanePoint(Car car)
    {
        if (car.NextLanePoint == null) return false;

        return !car.NextLanePoint.CanICome(this.gameObject);
    }

    private bool CheckSafetyDistanceProblem(Car car)
    {
        Ray ray = new Ray(car.MiddlePosition, car.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, car.SafetyDistance);

        if (hits.Length == 0) return false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.distance > car.SafetyDistance) continue;

            // If it is not a car, continue.
            if (!hit.collider.gameObject.TryGetComponent<Car>(out Car carMovement)) continue;

            // If the car is me, continue.
            if (carMovement == this) continue;

            // It the car is stopped, continue.
            if (carMovement.CurrentLanePoint == null) continue;

            // It the car is not on my line or my next line, continue.
            if (car.CurrentLanePoint.LaneIndex != carMovement.CurrentLanePoint.LaneIndex
                && car.NextLanePoint.LaneIndex != carMovement.CurrentLanePoint.LaneIndex) continue;

            else return true;
        }

        return false;
    }

    public override void Exit(Car car) { }
}
