using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using Inca;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Inca
{
    public enum IncaMode { VR, MonitorAndMouse }

    public class IncaMainManager : IncaManager
    {
        public static IncaMainManager Instance { get; private set; }

        [Header("Inca Managers")]
        [SerializeField]
        private List<IncaManager> managers = new List<IncaManager>();

        [Header("Mode")]
        [SerializeField]
        private IncaMode incaMode;
        public IncaMode GetIncaMode() => incaMode;

        [Space]
        [SerializeField]
        private UserObjects<EnvironmentObject> userObjectsForVRMode;
        [SerializeField]
        private UserObjects<EnvironmentObject> userObjectsForMonitorAndMouseMode;

        public UserObjects<EnvironmentObject> EnvironmentalUserObjects
            => incaMode == IncaMode.VR ? userObjectsForVRMode : userObjectsForMonitorAndMouseMode;

        [Space]
        [SerializeField]
        private UnityEvent onAwake = new UnityEvent();
        [SerializeField]
        private UnityEvent onAwakeInVRMode = new UnityEvent();
        [SerializeField]
        private UnityEvent onAwakeInMonitorAndMouseMode = new UnityEvent();

        private void Awake() => Init();

        public override void Init()
        {
            onAwake.Invoke();

            if (IsVRMode())
                onAwakeInVRMode.Invoke();
            else if (IsMonitorAndMouseMode())
                onAwakeInMonitorAndMouseMode.Invoke();

            if (Instance == null)
                Instance = this;
        }

        private void Start()
            => InitAllManagers();

        public void InitAllManagers()
        {
            foreach (IncaManager manager in managers)
                manager.Init();
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.R))
            //     XRInputSubsystem.TryRecenter()
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        public bool IsVRMode() => incaMode == IncaMode.VR;
        public bool IsMonitorAndMouseMode() => incaMode == IncaMode.MonitorAndMouse;
    }

    [Serializable]
    public struct UserObjects<T> where T : class
    {
        public T userCar;
        public T userHead;
        public T userRightHand;
        public T userLeftHand;
    }
}
