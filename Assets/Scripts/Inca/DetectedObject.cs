using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Inca
{
    public enum DetectedObjectType { None = -1, Car = 100, Building = 200, Pedestrian = 300 }

    public class DetectedObject : MonoBehaviour
    {
        private EnvironmentObject environmentObject;
        private Transform originalTransform;

        public Guid GUID => environmentObject.GUID;

        public DetectedObjectType ObjectType => (DetectedObjectType)environmentObject.ObjectType;

        public Vector3 Position => originalTransform.position;
        public Quaternion Rotation => originalTransform.rotation;
        public Vector3 Scale => environmentObject.ColliderSize;

        private bool isVisible = false;     // = is being detected by Inca system, is in range of Inca system

        public void Initialize(EnvironmentObject environmentObject)
        {
            this.environmentObject = environmentObject;
            this.originalTransform = environmentObject.transform;
            transform.position = Position;
            transform.rotation = Rotation;
            // transform.localScale = Scale;    // ??
        }

        public bool IsVisible() => isVisible;

        public void IsVisible(bool value)
        {
            isVisible = value;
        }

        /* Gizmos */
        private void OnDrawGizmos()
        {
            if (!isVisible) return;

            if (ObjectType == DetectedObjectType.Car)
                DrawMyGizmos(Color.red);
            if (ObjectType == DetectedObjectType.Building)
                DrawMyGizmos(Color.blue);
        }

        private void DrawMyGizmos(Color color)
        {
            Gizmos.color = color;

            Vector3 c = originalTransform.TransformPoint(environmentObject.ColliderCenter);
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(c, originalTransform.rotation, originalTransform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            // Gizmos.DrawCube(Vector3.zero, environmentObject.ColliderSize);
            Gizmos.DrawWireCube(Vector3.zero, environmentObject.ColliderSize);

            Gizmos.matrix = Matrix4x4.zero;
        }
    }
}
