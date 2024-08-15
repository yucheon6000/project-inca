using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Inca
{
    public static class IncaData
    {
        public static Transform UserCarTransform
            => IncaDetectManager.Instance.DetectedUserObjects.userCar.AvailableTransform;
        public static Vector3 UserCarPosition => UserCarTransform.position;
        public static Vector3 UserCarForward => UserCarTransform.forward;

        public static float UserCarSpeed => IncaDetectManager.Instance.UserCarSpeed;
        public static Vector3 UserCarVelocity => IncaDetectManager.Instance.UserCarVelocity;
        public static int UserCarLaneIndex => IncaDetectManager.Instance.UserCarLaneIndex;

        public static Transform UserHeadTrasnform
            => IncaDetectManager.Instance.DetectedUserObjects.userHead.transform;
        public static Vector3 UserHeadPosition => UserHeadTrasnform.position;


        // User Thing..
    }
}

