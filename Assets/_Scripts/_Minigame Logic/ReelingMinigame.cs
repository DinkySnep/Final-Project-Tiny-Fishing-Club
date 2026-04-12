using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Reeling Minigame:
/// Player controls a vertical bar to keep a moving fish inside their zone.
/// Progress increases while aligned, decreases otherwise.
/// </summary>
public class ReelingMinigame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform playerBar;     // player-ontrolled bar
    [SerializeField] private RectTransform fish;          // fish icon
    [SerializeField] private Image progressFill;          // progress bar fill
    [SerializeField] private Image playerBarImage;        // color feedback

    [Header("Bounds")]
    [SerializeField] private RectTransform playArea;      // main vertical container for the play space

    // movemnt of the player all can be controlled in the inspector
    [Header("Player Movement")]
    [SerializeField] private float riseSpeed = 200f;
    [SerializeField] private float fallSpeed = 150f;

    // movement for the fish, can be changed for dificulty
    [Header("Fish Movement")]
    [SerializeField] private float fishMoveSpeed = 100f;
    [SerializeField] private float fishChangeInterval = 1.5f;

    // the stuf that is the players goal ot fill
    [Header("Progress")]
    [SerializeField] private float progressGainRate = 0.25f;
    [SerializeField] private float progressLossRate = 0.2f;
    [SerializeField] private float startProgress = 0.33f;

    // colors that can be edited in the inspector for feedback
    [Header("Feedback Colors")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color idleColor = Color.white;

    // internal mingame states
    private float playerY;
    private float fishY;
    private float fishVelocity;

    private float progress;

    private float minY;
    private float maxY;

    private bool isActive = false;

    private void OnEnable()
    {
        Initialize();
        StartCoroutine(FishMovementRoutine());
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        HandlePlayerMovement();
        UpdatePositions();
        EvaluateCatch();
    }

    // initalize the minigame
    private void Initialize()
    {
        float height = playArea.rect.height;

        minY = -height * 0.5f;
        maxY = height * 0.5f;

        playerY = height * 0f;
        fishY = height * 0f;

        progress = startProgress;
        UpdateProgressUI();
    }

    // logic for when the player interacts with the game
    private void HandlePlayerMovement()
    {
        bool inputHeld = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

        if (inputHeld)
        {
            playerY += riseSpeed * Time.deltaTime;
        }
        else
        {
            playerY -= fallSpeed * Time.deltaTime;
        }

        playerY = Mathf.Clamp(playerY, minY, maxY);
    }

    // How the fish moves 
    private IEnumerator FishMovementRoutine()
    {
        while (true)
        {
            // pick a random vertical velocity
            fishVelocity = Random.Range(-fishMoveSpeed, fishMoveSpeed);

            yield return new WaitForSeconds(fishChangeInterval);
        }
    }
    private void UpdateFish()
    {
        fishY += fishVelocity * Time.deltaTime;

        // clamp fish inside play bounds
        if (fishY <= minY || fishY >= maxY)
        {
            fishVelocity *= -1f;
        }

        fishY = Mathf.Clamp(fishY, minY, maxY);
    }


    private void UpdatePositions()
    {
        UpdateFish();

        playerBar.anchoredPosition = new Vector2(playerBar.anchoredPosition.x, playerY);
        fish.anchoredPosition = new Vector2(fish.anchoredPosition.x, fishY);
    }

    // evaluate the players interaction
    private void EvaluateCatch()
    {
        float playerTop = playerY + (playerBar.rect.height / 2f);
        float playerBottom = playerY - (playerBar.rect.height / 2f);

        bool fishInside = fishY >= playerBottom && fishY <= playerTop;

        if (fishInside)
        {
            progress += progressGainRate * Time.deltaTime;
            playerBarImage.color = activeColor;
        }
        else
        {
            progress -= progressLossRate * Time.deltaTime;
            playerBarImage.color = idleColor;
        }

        progress = Mathf.Clamp01(progress);
        UpdateProgressUI();

        // Win condition
        if (progress >= 1f)
        {
            EndMinigame(true);
        }
        // Lose condition
        else if (progress <= 0f)
        {
            EndMinigame(false);
        }
    }

    // ui for the filled bar
    private void UpdateProgressUI()
    {
        progressFill.fillAmount = progress;
    }

    // ending the minigame
    private void EndMinigame(bool success)
    {
        isActive = false;

        if (success)
            EventBus.OnReelSuccess?.Invoke();
        else
            EventBus.OnReelFail?.Invoke();
    }
}