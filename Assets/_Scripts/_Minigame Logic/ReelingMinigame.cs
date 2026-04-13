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

    // delay before the minigame actually starts
    [Header("Start Delay")]
    [SerializeField] private float startDelay = 0.33f; // pause so player can read the screen

    // internal mingame states
    private float playerY;
    private float fishY;
    private float fishVelocity;


    private float playerVelocity;       // smooth velocity for player movement

    private float targetFishVelocity;   // target velocity for fish (smoothing)

    private float progress;

    private float minY;
    private float maxY;

    private bool isActive = false;

    private bool isPausedAtStart = true;    // gameplay puase for the begning

    private void OnEnable()
    {
        Initialize();
        StartCoroutine(FishMovementRoutine());
        StartCoroutine(StartDelayRoutine()); // start delay before gameplay begins
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

        playerVelocity = 0f; // reset velocity

        progress = startProgress;
        UpdateProgressUI();
    }

    // handle the pause before gameplay starts
    private IEnumerator StartDelayRoutine()
    {
        isPausedAtStart = true;
        isActive = false; // prevent updating the game when paused 

        yield return new WaitForSeconds(startDelay);

        isPausedAtStart = false;
        isActive = true; // start gamen
    }

    // logic for when the player interacts with the game
    private void HandlePlayerMovement()
    {
        if (isPausedAtStart) return; // no movement during start delay

        bool inputHeld = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

        // target velocity for the player bar
        float targetVelocity = inputHeld ? riseSpeed : -fallSpeed;

        // smoothing toward target
        playerVelocity = Mathf.Lerp(playerVelocity, targetVelocity, Time.deltaTime * 8f);

        // move the players bar
        playerY += playerVelocity * Time.deltaTime;

        playerY = Mathf.Clamp(playerY, minY, maxY);
    }

    // How the fish moves 
    private IEnumerator FishMovementRoutine()
    {
        while (true)
        {
            // vary speed slightly for some randomness
            float speedMultiplier = Random.Range(0.8f, 1.2f);

            // set a target velocity like the player movement
            targetFishVelocity = Random.Range(-fishMoveSpeed, fishMoveSpeed) * speedMultiplier;

            yield return new WaitForSeconds(fishChangeInterval * 0.75f);
        }
    }

    private void UpdateFish()
    {
        if (isPausedAtStart) return; // pause fish during start delay

        // smooth toward target velocity like the player but faster
        fishVelocity = Mathf.Lerp(fishVelocity, targetFishVelocity, Time.deltaTime * 12f);

        fishY += fishVelocity * Time.deltaTime;

        // prevent fish from hitting and getting stuck to a bound
        // if the fish hits or goes below the lower bound 
        if (fishY <= minY)
        {
            fishY = minY;
            // flip fish direction
            targetFishVelocity = Mathf.Abs(targetFishVelocity);
            fishVelocity = Mathf.Abs(fishVelocity);
        }
        // if the fish hits or goes above the upper bound 
        else if (fishY >= maxY)
        {
            fishY = maxY;
            // flip fish direction
            targetFishVelocity = -Mathf.Abs(targetFishVelocity);
            fishVelocity = -Mathf.Abs(fishVelocity);
        }
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
        if (isPausedAtStart) return; // no progress change during pause

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