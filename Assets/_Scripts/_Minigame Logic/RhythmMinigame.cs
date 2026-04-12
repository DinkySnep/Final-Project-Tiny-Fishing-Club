using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rhythm Minigame:
/// Player times input while a needle sweeps across a semicircle.
/// Player gets multiple attempts (hooks), and result is evaluated at the end.
/// </summary>
public class RhythmMinigame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform needle;      // the player 'character' 
    [SerializeField] private Image successZone;         // the goal zone for the player to hit the zeedle in
    [SerializeField] private Image[] hookIcons;         // UI icons for the hooks
    [SerializeField] private TMPro.TextMeshProUGUI feedbackText;    // the feedback text for the player

    [Header("Rotation")]
    [SerializeField] private float needleSpeed = 120f;  // degress / second for the needle
    [SerializeField] private float minAngle = 0f;       // left arc bound
    [SerializeField] private float maxAngle = 180f;     // right arc bound

    [Header("Zone Settings")]
    [SerializeField] private float zoneSize = 20f;      // size of success zone

    [Header("Gameplay")]
    [SerializeField] private int maxHooks = 3;          // how many hooks the player has

    [Header("Feedback Colors")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;

    [Header("Timing")]
    [SerializeField] private float endDelay = 0.75f;    // delay for player to view results

    // internal states of the minigame
    private float currentAngle;     // curr needle angle
    private float direction = 1f;   // dir of needle movement  ( clockwise or counter )

    private float zoneStart;        // start of goal
    private float zoneEnd;          // end angle of goal
    private float zoneCenter;       // center used for scoring

    private int hooksUsed = 0;      // used attempts
    private int successfulHits = 0; // successful hints

    private bool isActive = false;  // if the game is curerntly being played

    private void OnEnable()
    {
        // each time a new fish event happens we reset the state of the game
        ResetMinigame();
        GenerateZone();
        isActive = true;
    }

    private void Update()
    {
        // check if the game has started
        if (!isActive) return;

        HandleNeedleMovement();
        HandleInput();
    }

    // logic for the needle's movment
    private void HandleNeedleMovement()
    {
        // move it back and fourth like a metonome between the min & max angle
        currentAngle += direction * needleSpeed * Time.deltaTime;

        // handle the bounds fliping direction
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
        // localize the needle rotation as UI is infverted for some fucking reason
        needle.localRotation = Quaternion.Euler(0, 0, -currentAngle + 90f);
    }

    private void HandleInput()
    {
        // check for player input (both m1 / left click and or spacebar are valid)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            EvaluateHit();
        }
    }

    // hit eval from player input
    private void EvaluateHit()
    {
        // distance from the cetner of the goal
        float offset = currentAngle - zoneCenter;
        float absOffset = Mathf.Abs(offset);

        // check if inside goal
        bool success = absOffset <= (zoneSize / 2f);

        // convert to ms ofest  (i dont really think this is properly time based but the player will never know)
        float normalized = absOffset / (zoneSize / 2f);
        float ms = normalized * 100f;

        if (success)
        {
            OnSuccessfulHit(ms);
        }
        else
        {
            OnFailedHit(ms);
        }
    }

    // the player hit it in the goal zone
    private void OnSuccessfulHit(float ms)
    {
        successfulHits++;
        hooksUsed++;

        int index = hooksUsed - 1;  // an attempt has been used
        // color the hook for a success
        if (index >= 0 && index < hookIcons.Length)
            hookIcons[index].color = successColor;

        ShowFeedback($"HIT\n{ms:0} ms", Color.green);

        CheckEndCondition();
    }

    // the player did not hit the goal
    private void OnFailedHit(float ms)
    {
        hooksUsed++;

        int index = hooksUsed - 1;  // an attempt has been used
        // color the hook for failure
        if (index >= 0 && index < hookIcons.Length)
            hookIcons[index].color = failColor;

        ShowFeedback($"MISS\n{ms:0} ms", Color.red);

        CheckEndCondition();
    }

    private void CheckEndCondition()
    {
        // if the player has used all of their attempets figure out 
        // if the player won / lost and their score
        if (hooksUsed >= maxHooks)
        {
            ResolveMinigame();
            return;
        }
        // if they have more attempts left make a new zone for the next hook
        GenerateZone();
    }

    private void ResolveMinigame()
    {
        isActive = false;

        // exit delay for player to read result
        StartCoroutine(ResolveRoutine());
    }

    private IEnumerator ResolveRoutine()
    {
        // grade the players attempts
        if (successfulHits == 1)
            ShowFeedback("OK", Color.yellow);
        else if (successfulHits == 2)
            ShowFeedback("GOOD", Color.green);
        else if (successfulHits == 3)
            ShowFeedback("PERFECT", Color.cyan);
        else
            ShowFeedback("MISS", Color.red);

        // pause for transition
        yield return new WaitForSeconds(endDelay);

        // let the game state update
        if (successfulHits == 0)
            EventBus.OnHookFail?.Invoke();
        else
            EventBus.OnHookSuccess?.Invoke();
    }

    private void GenerateZone()
    {
        // radnomize zone withen valid arc range
        float start = Random.Range(minAngle, maxAngle - zoneSize);

        zoneStart = start;
        zoneEnd = start + zoneSize;
        zoneCenter = (zoneStart + zoneEnd) / 2f;
        // rotate new zone to corret pos (UI is also inverse her for some reason)
        successZone.rectTransform.localRotation = Quaternion.Euler(0, 0, -zoneStart);
        // zone size is based on radial fill not the scling 
        successZone.fillAmount = zoneSize / 360f;
    }

    private void ShowFeedback(string text, Color color)
    {
        feedbackText.text = text;
        feedbackText.color = color;
    }

    private void ResetMinigame()
    {
        hooksUsed = 0;
        successfulHits = 0;
        currentAngle = 0f;
        direction = 1f;

        // reset the hooks
        foreach (var h in hookIcons)
        {
            h.color = defaultColor;
            h.gameObject.SetActive(true);
        }

        feedbackText.text = "";
    }
}