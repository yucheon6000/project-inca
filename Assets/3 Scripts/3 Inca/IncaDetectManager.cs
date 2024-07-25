using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Environment;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

namespace Inca
{
    /// <summary>
    /// This class is for detecting environment objects and updating detected objects' position in the Detected World.
    /// </summary>
    public class IncaDetectManager : IncaManager
    {
        public static IncaDetectManager Instance { get; private set; }

        [Header("Detected World")]
        [SerializeField]
        private string detectedWorldSceneName;

        [Header("Detected Objects")]
        [SerializeField]
        private GameObject detectedObjectPrefab;

        [Header("User Car")]
        [SerializeField]
        private Transform envCar;       // environment object
        [SerializeField]
        private Transform detCar;       // detected object
        public void SetUserCar(Transform userCar) => detCar = userCar;

        [Header("Sensor")]
        [SerializeField]
        private SphereCollider lidarCollider;       // to detect environment objects

        private static Dictionary<Guid, DetectedObject> detectedObjects = new Dictionary<Guid, DetectedObject>();

        public static List<DetectedObject> GetAllDetectedObjects()
            => detectedObjects.Values.ToList();

        public UnityEvent<DetectedObject, bool> OnTriggerEnterDetectedObject { get; private set; }
            = new UnityEvent<DetectedObject, bool>();
        public UnityEvent<DetectedObject> OnTriggerExitDetectedObject { get; private set; }
            = new UnityEvent<DetectedObject>();

        private void Awake()
        {
            // Set sigleton
            if (Instance == null)
                Instance = this;

            // Load the Detected World.
            LoadDetectedWorld();

            StartCoroutine(CheckEnvironmentObjectIsDetectableRoutine());
        }

        private void LoadDetectedWorld()
        {
            SceneManager.LoadScene(detectedWorldSceneName, LoadSceneMode.Additive);
        }

        // This is a coroutine which checks if some environment objects are detectable(visible) or not.
        private IEnumerator CheckEnvironmentObjectIsDetectableRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            while (true)
            {
                yield return wait;

                foreach (DetectedObject detObj in detectedObjects.Values)
                {
                    if (detObj == null) continue;
                    if (canDetectEnvironmentObject(detObj.EnvironmentObject) == false)
                        ExitEnvironmentObject(detObj.EnvironmentObject);
                }
            }
        }

        private bool canDetectEnvironmentObject(EnvironmentObject environmentObject)
        {
            return environmentObject != null && environmentObject.gameObject.activeSelf && environmentObject.gameObject.activeInHierarchy;
        }

        private void EnterEnvironmentObject(EnvironmentObject environmentObject)
        {
            Guid guid = environmentObject.GUID;

            // Check if it was detected.
            bool isFirstDetection = !detectedObjects.ContainsKey(guid);

            // Get a detected object from the pool.
            DetectedObject detObj = MemoryPool
                                        .Instance(MemoryPoolType.DetectedObject)
                                        .ActivatePoolItem(detectedObjectPrefab)
                                        .GetComponent<DetectedObject>();

            // If it is first time detecting this object, add this detected object to the list.
            if (isFirstDetection)
                detectedObjects.Add(guid, detObj);
            else
                detectedObjects[guid] = detObj;

            detObj.Initialize(environmentObject);
            detObj.IsVisible(true);

            // Invoke the event.
            OnTriggerEnterDetectedObject.Invoke(detObj, isFirstDetection);
        }

        private void ExitEnvironmentObject(EnvironmentObject environmentObject)
        {
            Guid guid = environmentObject.GUID;

            DetectedObject detObj = detectedObjects[guid];
            if (detObj == null) return;

            detectedObjects[guid] = null;

            detObj.IsVisible(false);

            // Invoke the event.
            OnTriggerExitDetectedObject.Invoke(detObj);

            // Return the detected object to the pool.
            MemoryPool.Instance(MemoryPoolType.DetectedObject).DeactivatePoolItem(detObj.gameObject);
        }

        /*********** Lidar Events ***********/
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
    }
}
