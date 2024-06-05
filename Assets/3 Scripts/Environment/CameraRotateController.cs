using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateController : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private int mousePosMinX;
    [SerializeField]
    private int mousePosMaxX;

    [SerializeField]
    private float rotationMinY;
    [SerializeField]
    private float rotationMaxY;

    [SerializeField]
    private float currentRotationY;
    [SerializeField]
    private float targetRotationY;

    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private float rotateSpeed;

    private void Awake()
    {
        currentRotationY = camera.transform.localRotation.eulerAngles.y;
        targetRotationY = currentRotationY;
    }

    private void FixedUpdate()
    {

        // print(Input.mousePosition.x);

        // if (Input.mousePosition.x > mousePosMaxX)
        //     targetRotationY = rotationMaxY;
        // else if (Input.mousePosition.x < mousePosMinX)
        //     targetRotationY = rotationMinY;

        Quaternion target = Quaternion.LookRotation(targetTransform.position - camera.transform.position);

        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, target, rotateSpeed * Time.fixedDeltaTime);

        // targetRotationY = Math.Clamp(targetRotationY, rotationMinY, rotationMaxY);

        // float y = Mathf.Lerp(currentRotationY, targetRotationY, rotateSpeed * Time.deltaTime);

        // currentRotationY = y;

        // Vector3 result = camera.transform.localRotation.eulerAngles;
        // result.y = y;

        // camera.transform.localRotation = Quaternion.Euler(result);
    }
}
