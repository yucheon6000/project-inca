using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipFlopMovement : MonoBehaviour
{
    [SerializeField]
    private bool localSpace;

    [Space]
    [SerializeField]
    private Vector3 initialVelocity;
    private Vector3 currentVelocity;
    [SerializeField]
    private AnimationCurve velocityCurve;

    [Space]
    [SerializeField]
    private bool checkMaxPositionFirst = true;
    private bool checkMaxPosition = true;
    private bool checkMinPosition => !checkMaxPosition;
    [SerializeField]
    private Vector3 maxPosition;
    [SerializeField]
    private Vector3 minPosition;
    [SerializeField]
    private float rangeDistance;
    private Vector3 targetPosition => checkMaxPosition ? maxPosition : minPosition;
    private Vector3 MyPosition
    {
        get => localSpace ? transform.localPosition : transform.position;
        set
        {
            if (localSpace)
                transform.localPosition = value;
            else
                transform.position = value;
        }
    }

    [Space]
    [SerializeField]
    [Tooltip("Write 1 in the direction you want to check.")]
    private Vector3 checkAxis;

    private void Awake()
    {
        currentVelocity = initialVelocity;
        checkMaxPosition = checkMaxPositionFirst;
        rangeDistance = Vector3.Distance(minPosition, maxPosition);
    }

    [SerializeField]
    private float curveValue;

    private void Update()
    {
        curveValue = velocityCurve.Evaluate(
            (rangeDistance - Vector3.Distance(MyPosition, targetPosition)) / rangeDistance
        );

        // Move this gameObject.
        MyPosition += curveValue * currentVelocity * Time.deltaTime;

        // Check to see if it exceeds the range of movement.
        if (checkMaxPosition && CheckMaxPosition())
        {
            checkMaxPosition = !checkMaxPosition;
            currentVelocity *= -1;
        }
        else if (checkMinPosition && CheckMinPosition())
        {
            checkMaxPosition = !checkMaxPosition;
            currentVelocity *= -1;
        }
    }

    private bool CheckMaxPosition()
    {
        if (checkAxis.x > 0 && MyPosition.x < maxPosition.x) return false;
        if (checkAxis.y > 0 && MyPosition.y < maxPosition.y) return false;
        if (checkAxis.z > 0 && MyPosition.z < maxPosition.z) return false;

        return true;
    }

    private bool CheckMinPosition()
    {
        if (checkAxis.x > 0 && MyPosition.x > minPosition.x) return false;
        if (checkAxis.y > 0 && MyPosition.y > minPosition.y) return false;
        if (checkAxis.z > 0 && MyPosition.z > minPosition.z) return false;

        return true;
    }
}
