using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetState(GameState.Idle);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Game State: " + newState);

        EventBus.OnGameStateChanged?.Invoke(newState);
    }
}
