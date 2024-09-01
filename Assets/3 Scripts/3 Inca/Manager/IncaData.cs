using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment;
using Oculus.Interaction;

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

    public enum IncaButtonCode
    {
        A,
        B, // X, Y,
        RightTrigger,
        // LeftTrigger,
    }

    public static class IncaInput
    {
        public static IInteractable Target => IncaInputManager.Instance.CurrentTarget;
        public static Vector3 HitPoint => IncaInputManager.Instance.HitPoint;

        public static bool GetButtonDown(IncaButtonCode buttonCode)
        {
            if (IncaMainManager.Instance == null) return false;

            if (IncaMainManager.Instance.GetIncaMode() == IncaMode.VR)
            {
                switch (buttonCode)
                {
                    case IncaButtonCode.A:
                        return OVRInput.GetDown(OVRInput.Button.One);
                    case IncaButtonCode.B:
                        return OVRInput.GetDown(OVRInput.Button.Two);
                    case IncaButtonCode.RightTrigger:
                        return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f
                                && IncaInputManager.PrevRightTrigger < 0.5f;
                }
            }

            else
            {
                switch (buttonCode)
                {
                    case IncaButtonCode.A:
                        return Input.GetMouseButtonDown(1);
                    case IncaButtonCode.B:
                        return Input.GetMouseButtonDown(2);
                    case IncaButtonCode.RightTrigger:
                        return Input.GetMouseButtonDown(0);
                }
            }

            return false;
        }

        public static bool GetButton(IncaButtonCode buttonCode)
        {
            if (IncaMainManager.Instance == null) return false;

            if (IncaMainManager.Instance.GetIncaMode() == IncaMode.VR)
            {
                switch (buttonCode)
                {
                    case IncaButtonCode.A:
                        return OVRInput.Get(OVRInput.Button.One);
                    case IncaButtonCode.B:
                        return OVRInput.Get(OVRInput.Button.Two);
                    case IncaButtonCode.RightTrigger:
                        return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f
                                && IncaInputManager.PrevRightTrigger > 0.5f;
                }
            }

            else
            {
                switch (buttonCode)
                {
                    case IncaButtonCode.A:
                        return Input.GetMouseButton(1);
                    case IncaButtonCode.B:
                        return Input.GetMouseButton(2);
                    case IncaButtonCode.RightTrigger:
                        return Input.GetMouseButton(0);
                }
            }

            return false;
        }

        public static bool GetButtonUp(IncaButtonCode buttonCode)
        {
            if (IncaMainManager.Instance == null) return false;

            if (IncaMainManager.Instance.GetIncaMode() == IncaMode.VR)
            {
                switch (buttonCode)
                {
                    case IncaButtonCode.A:
                        return OVRInput.GetUp(OVRInput.Button.One);
                    case IncaButtonCode.B:
                        return OVRInput.GetUp(OVRInput.Button.Two);
                    case IncaButtonCode.RightTrigger:
                        return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.5f
                                && IncaInputManager.PrevRightTrigger > 0.5f;
                }
            }

            else
            {
                switch (buttonCode)
                {
                    case IncaButtonCode.A:
                        return Input.GetMouseButtonUp(1);
                    case IncaButtonCode.B:
                        return Input.GetMouseButtonUp(2);
                    case IncaButtonCode.RightTrigger:
                        return Input.GetMouseButtonUp(0);
                }
            }

            return false;
        }


    }
}

