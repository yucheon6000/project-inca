using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarState : MonoBehaviour
{
    public abstract void Enter(Car car);
    public abstract void Excute(Car car);
    public abstract void Exit(Car car);
}
