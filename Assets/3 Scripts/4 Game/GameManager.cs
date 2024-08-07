using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Transform player;
    public Transform Player => player;

    [SerializeField]
    private GameObject enemySapwnRange;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void StartGame()
    {
        enemySapwnRange.SetActive(true);
    }
}
