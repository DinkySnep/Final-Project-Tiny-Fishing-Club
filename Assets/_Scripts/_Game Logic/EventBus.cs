using System;

public static class EventBus
{
    public static Action OnCastStarted;
    public static Action OnFishBite;

    public static Action OnHookSuccess;
    public static Action OnHookFail;

    public static Action OnReelSuccess;
    public static Action OnReelFail;

    public static Action<GameState> OnGameStateChanged;
}