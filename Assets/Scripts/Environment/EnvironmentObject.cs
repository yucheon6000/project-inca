using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    protected Guid guid = Guid.NewGuid();
    public Guid GUID => guid;

    [Header("Bound Collider")]
    [SerializeField]
    protected BoxCollider boundCollider;

    [Header("Gizmos")]
    [SerializeField]
    private bool drawGizmosSeleted = true;
    private bool drawGizmos = false;

    public void DrawGizmos(bool value)
    {
        drawGizmos = value;
    }

    private void DrawGizmos(Color color)
    {
        Gizmos.color = color;

        Vector3 c = boundCollider.transform.TransformPoint(boundCollider.center);
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(c, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawCube(Vector3.zero, boundCollider.size);
        Gizmos.DrawWireCube(Vector3.zero, boundCollider.size);

        Gizmos.matrix = Matrix4x4.zero;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        DrawGizmos(Color.blue);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmosSeleted) return;

        DrawGizmos(Color.red);
    }
}
