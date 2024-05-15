using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum TrafficLightState { Red, Orange, Green, GreenLeft }

public class TrafficLight : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField]
    private List<TrafficLightSetting> settings = new List<TrafficLightSetting>();

    [Header("Phase")]
    [SerializeField]
    private List<TrafficLightPhase> phases = new List<TrafficLightPhase>();

    private int currentPhaseIndex = 0;
    TrafficLightPhase currentPhase = null;

    private void Awake()
    {
        foreach (var setting in settings)
            setting.Deactivate();

        currentPhase = phases[currentPhaseIndex];
        currentPhase.Setup(settings);
    }

    private void Update()
    {
        bool turnOn = currentPhase.Excute();

        if (!turnOn)
        {
            currentPhaseIndex++;
            if (currentPhaseIndex >= phases.Count) currentPhaseIndex = 0;

            currentPhase = phases[currentPhaseIndex];
            currentPhase.Setup(settings);
        }
    }
}

[Serializable]
public class TrafficLightSetting
{
    [SerializeField]
    private TrafficLightState trafficLightState;
    public TrafficLightState TrafficLightState => trafficLightState;
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Material activatedMaterial;
    [SerializeField]
    private Material deactivatedMeterial;
    [SerializeField]
    private UnityEvent onActivate;
    [SerializeField]
    private UnityEvent onDeactivate;

    public void Activate()
    {
        meshRenderer.material = activatedMaterial;
        onActivate.Invoke();
    }

    public void Deactivate()
    {
        meshRenderer.material = deactivatedMeterial;
        onDeactivate.Invoke();
    }
}

/*
[Serializable]
public class TrafficLightSequence
{
    [SerializeField]
    private List<TrafficLightPhase> phases = new List<TrafficLightPhase>();

    private int currentPhaseIndex = 0;
    TrafficLightPhase currentPhase = null;

    public void Excute()
    {
        if (currentPhase == null)
            currentPhase = phases[currentPhaseIndex];

        currentPhase.Excute();
    }
}
*/

[Serializable]
public class TrafficLightPhase
{
    [SerializeField]
    private List<TrafficLightInfo> trafficLightInfos = new List<TrafficLightInfo>();
    [SerializeField]
    private float time = 0;
    private float timer = 0;
    private bool turnOn = false;

    private List<TrafficLightSetting> settings;

    public void Setup(List<TrafficLightSetting> settings)
    {
        timer = 0;
        turnOn = true;
        this.settings = settings;
        TurnOn();
    }

    public bool Excute()
    {
        timer += Time.deltaTime;
        if (turnOn && timer > time)
        {
            turnOn = false;
            TurnOff();
        }

        return turnOn;
    }

    private void TurnOn()
    {
        foreach (var info in trafficLightInfos)
            foreach (var setting in settings)
                if (info.TrafficLightState == setting.TrafficLightState)
                    setting.Activate();
    }

    private void TurnOff()
    {
        foreach (var setting in settings)
            setting.Deactivate();
    }
}

[Serializable]
public class TrafficLightInfo
{
    [SerializeField]
    private TrafficLightState trafficLightState;
    public TrafficLightState TrafficLightState => trafficLightState;
}
