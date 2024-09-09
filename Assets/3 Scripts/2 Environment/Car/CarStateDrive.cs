using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class CarStateDrive : StateMonoBehaviour<Car>
{
    [SerializeField]
    private float originalMoveSpeed;
    [SerializeField]
    private float initialMoveSpeedMin = 20;
    [SerializeField]
    private float initialMoveSpeedMax = 30;
    [SerializeField]
    private float currentMoveSpeed = 20;
    [SerializeField]
    private float targetMoveSpeed = -1;
    private Vector3 moveDirection = Vector3.zero;

    public float CurrentMoveSpeed => currentMoveSpeed;
    public Vector3 CurrentVelocity => moveDirection * CurrentMoveSpeed;

    [Space]
    [SerializeField]
    private float rotateSpeed = 10;

    [Header("Is Starting")]
    [SerializeField]
    private float startDelayTime;
    private bool isStarting = false;
    private float startDelayTimer = 0;
    [SerializeField]
    private AnimationCurve stopAccelerationCurve;

    [Header("Is Stopping")]
    [SerializeField]
    private float stopDelayTime;
    private bool isStopping = false;
    private float stopDelayTimer = 0;
    [SerializeField]
    private AnimationCurve stopDecelerationCurve;

    [Header("Target Speed")]
    [SerializeField]
    private float slowDistance = 30;
    [SerializeField]
    private float increaseTargetSpeed = 5;
    [SerializeField]
    private float safetyDistance = 20;
    [SerializeField]
    private float decreaseTargetSpeed = 2;


    private void Awake()
    {
        currentMoveSpeed = Random.Range(initialMoveSpeedMin, initialMoveSpeedMax);
        originalMoveSpeed = currentMoveSpeed;
    }

    public override void Enter(Car car)
    {
        currentMoveSpeed = originalMoveSpeed;
        isStarting = true;
        isStopping = false;
        startDelayTimer = 0;
        stopDelayTimer = 0;
    }

    public void ChangeMoveSpeed(float moveSpeed)
    {
        originalMoveSpeed = moveSpeed;
        currentMoveSpeed = moveSpeed;
    }

    public override void Execute(Car car)
    {
        // If car has to stop
        if (!isStopping && car.ShouldStop)
        {
            isStopping = true;
            return;
        }

        UpdateStarting(car);
        UpdateStopping(car);

        UpdateTargetSpeedFromSafetyDistance(car);
        UpdateMoveSpeed();

        UpdateMoveAndRotate(car);

        UpdateNextLanePoint(car);
    }

    private void UpdateStarting(Car car)
    {
        if (!isStarting) return;

        startDelayTimer += Time.deltaTime;

        currentMoveSpeed = Mathf.Lerp(0, originalMoveSpeed, stopDecelerationCurve.Evaluate(startDelayTimer / startDelayTime));

        if (startDelayTimer >= startDelayTime)
            isStarting = false;
    }

    private void UpdateStopping(Car car)
    {
        if (!isStopping) return;

        stopDelayTimer += Time.deltaTime;

        currentMoveSpeed = Mathf.Lerp(originalMoveSpeed, 0, stopDecelerationCurve.Evaluate(stopDelayTimer / stopDelayTime));

        if (stopDelayTimer >= stopDelayTime)
            car.ChangeState(CarStates.Stop);
    }

    private void UpdateMoveSpeed()
    {
        if (targetMoveSpeed < 0) return;

        if (currentMoveSpeed > targetMoveSpeed)
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetMoveSpeed, decreaseTargetSpeed * Time.deltaTime);
        else
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetMoveSpeed, increaseTargetSpeed * Time.deltaTime);
    }

    private void UpdateTargetSpeedFromSafetyDistance(Car car)
    {
        Ray ray = new Ray(car.MiddlePosition, car.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);

        // if (hits.Length == 0) return;

        float minDistance = Mathf.Infinity;

        Car closestCar = null;
        foreach (RaycastHit hit in hits)
        {
            // If it is not a car, continue.
            if (hit.collider.gameObject.TryGetComponent<Car>(out Car otherCar))

                // If the car is me, continue.
                if (otherCar == car) continue;

            if (hit.distance < minDistance)
            {
                minDistance = hit.distance;
                closestCar = otherCar;
            }
        }

        if (closestCar == null || minDistance > slowDistance)
        {
            targetMoveSpeed = originalMoveSpeed;
            return;
        }

        else if (minDistance < safetyDistance)
            targetMoveSpeed = closestCar.CurrentMoveSpeed - 2;

    }

    private void UpdateMoveAndRotate(Car car)
    {
        if (CheckInvalidLanePointAndDestoryCar(car)) return;

        moveDirection = (car.NextLanePoint.Position - transform.position).normalized;

        float distCarToNextLanePoint = Vector3.Distance(transform.position, car.NextLanePoint.Position);
        float moveAmount = currentMoveSpeed * Time.deltaTime;

        Vector3 pos;

        if (distCarToNextLanePoint > moveAmount)
            pos = Vector3.MoveTowards(transform.position, car.NextLanePoint.Position, moveAmount);
        else
        {
            SetNextLanePoint(car);
            if (CheckInvalidLanePointAndDestoryCar(car)) return;

            pos = Vector3.MoveTowards(car.CurrentLanePoint.Position, car.NextLanePoint.Position, moveAmount - distCarToNextLanePoint);
        }
        // Vector3 pos = transform.position + (car.NextLanePoint.Position - transform.position).normalized * currentMoveSpeed * Time.deltaTime;


        Quaternion rot = Quaternion.Slerp(
              transform.rotation, Quaternion.LookRotation(car.NextLanePoint.Position - car.CurrentLanePoint.Position), Time.deltaTime * rotateSpeed
        );

        transform.SetPositionAndRotation(pos, rot);

        //IncaDetectManager.Instance.UpdateDetectedMyCar();
    }

    /* Update Next Lane Point */
    private float distanceToNextLanePoint = float.MaxValue;
    bool checkIsInLine = false;
    public void UpdateNextLanePoint(Car car)
    {
        if (CheckInvalidLanePointAndDestoryCar(car)) return;

        checkIsInLine = CheckPointIsBetweenTwoPoints(transform.position, car.CurrentLanePoint.Position, car.NextLanePoint.Position);

        if (!checkIsInLine)
        {
            SetNextLanePoint(car);
            return;
        }
    }

    private bool CheckPointIsBetweenTwoPoints(Vector3 currentPoint, Vector3 startPoint, Vector3 endPoint)
    {
        return Vector3.Distance(currentPoint, endPoint) > 0.001f;

        /*
        Vector2 V2StartPoint = new Vector2(startPoint.x, startPoint.z);
        Vector2 V2EndPoint = new Vector2(endPoint.x, endPoint.z);
        Vector2 V2CurrentPoint = new Vector2(currentPoint.x, currentPoint.z);

        float distA = Vector2.Distance(V2StartPoint, V2EndPoint);
        float distB = Vector2.Distance(V2CurrentPoint, V2EndPoint);
        float distC = Vector2.Distance(V2CurrentPoint, V2StartPoint);

        return Math.Pow(distA, 2) + Math.Pow(distB, 2) >= Math.Pow(distC, 2) && Math.Pow(distA, 2) + Math.Pow(distC, 2) >= Math.Pow(distB, 2);
        */
    }

    private void SetNextLanePoint(Car car)
    {
        car.PreviousLanePoint?.DeregisterUser(this.gameObject);
        car.CurrentLanePoint?.DeregisterUser(this.gameObject);
        car.NextLanePoint?.DeregisterUser(this.gameObject);

        car.PreviousLanePoint = car.CurrentLanePoint;
        car.CurrentLanePoint = car.NextLanePoint;

        car.NextLanePoint = car.CurrentLanePoint.GetNextLanePoint(car.TargetLaneIndex);

        if (CheckInvalidLanePointAndDestoryCar(car)) return;

        car.NextLanePoint.RegisterUser(this.gameObject);
        distanceToNextLanePoint = float.MaxValue;
    }

    public override void Exit(Car car) { }

    private bool CheckInvalidLanePointAndDestoryCar(Car car)
    {
        bool r = car.CurrentLanePoint == null || car.NextLanePoint == null;
        if (r)
            Destroy(car.gameObject);

        return r;
    }
}
