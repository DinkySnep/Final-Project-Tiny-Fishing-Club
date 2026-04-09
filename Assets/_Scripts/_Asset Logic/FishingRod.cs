using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] private float castCooldown = 1f;

    private bool canCast = true;

    public void Cast()
    {
        if (!canCast) return;
        if (GameManager.Instance.CurrentState != GameState.Idle) return;

        StartCoroutine(CastRoutine());
    }

    private System.Collections.IEnumerator CastRoutine()
    {
        canCast = false;

        Debug.Log("Cast line");
        EventBus.OnCastStarted?.Invoke();
        GameManager.Instance.SetState(GameState.Casting);

        yield return new WaitForSeconds(0.5f);

        GameManager.Instance.SetState(GameState.WaitingForBite);

        yield return new WaitForSeconds(castCooldown);
        canCast = true;
    }
}
