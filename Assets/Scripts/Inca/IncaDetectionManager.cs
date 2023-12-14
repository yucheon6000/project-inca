using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inca
{
    public class IncaDetectionManager : IncaManager
    {
        [SerializeField]
        private SphereCollider lidarCollider;

        private List<DetectableObject> detectableObjects = new List<DetectableObject>();

        private void AddDetectableObeject(DetectableObject detectableObject)
        {
            if (detectableObjects.Contains(detectableObject)) return;

            detectableObjects.Add(detectableObject);
        }

        private void RemoveDetectableObject(DetectableObject detectableObject)
        {
            detectableObjects.Remove(detectableObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<DetectableObject>(out DetectableObject obj))
            {
                AddDetectableObeject(obj);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<DetectableObject>(out DetectableObject obj))
            {
                RemoveDetectableObject(obj);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lidarCollider.transform.position, lidarCollider.radius);

            Gizmos.color = Color.red;
            foreach (DetectableObject obj in detectableObjects)
            {
                Gizmos.DrawWireCube(obj.transform.position, obj.transform.localScale);
            }
        }
    }
}
