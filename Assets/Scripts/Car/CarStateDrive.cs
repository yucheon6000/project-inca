using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStateDrive : State<Car>
{
    [SerializeField]
    private float originalMoveSpeed;
    [SerializeField]
    private float currentMoveSpeed = 20;

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

    private void Awake()
    {
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
        UpdateMove(car);
        UpdateRotate(car);
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

    private void UpdateMove(Car car)
    {
        if (car.NextLanePoint == null || car.CurrentLanePoint == null) return;

        Vector3 moveDir = car.NextLanePoint.Position - car.CurrentLanePoint.Position;
        moveDir.Normalize();

        transform.position += moveDir * currentMoveSpeed * Time.deltaTime;
    }

    private void UpdateRotate(Car car)
    {

        // float t = (transform.position - car.CurrentLanePoint.Position).magnitude / (car.NextLanePoint.Position - car.CurrentLanePoint.Position).magnitude;

        transform.rotation = Quaternion.Slerp(
           transform.rotation, Quaternion.LookRotation(car.NextLanePoint.Position - car.CurrentLanePoint.Position), Time.deltaTime * rotateSpeed
        );
    }

    public override void Exit(Car car) { }
}
