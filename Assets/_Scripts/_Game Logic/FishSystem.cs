using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSystem : MonoBehaviour
{
    [SerializeField] private float minBiteTime = 1f;    // least ammount of time needed for a fish to bite
    [SerializeField] private float maxBiteTime = 4f;    // most ammount of time needed for a fish to bite

    private void OnEnable()
    {
        EventBus.OnCastStarted += HandleCast;
    }

    private void OnDisable()
    {
        EventBus.OnCastStarted -= HandleCast;
    }

    private void HandleCast()
    {
        StartCoroutine(BiteRoutine());
    }

    private IEnumerator BiteRoutine()
    {
        float waitTime = Random.Range(minBiteTime, maxBiteTime);    // get a random time to wait for a bite
        Debug.Log("Waiting for bite: " + waitTime);
        // wait for the bite ...
        yield return new WaitForSeconds(waitTime);
        // the fish has bit
        Debug.Log("Fish Bit!");
        EventBus.OnFishBite?.Invoke();
    }
}
