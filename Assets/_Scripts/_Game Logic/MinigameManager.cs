using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private GameObject rhythmUI;
    [SerializeField] private GameObject reelUI;
    [SerializeField] private GameObject resultUI;

    private FishSystem fishSystem; // reference to fish system

    private void Awake()
    {
        fishSystem = FindObjectOfType<FishSystem>(); // get fish system in scene
    }

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

        // resolve the fish that was caught
        FishInstance fish = fishSystem.ResolveCatch();

        // add fish to player inventory
        PlayerData.Instance.AddFish(fish);

        // send fish to result screen
        resultUI.GetComponent<CatchResultScreen>().ShowResult(fish);
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