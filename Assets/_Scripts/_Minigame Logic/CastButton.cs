using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastButton : MonoBehaviour
{
    [SerializeField] private FishingRod fishingRod;

    public void OnCastPressed()
    {
        fishingRod.Cast();
    }
}
