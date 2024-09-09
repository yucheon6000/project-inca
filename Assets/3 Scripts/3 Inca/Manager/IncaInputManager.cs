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

        [Header("[Cursor]")]
        [SerializeField]
        private float defaultCursorDistance;
        [SerializeField]
        private float cursorOffset = 0.1f;      // Amount to position the cursor in front of the hit point
        private Transform rightHand;
        [SerializeField]
        private SpriteRenderer cursor;
        [SerializeField]
        public float cursorSmoothSpeed = 0.1f;
        Vector3 cursorVelocity = Vector3.zero;

        [Space]
        [SerializeField]
        private Color defaultCursorColor;
        [SerializeField]
        private Color activeCursorColor;

        // Hover
        public Vector3 HitPoint { get; private set; }
        public IInteractable CurrentTarget { get; private set; }
        public GameObject CurrentTargetGameObject { get; private set; }
        public bool HasCurrentTarget => CurrentTarget != null;
        public UnityEvent<IInteractable, GameObject> OnHoverEnter { get; private set; }
        public UnityEvent<IInteractable, GameObject> OnHoverExit { get; private set; }

        [Header("[Click]")]
        [SerializeField]
        public bool useDefaultClickEvent = true;
        public bool UseDefaultClickEvent(bool value) => useDefaultClickEvent = value;
        public static float PrevRightTrigger { get; private set; } = 0.0f;

        [Header("[@For Mouse]")]
        [SerializeField]
        private RightHandControlByMouse rightHandControlByMouse;

        private void Awake()
        {
            if (rightHandControlByMouse == null)
                rightHandControlByMouse = FindObjectOfType<RightHandControlByMouse>();
        }

        public override void Init()
        {
            SetDefaultCursorDistance(defaultCursorDistance);

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

            UpdateClick();

            UpdateCursorPosition();
            UpdateCursorColor();
        }

        private void LateUpdate()
        {
            PrevRightTrigger = Mathf.Max(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger), OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger));
        }

        private void UpdateTarget()
        {
            Ray ray = new Ray(rightHand.position, rightHand.forward);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, rayMaxDistance, targetLayerMask);
            HitPoint = hitInfo.point;

            // When ray has no hits.
            if (hit == false)
            {
                ProcessHoverExit();
                return;
            }

            // When ray has a hit.
            bool hasInteractable = hitInfo.collider.TryGetComponent<IInteractable>(out IInteractable newTarget);

            // If it's not interactable.
            if (hasInteractable == false || newTarget.IsInteractable() == false)
            {
                ProcessHoverExit();
                return;
            }

            // If it's interactable and new.
            if (newTarget != CurrentTarget)
                ProcessHoverEnter(newTarget, hitInfo.collider.gameObject);
        }

        private void ProcessHoverExit()
        {
            IInteractable prevTarget = CurrentTarget;
            GameObject prevTargetGobj = CurrentTargetGameObject;

            CurrentTarget = null;
            CurrentTargetGameObject = null;

            if (prevTarget == null) return;

            prevTarget.OnHoverExit();
            OnHoverExit.Invoke(prevTarget, prevTargetGobj);
            return;
        }

        private void ProcessHoverEnter(IInteractable newTarget, GameObject newTargetGameObject)
        {
            IInteractable prevTarget = CurrentTarget;
            GameObject prevTargetGobj = CurrentTargetGameObject;

            CurrentTarget = newTarget;
            CurrentTargetGameObject = newTargetGameObject;

            if (prevTarget != null)
            {
                prevTarget.OnHoverExit();
                OnHoverExit.Invoke(prevTarget, prevTargetGobj);
            }

            CurrentTarget.OnHoverEnter();
            OnHoverEnter.Invoke(CurrentTarget, CurrentTargetGameObject);
        }

        private void UpdateClick()
        {
            if (useDefaultClickEvent == false) return;

            if (IncaInput.GetButtonDown(IncaButtonCode.RightTrigger))
                CurrentTarget?.OnClick();
        }

        private void UpdateCursorPosition()
        {

            float dist = HasCurrentTarget
                            ? Vector3.Distance(rightHand.position, HitPoint) - cursorOffset
                            : defaultCursorDistance;

            Vector3 targetPos = rightHand.transform.position + rightHand.transform.forward * dist;

            cursor.transform.position = Vector3.SmoothDamp(cursor.transform.position, targetPos, ref cursorVelocity, cursorSmoothSpeed);
        }

        private void UpdateCursorColor()
        {
            cursor.color = HasCurrentTarget ? activeCursorColor : defaultCursorColor;
        }

        private void OnDrawGizmos()
        {
            if (rightHand == null) return;

            Gizmos.color = Color.yellow;

            if (HasCurrentTarget)
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

        /* Setters */
        public void SetDefaultCursorDistance(float value)
        {
            defaultCursorDistance = value;
            rightHandControlByMouse.SetMousePosZ(value);
        }

        public void SetCursorSprite(Sprite sprite)
            => cursor.sprite = sprite;

        public void SetCursorScale(float scale)
            => SetCursorScale(Vector3.one * scale);

        public void SetCursorScale(Vector3 scale)
            => cursor.transform.localScale = scale;

        public void SetDefaultCursorColor(Color color)
            => defaultCursorColor = color;

        public void SetActiveCursorColor(Color color)
            => activeCursorColor = color;
    }
}
