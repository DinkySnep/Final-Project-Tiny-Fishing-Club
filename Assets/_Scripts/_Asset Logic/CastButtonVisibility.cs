using UnityEngine;

// this is attached to the ui empty so that the cast buttion isn  not overlapping other ui
// and there is consistant feedback with just ui that the player is waiting as there
// are currently no anitmaitons
public class CastButtonVisibility : MonoBehaviour
{
    [SerializeField] private GameObject castButton;

    private void OnEnable()
    {
        EventBus.OnGameStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        EventBus.OnGameStateChanged -= HandleStateChanged;
    }

    private void Start()
    {
        // safety default state
        HandleStateChanged(GameManager.Instance.CurrentState);
    }

    private void HandleStateChanged(GameState state)
    {
        bool show = state == GameState.Idle;
        castButton.SetActive(show);
    }
}