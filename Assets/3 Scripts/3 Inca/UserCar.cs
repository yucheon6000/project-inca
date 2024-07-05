using Inca;
using UnityEngine;

public class UserCar : MonoBehaviour
{
    private void Awake()
    {
        GameObject.FindFirstObjectByType<IncaDetectManager>().SetUserCar(transform);
    }

    [ContextMenu("SyncPosition")]
    private void SyncPosition()
    {
        transform.position = GameObject.FindFirstObjectByType<IncaDetectManager>().transform.position;
    }
}
