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
        public static Vector3 PlayerForward => IncaDataManager.Instance.GetPlayerForward();
        public static Transform PlayerTransform => IncaDataManager.Instance.GetPlayerTransform();
        public static int PlayerLaneIndex => IncaDataManager.Instance.GetPlayerLaneIndex();
        public static Vector3 PlayerVelocity => IncaDataManager.Instance.GetPlayerVelocity();

        // [ASSUME] Player와 Car의 구분이 필요할 수 있음
    }

    public class IncaDataManager : IncaManager
    {
        public static IncaDataManager Instance;

        [SerializeField]
        private Car car;
        [SerializeField]
        private CarStateDrive carStateDrive;

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
            // float speed = carController.GetComponent<PrometeoCarController>().carSpeed;
            // SetSpeed(speed);
        }

        private void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public float GetSpeed() => this.speed;

        public Vector3 GetPlayerPosition() => this.player.transform.position;
        public Vector3 GetPlayerForward() => this.player.transform.forward;
        public Transform GetPlayerTransform() => this.player.transform;
        public int GetPlayerLaneIndex() => this.car.CurrentLanePoint.LaneIndex;
        public Vector3 GetPlayerVelocity()
        {
            return carStateDrive.CurrentVelocity;
        }
    }
}

