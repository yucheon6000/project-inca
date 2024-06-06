using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class ModeSwitch : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onChangToVrMode = new UnityEvent();
    [SerializeField]
    private UnityEvent onChangToMonitorMode = new UnityEvent();

    public void ChangeToVrMode()
    {
        onChangToVrMode.Invoke();
    }

    public void ChangeToMonitorMode()
    {
        onChangToMonitorMode.Invoke();
    }
}
