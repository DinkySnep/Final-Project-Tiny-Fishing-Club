using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmMinigame : MonoBehaviour
{
    private void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Hook Success");
            EventBus.OnHookSuccess?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Hook Fail");
            EventBus.OnHookFail?.Invoke();
        }
    }
}
