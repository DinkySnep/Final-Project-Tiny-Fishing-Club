using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmMinigame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform needle;
    [SerializeField] private Image successZone;
    [SerializeField] private Image[] hookIcons; // CHANGED (was Transform[])
    [SerializeField] private TMPro.TextMeshProUGUI feedbackText;

    [Header("Rotation")]
    [SerializeField] private float needleSpeed = 120f;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 180f;

    [Header("Zone Settings")]
    [SerializeField] private float zoneSize = 20f;

    [Header("Gameplay")]
    [SerializeField] private int maxHooks = 3;

    [Header("Feedback Colors")] // ADDED
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;

    [Header("Timing")] // ADDED
    [SerializeField] private float endDelay = 0.75f;

    private float currentAngle;
    private float direction = 1f;

    private float zoneStart;
    private float zoneEnd;
    private float zoneCenter;

    private int hooksUsed = 0;
    private int successfulHits = 0;

    private bool isActive = false;

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

        needle.localRotation = Quaternion.Euler(0, 0, -currentAngle + 90f);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            EvaluateHit();
        }
    }

    private void EvaluateHit()
    {
        float offset = currentAngle - zoneCenter;
        float absOffset = Mathf.Abs(offset);

        bool success = absOffset <= (zoneSize / 2f);

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

    private void OnSuccessfulHit(float ms)
    {
        successfulHits++;
        hooksUsed++;

        int index = hooksUsed - 1; // CHANGED
        if (index >= 0 && index < hookIcons.Length)
            hookIcons[index].color = successColor;

        ShowFeedback($"HIT\n{ms:0} ms", Color.green);

        CheckEndCondition();
    }

    private void OnFailedHit(float ms)
    {
        hooksUsed++;

        int index = hooksUsed - 1;
        if (index >= 0 && index < hookIcons.Length)
            hookIcons[index].color = failColor;

        ShowFeedback($"MISS\n{ms:0} ms", Color.red);

        CheckEndCondition();
    }

    private void CheckEndCondition()
    {
        if (hooksUsed >= maxHooks)
        {
            ResolveMinigame();
            return;
        }

        GenerateZone();
    }

    private void ResolveMinigame()
    {
        isActive = false;
        StartCoroutine(ResolveRoutine()); // CHANGED
    }

    private IEnumerator ResolveRoutine() // ADDED
    {
        if (successfulHits == 1)
            ShowFeedback("OK", Color.yellow);
        else if (successfulHits == 2)
            ShowFeedback("GOOD", Color.green);
        else if (successfulHits == 3)
            ShowFeedback("PERFECT", Color.cyan);
        else
            ShowFeedback("MISS", Color.red);

        yield return new WaitForSeconds(endDelay);

        if (successfulHits == 0)
            EventBus.OnHookFail?.Invoke();
        else
            EventBus.OnHookSuccess?.Invoke();
    }

    private void GenerateZone()
    {
        float start = Random.Range(minAngle, maxAngle - zoneSize);

        zoneStart = start;
        zoneEnd = start + zoneSize;
        zoneCenter = (zoneStart + zoneEnd) / 2f;

        successZone.rectTransform.localRotation = Quaternion.Euler(0, 0, -zoneStart);
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

        foreach (var h in hookIcons)
        {
            h.color = defaultColor; // CHANGED
            h.gameObject.SetActive(true);
        }

        feedbackText.text = "";
    }
}