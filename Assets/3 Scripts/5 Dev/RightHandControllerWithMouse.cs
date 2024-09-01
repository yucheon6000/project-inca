using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class RightHandControlByMouse : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private float mousePosZ = 10f;
    private Vector3 targetPos;

    private void Update()
    {
        if (IncaMainManager.Instance.IsVRMode()) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mousePosZ;
        targetPos = camera.ScreenToWorldPoint(mousePos);

        rightHand.LookAt(targetPos, Vector3.up);
    }
}
