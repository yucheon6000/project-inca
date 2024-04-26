using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Shotput_Animation : MonoBehaviour
{
    [SerializeField]
    private Enemy_Shotput main;

    public void Init()
    {
        main.Init();
    }

    public void Attack()
    {
        if (main == null) Debug.LogError("Enemy_Shotput_Animation: It can't find main component.");

        main.Attack();
    }
}
