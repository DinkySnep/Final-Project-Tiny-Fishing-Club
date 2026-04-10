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
            // testing minigame, spacebar is a success
            Debug.Log("Reel Success");
            EventBus.OnReelSuccess?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            // testing minigame, 'F' key is a fail
            Debug.Log("Reel Fail");
            EventBus.OnReelFail?.Invoke();
        }
    }
}
