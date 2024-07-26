using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Transform player;
    public Transform Player => player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
