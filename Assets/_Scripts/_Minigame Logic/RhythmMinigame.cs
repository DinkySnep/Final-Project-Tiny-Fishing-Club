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
            // testing minigame, spacebar is a success
            Debug.Log("Hook Success");
            EventBus.OnHookSuccess?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            // testing minigame, 'F' key is a fail
            Debug.Log("Hook Fail");
            EventBus.OnHookFail?.Invoke();
        }
    }
}
