using System.Collections;
using System.Collections.Generic;
using Inca;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Transform player;
    public Transform Player => player;

    [SerializeField]
    private GameObject enemySapwnRange;

    public bool IsGameStarted { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        IncaInputManager.Instance.SetDefaultCursorDistance(10f);

        enemySapwnRange.SetActive(true);

        IsGameStarted = true;
    }
}
