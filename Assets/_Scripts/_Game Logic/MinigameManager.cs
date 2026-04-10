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
        // start the rhythm game
        GameManager.Instance.SetState(GameState.HookMinigame);
        rhythmUI.SetActive(true);
    }

    private void StartReelMinigame()
    {
        // end the rhythm game
        rhythmUI.SetActive(false);
        // start the reeling minigame
        GameManager.Instance.SetState(GameState.ReelMinigame);
        reelUI.SetActive(true);
    }

    private void HandleCatch()
    {
        // disable the minigame ui
        reelUI.SetActive(false);
        rhythmUI.SetActive(false);

        // show the fish caught
        resultUI.SetActive(true);
        GameManager.Instance.SetState(GameState.CatchResult);

        Debug.Log("Fish Caught!");
    }

    private void HandleFail()
    {
        // make sure the minigame ui is cleared
        rhythmUI.SetActive(false);
        reelUI.SetActive(false);
        resultUI.SetActive(false);

        // let the player cast another line
        GameManager.Instance.SetState(GameState.Idle);

        Debug.Log("Fish Lost");
    }
}
