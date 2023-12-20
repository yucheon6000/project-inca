using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inca
{
    public class IncaDetectionManager : IncaManager
    {
        [SerializeField]
        private SphereCollider lidarCollider;

        private List<EnvironmentObject> environmentObjects = new List<EnvironmentObject>();

        private void AddDetectableObeject(EnvironmentObject environmentObject)
        {
            if (environmentObjects.Contains(environmentObject)) return;

            environmentObjects.Add(environmentObject);
        }

        private void RemoveenvironmentObject(EnvironmentObject environmentObject)
        {
            environmentObjects.Remove(environmentObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EnvironmentObject>(out EnvironmentObject obj))
            {
                obj.TurnGizmos(true);
                AddDetectableObeject(obj);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<EnvironmentObject>(out EnvironmentObject obj))
            {
                obj.TurnGizmos(false);
                RemoveenvironmentObject(obj);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lidarCollider.transform.position, lidarCollider.radius);
        }
    }
}
