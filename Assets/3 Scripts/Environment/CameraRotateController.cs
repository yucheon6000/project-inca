using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateController : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Vector3 originPosition;
    [SerializeField]
    private float maxDistance;

    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private float rotateSpeed;

    private void LateUpdate()
    {
        Vector3 targetPos = targetTransform.localPosition - originPosition;
        targetPos = originPosition + Vector3.ClampMagnitude(targetPos, maxDistance);

        Vector3 worldTargetPos = transform.TransformPoint(targetPos);

        Quaternion lookAtTarget = Quaternion.LookRotation(worldTargetPos - camera.transform.position);

        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, lookAtTarget, rotateSpeed * Time.fixedDeltaTime);
    }
}
