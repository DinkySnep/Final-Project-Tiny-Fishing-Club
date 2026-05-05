using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastButton : MonoBehaviour
{
    [SerializeField] private FishingRod fishingRod;
    public AudioClip castSound;

    public void OnCastPressed()
    {
        fishingRod.Cast();  // player cast a line
        AudioSource.PlayClipAtPoint(castSound, Camera.main.transform.position);
    }
}
