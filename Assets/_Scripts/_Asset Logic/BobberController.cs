using System.Collections;
using UnityEngine;

// handles all bobber visual behavior (idle, casting, floating, bite reaction)
// this system is event-driven and does not control gameplay
public class BobberController : MonoBehaviour
{
    public enum BobberState
    {
        Idle,
        Casting,
        Waiting,
        Returning
    }

    private BobberState state = BobberState.Idle;

    [Header("References")]
    public Transform rodTip;              // where bobber rests when idle
    public Transform waterTarget;         // where bobber lands in water
    public GameObject exclamationMark;    // bite indicator

    [Header("Cast Settings")]
    public float castDuration = 0.6f;
    public float arcHeight = 2f;

    [Header("Idle Motion")]
    public float idleBobAmplitude = 0.05f;
    public float idleBobSpeed = 2f;

    [Header("Water Motion")]
    public float waterBobAmplitude = 0.1f;
    public float waterBobSpeed = 1.5f;

    [Header("Line")]
    public LineRenderer lineRenderer;

    private void OnEnable()
    {
        EventBus.OnCastStarted += HandleCastStarted;
        EventBus.OnFishBite += HandleFishBite;
        EventBus.OnHookFail += HandleEnd;
        EventBus.OnReelFail += HandleEnd;
        EventBus.OnReelSuccess += HandleEnd;
    }

    private void OnDisable()
    {
        EventBus.OnCastStarted -= HandleCastStarted;
        EventBus.OnFishBite -= HandleFishBite;
        EventBus.OnHookFail -= HandleEnd;
        EventBus.OnReelFail -= HandleEnd;
        EventBus.OnReelSuccess -= HandleEnd;
    }

    private void Start()
    {
        exclamationMark.SetActive(false); // ensure hidden at start
    }

    private void Update()
    {
        UpdateLine();
        // idle bobbing near rod tip
        if (state == BobberState.Idle)
        {
            float yOffset = Mathf.Sin(Time.time * idleBobSpeed) * idleBobAmplitude;
            transform.position = rodTip.position + new Vector3(0, yOffset - 0.5f, 0);
        }

        // floating on water
        if (state == BobberState.Waiting)
        {
            float yOffset = Mathf.Sin(Time.time * waterBobSpeed) * waterBobAmplitude;
            transform.position = waterTarget.position + new Vector3(0, yOffset + 0.05f, 0);
        }
    }

    // --- EVENT HANDLERS ---

    private void HandleCastStarted()
    {
        StartCoroutine(CastRoutine());
    }

    private void HandleFishBite()
    {
        exclamationMark.SetActive(true); // show bite feedback
        StartCoroutine(BiteBounceRoutine());
    }

    private void HandleEnd()
    {
        StartCoroutine(ReturnRoutine());
    }

    // handles bobber arc when casting
    private IEnumerator CastRoutine()
    {
        state = BobberState.Casting;

        Vector3 start = rodTip.position;
        Vector3 end = waterTarget.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / castDuration;

            Vector3 pos = Vector3.Lerp(start, end, t);

            // arc motion
            pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

            transform.position = pos;

            yield return null;
        }

        state = BobberState.Waiting;
    }

    // small bounce when fish bites
    private IEnumerator BiteBounceRoutine()
    {
        float duration = 0.3f;
        float t = 0f;

        Vector3 basePos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float dip = Mathf.Sin(t * Mathf.PI) * 0.3f;
            transform.position = basePos - new Vector3(0, dip, 0);

            yield return null;
        }
    }

    // returns bobber back to rod after minigame ends
    private IEnumerator ReturnRoutine()
    {
        state = BobberState.Returning;

        Vector3 start = transform.position;
        Vector3 end = rodTip.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            transform.position = Vector3.Lerp(start, end, t);

            yield return null;
        }

        exclamationMark.SetActive(false); // hide indicator
        state = BobberState.Idle;
    }
    private void UpdateLine()
    {
        if (lineRenderer == null || rodTip == null) return;

        lineRenderer.SetPosition(0, rodTip.position);
        lineRenderer.SetPosition(1, transform.position);
    }
}