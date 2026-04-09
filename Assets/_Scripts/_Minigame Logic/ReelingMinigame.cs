using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelingMinigame : MonoBehaviour
{
    private void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Reel Success");
            EventBus.OnReelSuccess?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Reel Fail");
            EventBus.OnReelFail?.Invoke();
        }
    }
}
