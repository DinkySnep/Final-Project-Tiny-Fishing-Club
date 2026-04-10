using System;

public static class EventBus
{
    public static Action OnCastStarted;     // the player has clicked to cast a line
    public static Action OnFishBite;        // the rng cooldown is over and a fish has bit

    public static Action OnHookSuccess;     // the player has won the rhythm minigame
    public static Action OnHookFail;        // the player has lost the rhythm minigame

    public static Action OnReelSuccess;     // the player has won the reeling minigame
    public static Action OnReelFail;        // the player has lost the reeling minigame

    public static Action<GameState> OnGameStateChanged;
}