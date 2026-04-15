using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Class for all fishing rods
// This is responsable for handling the info needed when the player casts their line
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

        // trigger small rod flick animation
        StartCoroutine(FlickRoutine());

        Debug.Log("Cast line");
        EventBus.OnCastStarted?.Invoke();
        GameManager.Instance.SetState(GameState.Casting);

        yield return new WaitForSeconds(0.5f);

        GameManager.Instance.SetState(GameState.WaitingForBite);

        yield return new WaitForSeconds(castCooldown);
        canCast = true;
    }

    // simple rod flick animation for visual feedback when casting
    private IEnumerator FlickRoutine()
    {
        float duration = 0.15f;
        float t = 0f;

        Quaternion startRot = transform.localRotation;
        Quaternion flickRot = startRot * Quaternion.Euler(-25f, 0f, 0f);

        // flick forward
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localRotation = Quaternion.Slerp(startRot, flickRot, t);
            yield return null;
        }

        t = 0f;

        // return to original rotation
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localRotation = Quaternion.Slerp(flickRot, startRot, t);
            yield return null;
        }
    }

}