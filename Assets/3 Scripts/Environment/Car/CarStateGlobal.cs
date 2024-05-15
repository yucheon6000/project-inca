using System;
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
    bool checkIsInLine = false;
    public void UpdateNextLanePoint(Car car)
    {
        checkIsInLine = CheckPointIsBetweenTwoPoints(transform.position, car.CurrentLanePoint.Position, car.NextLanePoint.Position);

        if (!checkIsInLine)
        {
            SetNextLanePoint(car);
            return;
        }
    }

    private bool CheckPointIsBetweenTwoPoints(Vector3 targetPoint, Vector3 point1, Vector3 point2)
    {
        Vector2 V2point1 = new Vector2(point1.x, point1.z);
        Vector2 V2point2 = new Vector2(point2.x, point2.z);
        Vector2 V2targetPoint = new Vector2(targetPoint.x, targetPoint.z);

        float distA = Vector2.Distance(V2point1, V2point2);
        float distB = Vector2.Distance(V2targetPoint, V2point2);
        float distC = Vector2.Distance(V2targetPoint, V2point1);

        return Math.Pow(distA, 2) + Math.Pow(distB, 2) >= Math.Pow(distC, 2) && Math.Pow(distA, 2) + Math.Pow(distC, 2) >= Math.Pow(distB, 2);
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
