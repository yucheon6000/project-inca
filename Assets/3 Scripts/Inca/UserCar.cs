using Inca;
using UnityEngine;

public class UserCar : MonoBehaviour
{
    private void Awake()
    {
        GameObject.FindFirstObjectByType<IncaDetectManager>().SetUserCar(transform);
    }
}
