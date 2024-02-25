using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public enum EnvironmentObjectType { None = -1, Car = 100, Building = 200, Pedestrian = 300 }

    public class EnvironmentObject : MonoBehaviour
    {
        protected Guid guid = Guid.NewGuid();       // [ASSUME] Environment objects have each GUID.
        public Guid GUID => guid;

        [Header("Type")]
        [SerializeField]
        protected EnvironmentObjectType objectType = EnvironmentObjectType.None;
        public EnvironmentObjectType ObjectType => objectType;

        [Header("Bound Collider")]
        [SerializeField]
        protected BoxCollider boundCollider;
        public Vector3 ColliderCenter => boundCollider.center;
        public Vector3 ColliderSize => boundCollider.size;
        public Bounds ColliderBounds => boundCollider.bounds;
    }
}