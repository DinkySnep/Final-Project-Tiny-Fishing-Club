using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSystem : MonoBehaviour
{
    [SerializeField] private float minBiteTime = 1f;
    [SerializeField] private float maxBiteTime = 4f;

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
        float waitTime = Random.Range(minBiteTime, maxBiteTime);
        Debug.Log("Waiting for bite: " + waitTime);

        yield return new WaitForSeconds(waitTime);

        Debug.Log("Fish Bit!");
        EventBus.OnFishBite?.Invoke();
    }
}
