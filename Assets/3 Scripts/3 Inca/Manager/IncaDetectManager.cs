using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Environment;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Inca
{
    /// <summary>
    /// This class is for detecting environment objects and updating detected objects' position in the Detected World.
    /// </summary>
    public class IncaDetectManager : IncaManager
    {
        public static IncaDetectManager Instance { get; private set; }

        [Header("User Car")]
        [SerializeField]
        private Car userCar;
        public int UserCarLaneIndex
            => userCar.CurrentLanePoint == null ? 1 : userCar.CurrentLanePoint.LaneIndex;
        private CarStateDrive userCarStateDrive;
        public float UserCarSpeed => userCarStateDrive.CurrentMoveSpeed;
        public Vector3 UserCarVelocity => userCarStateDrive.CurrentVelocity;

        [Header("Detected World")]
        [SerializeField]
        private string detectedWorldSceneName;

        [Header("Detected Objects")]
        [SerializeField]
        private GameObject detectedObjectPrefab;
        private static Dictionary<Guid, DetectedObject> detectedObjects = new Dictionary<Guid, DetectedObject>();

        public static List<DetectedObject> GetAllDetectedObjects()
            => detectedObjects.Values.ToList();

        // Init
        private bool init = false;
        private List<EnvironmentObject> environmentObjectsBeforeInit = new List<EnvironmentObject>();

        // User things
        public UserObjects<DetectedObject> DetectedUserObjects { get; private set; }

        // This method is a kind of "Init" method. Because it is called after loading the Detected World scene.
        public void SetDetectedUserObjects(UserObjects<DetectedObject> detectedUserObjects)
        {
            DetectedUserObjects = detectedUserObjects;

            init = true;

            for (int i = 0; i < environmentObjectsBeforeInit.Count; ++i)
            {
                var envObj = environmentObjectsBeforeInit[i];
                if (envObj != null)
                    EnterEnvironmentObject(envObj);
            }
        }

        public UnityEvent<DetectedObject, bool> OnTriggerEnterDetectedObject { get; private set; }
            = new UnityEvent<DetectedObject, bool>();
        public UnityEvent<DetectedObject> OnTriggerExitDetectedObject { get; private set; }
            = new UnityEvent<DetectedObject>();


        public override void Init()
        {
            userCarStateDrive = userCar.GetComponent<CarStateDrive>();

            // Set sigleton
            if (Instance == null)
                Instance = this;

            // Load the Detected World.
            LoadDetectedWorld();

            StartCoroutine(CheckEnvironmentObjectIsDetectableRoutine());
        }

        private void LoadDetectedWorld()
        {
            // SceneManager의 sceneCount를 이용해 현재 로드된 모든 씬을 확인
            int sceneCount = SceneManager.sceneCount;

            // 모든 씬을 순회하면서 additive로 로드된 씬을 확인
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.isLoaded && scene.buildIndex != SceneManager.GetActiveScene().buildIndex)
                    if (scene.name == detectedWorldSceneName) return;
            }

            // If the Detected World was not loaded, load it.
            SceneManager.LoadScene(detectedWorldSceneName, LoadSceneMode.Additive);
        }

        // This is a coroutine which checks if some environment objects are detectable(visible) or not.
        private IEnumerator CheckEnvironmentObjectIsDetectableRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            while (true)
            {
                yield return wait;

                List<DetectedObject> detObjs = detectedObjects.Values.ToList();
                foreach (DetectedObject detObj in detObjs)
                {
                    if (detObj == null) continue;
                    if (CheckIfValidEnvironmentObject(detObj.EnvironmentObject) == false)
                        ExitEnvironmentObject(detObj.EnvironmentObject);
                }
            }
        }

        private bool CheckIfValidEnvironmentObject(EnvironmentObject environmentObject)
        {
            return environmentObject != null && environmentObject.gameObject.activeSelf && environmentObject.gameObject.activeInHierarchy;
        }

        private void EnterEnvironmentObject(EnvironmentObject environmentObject)
        {
            if (!init)
            {
                environmentObjectsBeforeInit.Add(environmentObject);
                return;
            }

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
            if (!init)
            {
                environmentObjectsBeforeInit.Remove(environmentObject);
                return;
            }

            Guid guid = environmentObject.GUID;

            if (detectedObjects.TryGetValue(guid, out DetectedObject detObj) == false) return;

            detectedObjects[guid] = null;

            detObj.IsVisible(false);

            // Invoke the event.
            OnTriggerExitDetectedObject.Invoke(detObj);

            // Return the detected object to the pool.
            MemoryPool.Instance(MemoryPoolType.DetectedObject).DeactivatePoolItem(detObj.gameObject);
        }

        /*------------ Lidar Events ------------*/
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
