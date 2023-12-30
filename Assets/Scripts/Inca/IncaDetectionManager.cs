using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Environment;

namespace Inca
{
    public class IncaDetectionManager : IncaManager
    {
        [SerializeField]
        private SphereCollider lidarCollider;

        private Dictionary<Guid, DetectedEnvironmentObject> detectedEnvironmentObjects = new Dictionary<Guid, DetectedEnvironmentObject>();

        #region Methods related on UnityAction

        /* onTriggerEnterDetectedEnvironmentObject */
        private static List<UnityAction<DetectedEnvironmentObject, bool>> onTriggerEnterDetectedEnvironmentObject
            = new List<UnityAction<DetectedEnvironmentObject, bool>>();

        public static UnityAction<DetectedEnvironmentObject, bool> AddOnTriggerEnterDetectedEnvironmentObject
            (UnityAction<DetectedEnvironmentObject, bool> action)
        {
            onTriggerEnterDetectedEnvironmentObject.Add(action);
            return action;
        }

        public static bool RemoveOnTriggerEnterDetectedEnvironmentObject(UnityAction<DetectedEnvironmentObject, bool> action)
            => onTriggerEnterDetectedEnvironmentObject.Remove(action);

        /* onTriggerExitDetectedEnvironmentObject */
        private static List<UnityAction<DetectedEnvironmentObject>> onTriggerExitDetectedEnvironmentObject = new List<UnityAction<DetectedEnvironmentObject>>();

        public static UnityAction<DetectedEnvironmentObject> AddOnTriggerExitDetectedEnvironmentObject
            (UnityAction<DetectedEnvironmentObject> action)
        {
            onTriggerExitDetectedEnvironmentObject.Add(action);
            return action;
        }

        public static bool RemoveOnTriggerExitDetectedEnvironmentObject(UnityAction<DetectedEnvironmentObject> action)
            => onTriggerExitDetectedEnvironmentObject.Remove(action);

        #endregion

        private void EnterEnvironmentObject(EnvironmentObject environmentObject)
        {
            Guid guid = environmentObject.GUID;
            bool firstTime = !detectedEnvironmentObjects.ContainsKey(guid);

            // If there is no DetectedEnvironmentObject, create a new DetectedEnvironmentObject.
            if (firstTime)
            {
                DetectedEnvironmentObject newObj = new DetectedEnvironmentObject(environmentObject);
                detectedEnvironmentObjects.Add(guid, newObj);
            }

            DetectedEnvironmentObject obj = detectedEnvironmentObjects[guid];

            obj.IsVisible(true);

            foreach (var action in onTriggerEnterDetectedEnvironmentObject)
                action.Invoke(obj, firstTime);
        }

        private void ExitEnvironmentObject(EnvironmentObject environmentObject)
        {
            DetectedEnvironmentObject obj = new DetectedEnvironmentObject(environmentObject);
            obj.IsVisible(false);

            foreach (var action in onTriggerExitDetectedEnvironmentObject)
                action.Invoke(obj);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EnvironmentObject>(out EnvironmentObject obj))
            {
                print(obj);
                obj.DrawGizmos(true);
                EnterEnvironmentObject(obj);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<EnvironmentObject>(out EnvironmentObject obj))
            {
                obj.DrawGizmos(false);
                ExitEnvironmentObject(obj);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lidarCollider.transform.position, lidarCollider.radius);
        }
    }
}
