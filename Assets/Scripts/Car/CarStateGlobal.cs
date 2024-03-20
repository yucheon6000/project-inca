using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStateGlobal : State<Car>
{
    public override void Enter(Car car) { }

    public override void Execute(Car car)
    {
        UpdateSafety(car);
        UpdateNextLanePoint(car);
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

            else return true;
        }

        return false;
    }

    /* Update Next Lane Point */
    private float distanceToNextLanePoint = float.MaxValue;
    public void UpdateNextLanePoint(Car car)
    {
        float dist = Vector3.Distance(transform.position, car.NextLanePoint.Position);

        if (dist < 1)
        {
            SetNextLanePoint(car);
            return;
        }

        if (dist > distanceToNextLanePoint)
        {
            SetNextLanePoint(car);
            return;
        }

        distanceToNextLanePoint = dist;
    }

    private void SetNextLanePoint(Car car)
    {
        car.PreviousLanePoint?.DeregisterUser(this.gameObject);
        car.CurrentLanePoint?.DeregisterUser(this.gameObject);
        car.NextLanePoint?.DeregisterUser(this.gameObject);

        car.PreviousLanePoint = car.CurrentLanePoint;
        car.CurrentLanePoint = car.NextLanePoint;

        car.NextLanePoint = car.CurrentLanePoint.GetNextLanePoint(car.TargetLaneIndex);
        if (car.NextLanePoint == null)
        {
            Destroy(gameObject);
            return;
        }
        car.NextLanePoint.RegisterUser(this.gameObject);
        distanceToNextLanePoint = float.MaxValue;
    }

    public override void Exit(Car car) { }
}
