using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    [Header("Bound Collider")]
    [SerializeField]
    protected BoxCollider boundCollider;

    private bool isTurnedGizmos = false;

    public void TurnGizmos(bool value)
    {
        isTurnedGizmos = value;
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
        if (!isTurnedGizmos) return;

        DrawGizmos(Color.blue);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos(Color.red);
    }
}
