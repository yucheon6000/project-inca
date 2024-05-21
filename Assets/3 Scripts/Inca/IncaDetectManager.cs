using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Environment;
using System.Linq;
using Unity.VisualScripting;

namespace Inca
{
    public class IncaDetectManager : IncaManager
    {
        [Header("User Car")]
        [SerializeField]
        private Transform envCar;
        [SerializeField]
        private Transform detCar;
        public void SetUserCar(Transform userCar) => detCar = userCar;

        [Header("Detected Objects")]
        [SerializeField]
        private GameObject detectedObjectPrefab;

        [SerializeField]
        private SphereCollider lidarCollider;

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
        [SerializeField]
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

        private void LateUpdate()
        {
            if (detCar == null) return;

            detCar.position = envCar.position;
            detCar.rotation = envCar.rotation;
        }

        private void EnterEnvironmentObject(EnvironmentObject environmentObject)
        {
            Guid guid = environmentObject.GUID;
            bool firstTime = !detectedObjects.ContainsKey(guid);

            GameObject newGameObj = MemoryPool
                                        .Instance(MemoryPoolType.DetectedObject)
                                        .ActivatePoolItem(detectedObjectPrefab);

            DetectedObject newDetObj = newGameObj.GetComponent<DetectedObject>();

            if (firstTime)
                detectedObjects.Add(guid, newDetObj);

            detectedObjects[guid] = newDetObj;
            DetectedObject obj = detectedObjects[guid];

            obj.Initialize(environmentObject);

            foreach (var action in onTriggerEnterDetectedObject)
                action.Invoke(obj, firstTime);
        }

        private void ExitEnvironmentObject(EnvironmentObject environmentObject)
        {
            Guid guid = environmentObject.GUID;

            DetectedObject detObj = detectedObjects[guid];
            if (!detObj) return;

            foreach (var action in onTriggerExitDetectedObject)
                action.Invoke(detObj);

            MemoryPool.Instance(MemoryPoolType.DetectedObject).DeactivatePoolItem(detObj.gameObject);
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
            {
                ExitEnvironmentObject(obj);
            }
        }

        private void OnDrawGizmos()
        {
            // Draw Lidar range
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lidarCollider.transform.position, lidarCollider.radius);
        }
    }
}
