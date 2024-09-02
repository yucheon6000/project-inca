using System.Collections;
using System.Collections.Generic;
using Inca;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Inca
{
    public class IncaInputManager : IncaManager
    {
        public static IncaInputManager Instance;

        [Header("[Setting]")]
        [SerializeField]
        private LayerMask targetLayerMask;
        [SerializeField]
        private float rayMaxDistance = 10000;
        [SerializeField]
        private float defaultCursorDistance;
        private Transform rightHand;
        [SerializeField]
        private SpriteRenderer cursor;

        // Hover
        public Vector3 HitPoint { get; private set; }
        public IInteractable CurrentTarget { get; private set; }
        public GameObject CurrentTargetGameObject { get; private set; }
        public UnityEvent<IInteractable, GameObject> OnHoverEnter { get; private set; }
        public UnityEvent<IInteractable, GameObject> OnHoverExit { get; private set; }

        [Header("[Click]")]
        [SerializeField]
        public bool useDefaultClickEvent = true;
        public bool UseDefaultClickEvent(bool value) => useDefaultClickEvent = value;
        public static float PrevRightTrigger { get; private set; } = 0.0f;

        public override void Init()
        {
            OnHoverEnter = new UnityEvent<IInteractable, GameObject>();
            OnHoverExit = new UnityEvent<IInteractable, GameObject>();

            CurrentTarget = null;
            CurrentTargetGameObject = null;

            if (Instance == null)
                Instance = this;
        }

        private void Update()
        {
            if (rightHand == null)
                rightHand = IncaMainManager.Instance.EnvironmentalUserObjects.userRightHand.transform;

            UpdateTarget();
            UpdateInput();
            UpdateCursor();
        }

        private void LateUpdate()
        {
            PrevRightTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        }

        private void UpdateTarget()
        {
            Ray ray = new Ray(rightHand.position, rightHand.forward);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, rayMaxDistance, targetLayerMask);
            HitPoint = hitInfo.point;

            // When ray has no hits.
            if (!hit)
            {
                IInteractable temp = CurrentTarget;
                GameObject tempGobj = CurrentTargetGameObject;
                CurrentTarget = null;
                CurrentTargetGameObject = null;

                if (temp == null) return;

                temp.OnHoverExit();
                OnHoverExit.Invoke(temp, tempGobj);
                return;
            }

            // When ray has a hit.
            bool hasInteractable = hitInfo.collider.TryGetComponent<IInteractable>(out IInteractable newTarget);

            // If It is not interactable.
            if (hasInteractable == false || newTarget.IsInteractable() == false)
            {
                IInteractable temp = CurrentTarget;
                GameObject tempGobj = CurrentTargetGameObject;
                CurrentTarget = null;
                CurrentTargetGameObject = null;

                if (temp != null)
                {
                    temp.OnHoverExit();
                    OnHoverExit.Invoke(temp, tempGobj);
                }

                return;
            }

            if (newTarget == CurrentTarget) return;

            IInteractable prevTarget = CurrentTarget;
            CurrentTarget = newTarget;
            GameObject prevTargetGobj = CurrentTargetGameObject;
            CurrentTargetGameObject = hitInfo.collider.gameObject;

            if (prevTarget != null)
            {
                prevTarget.OnHoverExit();
                OnHoverExit.Invoke(prevTarget, prevTargetGobj);
            }

            CurrentTarget.OnHoverEnter();
            OnHoverEnter.Invoke(CurrentTarget, CurrentTargetGameObject);
        }

        private void UpdateInput()
        {
            if (useDefaultClickEvent == false) return;

            if (IncaInput.GetButtonDown(IncaButtonCode.RightTrigger))
                CurrentTarget?.OnClick();
        }

        private void UpdateCursor()
        {
            float dist = CurrentTarget != null ? Vector3.Distance(rightHand.position, HitPoint) : defaultCursorDistance;
            cursor.transform.position = rightHand.transform.position + rightHand.transform.forward * dist;

            cursor.color = CurrentTarget != null ? Color.red : Color.blue;
        }

        private void OnDrawGizmos()
        {
            if (rightHand == null) return;

            Gizmos.color = Color.yellow;

            if (CurrentTarget != null)
            {
                Gizmos.DrawSphere(HitPoint, 0.3f);
                Gizmos.DrawLine(rightHand.position, HitPoint);
            }
            else
            {
                Vector3 pos = rightHand.position + rightHand.forward * defaultCursorDistance;
                Gizmos.DrawWireSphere(pos, 0.3f);
                Gizmos.DrawLine(rightHand.position, pos);
            }
        }
    }
}
