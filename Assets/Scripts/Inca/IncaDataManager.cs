using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inca
{
    public class IncaData
    {
        public static float Speed => IncaDataManager.Instance.GetSpeed();
        public static Vector3 PlayerPosition => IncaDataManager.Instance.GetPlayerPosition();
    }

    public class IncaDataManager : IncaManager
    {
        public static IncaDataManager Instance;

        [SerializeField]
        private PrometeoCarController carController;

        [SerializeField]
        private float speed = 0;

        [SerializeField]
        private Transform player;

        private void Awake()
        {
            if (!Instance) Instance = this;
        }

        private void Update()
        {
            float speed = carController.GetComponent<PrometeoCarController>().carSpeed;
            SetSpeed(speed);
        }

        private void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public float GetSpeed() => this.speed;

        public Vector3 GetPlayerPosition() => this.player.transform.position;
    }
}

