using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Environment;
using System.Linq;

namespace Inca
{
    public class IncaDetectManager : IncaManager
    {
        [SerializeField]
        private SphereCollider lidarCollider;

        [Header("Detected Object Prefab")]
        [SerializeField]
        private DetectedObject detectedObjectPrefab;

        private static Dictionary<Guid, DetectedObject> detectedObjects = new Dictionary<Guid, DetectedObject>();

        public static List<DetectedObject> GetAllDetectedObjects()
        {
            return detectedObjects.Values.ToList();
        }

        #region Methods related on UnityAction

        /* onTriggerEnterDetectedObject */
        private static List<UnityAction<DetectedObject, bool>> onTriggerEnterDetectedObject
            = new List<UnityAction<DetectedObject, bool>>();

        public static UnityAction<DetectedObject, bool> AddOnTriggerEnterDetectedObject
            (UnityAction<DetectedObject, bool> action)
        {
            onTriggerEnterDetectedObject.Add(action);
            return action;
        }

        public static bool RemoveOnTriggerEnterDetectedObject(UnityAction<DetectedObject, bool> action)
            => onTriggerEnterDetectedObject.Remove(action);

        /* onTriggerExitDetectedObject */
        private static List<UnityAction<DetectedObject>> onTriggerExitDetectedObject = new List<UnityAction<DetectedObject>>();

        public static UnityAction<DetectedObject> AddOnTriggerExitDetectedObject
            (UnityAction<DetectedObject> action)
        {
            onTriggerExitDetectedObject.Add(action);
            return action;
        }

        public static bool RemoveOnTriggerExitDetectedObject(UnityAction<DetectedObject> action)
            => onTriggerExitDetectedObject.Remove(action);

        #endregion

        private void EnterEnvironmentObject(EnvironmentObject environmentObject)
        {
            Guid guid = environmentObject.GUID;
            bool firstTime = !detectedObjects.ContainsKey(guid);

            // If there is no existed DetectedObject in Dictionary, create a new DetectedObject.
            if (firstTime)
            {
                DetectedObject newObj = Instantiate<DetectedObject>(detectedObjectPrefab);
                detectedObjects.Add(guid, newObj);
            }

            DetectedObject obj = detectedObjects[guid];

            obj.Initialize(environmentObject);
            obj.IsVisible(true);

            foreach (var action in onTriggerEnterDetectedObject)
                action.Invoke(obj, firstTime);
        }

        private void ExitEnvironmentObject(EnvironmentObject environmentObject)
        {
            DetectedObject obj = detectedObjects[environmentObject.GUID];
            if (!obj) return;

            obj.IsVisible(false);

            foreach (var action in onTriggerExitDetectedObject)
                action.Invoke(obj);
        }

        /* Lidar Events */
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EnvironmentObject>(out EnvironmentObject obj))
                EnterEnvironmentObject(obj);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<EnvironmentObject>(out EnvironmentObject obj))
                ExitEnvironmentObject(obj);
        }

        private void OnDrawGizmos()
        {
            // Draw Lidar range
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lidarCollider.transform.position, lidarCollider.radius);
        }
    }
}
