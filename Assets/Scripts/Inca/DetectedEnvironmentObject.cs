using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Inca
{
    public enum EnvironmentObjectType { None = -1, Car = 100, Building = 200, Pedestrian = 300 }

    public class DetectedEnvironmentObject
    {
        private EnvironmentObject environmentObject;
        private Transform originalTransform;

        public Guid GUID => environmentObject.GUID;

        public EnvironmentObjectType ObjectType => (EnvironmentObjectType)environmentObject.ObjectType;

        public Vector3 Position => originalTransform.position;
        public Quaternion Rotation => originalTransform.rotation;
        public Vector3 Scale => environmentObject.ColliderSize;

        private bool isVisible = false;

        public DetectedEnvironmentObject(EnvironmentObject environmentObject)
        {
            this.environmentObject = environmentObject;
            this.originalTransform = environmentObject.transform;
        }

        public bool IsVisible()
        {
            return isVisible;
        }

        public void IsVisible(bool value)
        {
            isVisible = value;
        }
    }
}

