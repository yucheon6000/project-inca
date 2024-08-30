using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using Inca;
using UnityEngine;
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

        private void Awake() => Init();

        public override void Init()
        {
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
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
    }

    [Serializable]
    public struct UserObjects<T> where T : class
    {
        public T userCar;
        public T userHead;
        public T userHandRight;
        public T userHandLeft;
    }
}
