using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment;
using UnityEngine.Events;

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
            environmentObject.RegisterOnDistroyAction(() => IsVisible(false));
            // transform.localScale = Scale;    // ??
        }

        private void Update()
        {
            if (originalTransform == null || !originalTransform.gameObject.activeSelf || !originalTransform.gameObject.activeInHierarchy)
                IsVisible(false);

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (!environmentObject) return;

            transform.position = Position;
        }

        public bool IsVisible() => isVisible;

        public void IsVisible(bool value)
        {
            if (isVisible == value) return;

            isVisible = value;

            if (value) return;

            // if this is hided
            OnHide();
        }

        public void OnHide()
        {
            // Deactivate colliders
            var colliders = GetComponents<Collider>();

            if (colliders != null && colliders.Length != 0)
                foreach (var col in colliders)
                    col.enabled = false;

            // call OnHide events
            foreach (var action in onHideActions)
                action.Invoke();
        }

        private List<UnityAction> onHideActions = new List<UnityAction>();
        public void RegisterOnHideAction(UnityAction action)
        {
            onHideActions.Add(action);
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

