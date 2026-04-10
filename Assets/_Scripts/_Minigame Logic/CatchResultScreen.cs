using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchResultScreen : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.CatchResult)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // temp code to allow the game to loop after the result screen
            Debug.Log("Continuing after succsesful catch...");
            gameObject.SetActive(false);

            GameManager.Instance.SetState(GameState.Idle);
        }
    }
}
