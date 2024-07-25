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
        [SerializeField]
        protected EnvironmentObject environmentObject;
        public EnvironmentObject EnvironmentObject => environmentObject;

        public Guid GUID => environmentObject.GUID;
        public DetectedObjectType ObjectType => (DetectedObjectType)environmentObject.ObjectType;

        private Transform originalTransform;

        public Vector3 Position
            => (environmentObject != null) ? originalTransform.position : environmentObject.transform.position;
        public Quaternion Rotation
            => (environmentObject != null) ? originalTransform.rotation : environmentObject.transform.rotation;
        public Vector3 Scale
            => environmentObject.ColliderSize;

        private bool isVisible = false;     // = is being detected by Inca system, is in range of Inca system

        public UnityEvent OnHideDetectedObject { get; private set; } = new UnityEvent();

        private Collider[] colliders;

        private void Awake()
        {
            colliders = GetComponents<Collider>();
        }

        public void Initialize(EnvironmentObject environmentObject)
        {
            this.environmentObject = environmentObject;
            originalTransform = environmentObject.transform;
            transform.position = Position;
            transform.rotation = Rotation;

            OnHideDetectedObject = new UnityEvent();
        }

        private void FixedUpdate()
        {
            if (EnvironmentObjectIsVisible() == false)
            {
                IsVisible(false);
                return;
            }

            SyncEnvObjPosAndRot();
        }

        public bool EnvironmentObjectIsVisible()
        {
            return environmentObject != null && environmentObject.gameObject.activeSelf && environmentObject.gameObject.activeInHierarchy;
        }

        [ContextMenu("Synchronize the Environment Object")]
        protected void SyncEnvObjPosAndRot()
        {
            if (environmentObject == null) return;

            transform.SetPositionAndRotation(Position, Rotation);
        }

        public bool IsVisible() => isVisible;

        public void IsVisible(bool value)
        {
            if (isVisible == value) return;

            isVisible = value;

            // if this is hided
            if (!isVisible)
                OnHide();
        }

        private void OnHide()
        {
            // Deactivate all colliders
            foreach (var col in colliders)
                col.enabled = false;

            // Invoke the event
            OnHideDetectedObject.Invoke();
        }

        private void OnEnable()
        {
            // Activate all colliders
            foreach (var col in colliders)
                col.enabled = true;
        }

        private void OnDisable()
        {
            IsVisible(false);

            // int chCount = transform.childCount;
            // for (int i = chCount - 1; i >= 0; i--)
            //     Destroy(transform.GetChild(i).gameObject);
        }

        private void OnDrawGizmos()
        {
            // if (!isVisible) return;
            if (environmentObject == null) return;

            DrawGizmos(Color.red);
        }

        private void DrawGizmos(Color color)
        {
            Gizmos.color = color;

            Vector3 c = originalTransform.TransformPoint(environmentObject.ColliderCenter);
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(c, Rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;

            // Gizmos.DrawCube(Vector3.zero, environmentObject.ColliderSize);
            Gizmos.DrawWireCube(Vector3.zero, Scale);

            Gizmos.matrix = Matrix4x4.zero;
        }
    }
}

