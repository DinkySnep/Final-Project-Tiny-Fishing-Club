using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private GameObject rhythmUI;
    [SerializeField] private GameObject reelUI;
    [SerializeField] private GameObject resultUI;

    private void OnEnable()
    {
        EventBus.OnFishBite += StartHookMinigame;
        EventBus.OnHookSuccess += StartReelMinigame;
        EventBus.OnHookFail += HandleFail;
        EventBus.OnReelSuccess += HandleCatch;
        EventBus.OnReelFail += HandleFail;
    }

    private void OnDisable()
    {
        EventBus.OnFishBite -= StartHookMinigame;
        EventBus.OnHookSuccess -= StartReelMinigame;
        EventBus.OnHookFail -= HandleFail;
        EventBus.OnReelSuccess -= HandleCatch;
        EventBus.OnReelFail -= HandleFail;
    }

    private void StartHookMinigame()
    {
        GameManager.Instance.SetState(GameState.HookMinigame);
        rhythmUI.SetActive(true);
    }

    private void StartReelMinigame()
    {
        rhythmUI.SetActive(false);

        GameManager.Instance.SetState(GameState.ReelMinigame);
        reelUI.SetActive(true);
    }

    private void HandleCatch()
    {
        reelUI.SetActive(false);
        rhythmUI.SetActive(false);

        resultUI.SetActive(true);

        GameManager.Instance.SetState(GameState.CatchResult);

        Debug.Log("Fish Caught!");
    }

    private void HandleFail()
    {
        rhythmUI.SetActive(false);
        reelUI.SetActive(false);
        resultUI.SetActive(false);

        GameManager.Instance.SetState(GameState.Idle);

        Debug.Log("Fish Lost");
    }
}
