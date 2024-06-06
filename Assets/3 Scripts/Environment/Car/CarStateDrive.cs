using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class CarStateDrive : State<Car>
{
    [SerializeField]
    private float originalMoveSpeed;
    [SerializeField]
    private float currentMoveSpeed = 20;
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
        UpdateMoveAndRotate(car);
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

    private void UpdateMoveAndRotate(Car car)
    {
        if (car.NextLanePoint == null || car.CurrentLanePoint == null) return;

        moveDirection = (car.NextLanePoint.Position - transform.position).normalized;

        Vector3 pos = Vector3.MoveTowards(transform.position, car.NextLanePoint.Position, currentMoveSpeed * Time.fixedDeltaTime);
        Quaternion rot = Quaternion.Slerp(
              transform.rotation, Quaternion.LookRotation(car.NextLanePoint.Position - car.CurrentLanePoint.Position), Time.deltaTime * rotateSpeed
        );

        transform.SetPositionAndRotation(pos, rot);

        IncaDetectManager.Instance.UpdateDetectedMyCar();
    }

    public override void Exit(Car car) { }
}
