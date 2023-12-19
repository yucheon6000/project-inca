using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inca
{
    public class IncaData
    {
        public static float Speed => IncaDataManager.Instance.GetSpeed();
    }

    public class IncaDataManager : IncaManager
    {
        public static IncaDataManager Instance;

        [SerializeField]
        private float speed = 0;

        private void Awake()
        {
            if (!Instance) Instance = this;
        }

        private void Update()
        {
            float speed = targetCar.GetComponent<PrometeoCarController>().carSpeed;
            SetSpeed(speed);
        }

        private void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public float GetSpeed() => this.speed;
    }
}

