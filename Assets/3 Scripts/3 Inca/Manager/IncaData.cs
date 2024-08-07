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

        public static float UserCarSpeed => IncaDetectManager.Instance.UserCarSpeed;
        public static int UserCarLaneIndex => IncaDetectManager.Instance.UserCarLaneIndex;

        // User Thing..
    }
}

