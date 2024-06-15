using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class CarStateDrive : State<Car>
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
    }

    private void UpdateStarting(Car car)
    {
        if (!isStarting) return;

        startDelayTimer += Time.fixedDeltaTime;

        currentMoveSpeed = Mathf.Lerp(0, originalMoveSpeed, stopDecelerationCurve.Evaluate(startDelayTimer / startDelayTime));

        if (startDelayTimer >= startDelayTime)
            isStarting = false;
    }

    private void UpdateStopping(Car car)
    {
        if (!isStopping) return;

        stopDelayTimer += Time.fixedDeltaTime;

        currentMoveSpeed = Mathf.Lerp(originalMoveSpeed, 0, stopDecelerationCurve.Evaluate(stopDelayTimer / stopDelayTime));

        if (stopDelayTimer >= stopDelayTime)
            car.ChangeState(CarStates.Stop);
    }

    private void UpdateMoveSpeed()
    {
        if (targetMoveSpeed < 0) return;

        if (currentMoveSpeed > targetMoveSpeed)
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetMoveSpeed, decreaseTargetSpeed * Time.fixedDeltaTime);
        else
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetMoveSpeed, increaseTargetSpeed * Time.fixedDeltaTime);
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
        if (car.NextLanePoint == null || car.CurrentLanePoint == null) return;

        moveDirection = (car.NextLanePoint.Position - transform.position).normalized;

        Vector3 pos = Vector3.MoveTowards(transform.position, car.NextLanePoint.Position, currentMoveSpeed * Time.fixedDeltaTime);
        Quaternion rot = Quaternion.Slerp(
              transform.rotation, Quaternion.LookRotation(car.NextLanePoint.Position - car.CurrentLanePoint.Position), Time.fixedDeltaTime * rotateSpeed
        );

        transform.SetPositionAndRotation(pos, rot);

        IncaDetectManager.Instance.UpdateDetectedMyCar();
    }

    public override void Exit(Car car) { }
}
