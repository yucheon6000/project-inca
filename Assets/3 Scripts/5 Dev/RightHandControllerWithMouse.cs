using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class RightHandControlByMouse : MonoBehaviour
{
    [SerializeField]
    private LayerMask targetLayerMask;
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private Transform rightHand;
    [SerializeField]
    private float mousePosZ = 10f;
    private Vector3 mousePosWorld;
    private Vector3 targetPos;

    private void Update()
    {
        if (IncaMainManager.Instance.IsVRMode()) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mousePosZ;
        mousePosWorld = camera.ScreenToWorldPoint(mousePos);

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, 10000, targetLayerMask);

        if (hit == false)
            targetPos = mousePosWorld;
        else
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();

            if (interactable == null || interactable.IsInteractable() == false)
                targetPos = mousePosWorld;
            else
                targetPos = hitInfo.point;
        }

        rightHand.LookAt(targetPos, Vector3.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(camera.transform.position, mousePosWorld);
        Gizmos.DrawWireSphere(targetPos, 0.3f);
    }
}
