using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rhythm Minigame:
/// Player must press input when the needle is inside a randomly generated zone.
/// 3 hooks total. At least 1 success = win.
/// </summary>
public class RhythmMinigame : MonoBehaviour
{
    // private void Update()
    // {
    //     if (!gameObject.activeSelf) return;

    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         // testing minigame, spacebar is a success
    //         Debug.Log("Hook Success");
    //         EventBus.OnHookSuccess?.Invoke();
    //     }

    //     if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         // testing minigame, 'F' key is a fail
    //         Debug.Log("Hook Fail");
    //         EventBus.OnHookFail?.Invoke();
    //     }
    // }

    [Header("References")]
    [SerializeField] private RectTransform needle;          // rotating UI needle
    [SerializeField] private RectTransform successZone;     // visual zone indicator
    [SerializeField] private Transform[] hookIcons;         // 3 hook UI icons
    [SerializeField] private TMPro.TextMeshProUGUI feedbackText;

    [Header("Rotation")]
    [SerializeField] private float needleSpeed = 120f;     // degrees per second
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 180f;        // semicircle range

    [Header("Zone Settings")]
    [SerializeField] private float zoneSize = 20f;         // degrees width

    [Header("Gameplay")]
    [SerializeField] private int maxHooks = 3;

    // internal state
    private float currentAngle;
    private float direction = 1f;

    private float zoneStart;
    private float zoneEnd;

    private int hooksUsed = 0;
    private int successfulHits = 0;

    private bool isActive = false;

    // ---------------------------
    // UNITY LOOP
    // ---------------------------
    private void OnEnable()
    {
        ResetMinigame();
        GenerateZone();
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        HandleNeedleMovement();
        HandleInput();
    }

    // ---------------------------
    // NEEDLE MOVEMENT (PING PONG)
    // ---------------------------
    private void HandleNeedleMovement()
    {
        currentAngle += direction * needleSpeed * Time.deltaTime;

        if (currentAngle >= maxAngle)
        {
            currentAngle = maxAngle;
            direction = -1f;
        }
        else if (currentAngle <= minAngle)
        {
            currentAngle = minAngle;
            direction = 1f;
        }

        needle.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }

    // ---------------------------
    // INPUT HANDLING
    // ---------------------------
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            EvaluateHit();
        }
    }

    // ---------------------------
    // HIT EVALUATION
    // ---------------------------
    private void EvaluateHit()
    {
        // Check if needle is inside success zone
        bool success = currentAngle >= zoneStart && currentAngle <= zoneEnd;

        if (success)
        {
            OnSuccessfulHit();
        }
        else
        {
            OnFailedHit();
        }
    }

    // ---------------------------
    // SUCCESS LOGIC
    // ---------------------------
    private void OnSuccessfulHit()
    {
        successfulHits++;

        // Light up a hook (visual feedback)
        if (successfulHits <= hookIcons.Length)
            hookIcons[successfulHits - 1].gameObject.SetActive(true);

        ShowFeedback("GOOD!", Color.green);

        // Trigger EVENT:
        // Tells the rest of the game (MinigameManager) to move to reel phase
        EventBus.OnHookSuccess?.Invoke();

        EndMinigame();
    }

    // ---------------------------
    // FAILURE LOGIC
    // ---------------------------
    private void OnFailedHit()
    {
        hooksUsed++;

        // Remove a hook visually
        int index = hooksUsed - 1;
        if (index >= 0 && index < hookIcons.Length)
            hookIcons[index].gameObject.SetActive(false);

        ShowFeedback("MISS", Color.red);

        // If player runs out of hooks → fail minigame
        if (hooksUsed >= maxHooks)
        {
            EventBus.OnHookFail?.Invoke();
            EndMinigame();
            return;
        }

        // Otherwise regenerate zone for next attempt
        GenerateZone();
    }

    // ---------------------------
    // ZONE GENERATION
    // ---------------------------
    private void GenerateZone()
    {
        float start = Random.Range(minAngle, maxAngle - zoneSize);

        zoneStart = start;
        zoneEnd = start + zoneSize;

        // Position UI zone (rotation-based)
        successZone.localRotation = Quaternion.Euler(0, 0, zoneStart);
        successZone.sizeDelta = new Vector2(zoneSize * 2f, successZone.sizeDelta.y);
    }

    // ---------------------------
    // FEEDBACK SYSTEM
    // ---------------------------
    private void ShowFeedback(string text, Color color)
    {
        feedbackText.text = text;
        feedbackText.color = color;

        // Could later be expanded into fade-out animation
    }

    // ---------------------------
    // RESET
    // ---------------------------
    private void ResetMinigame()
    {
        hooksUsed = 0;
        successfulHits = 0;
        currentAngle = 0f;
        direction = 1f;

        // Reset hooks visually
        foreach (var h in hookIcons)
            h.gameObject.SetActive(true);

        feedbackText.text = "";
    }

    // ---------------------------
    // END MINIGAME
    // ---------------------------
    private void EndMinigame()
    {
        isActive = false;

        /*
         EVENT PURPOSE:
         These events are already used by MinigameManager.

         OnHookSuccess → transitions to Reel Minigame
         OnHookFail    → returns to Idle state

         This script does NOT control game flow.
         It only reports results.
        */
    }
}
