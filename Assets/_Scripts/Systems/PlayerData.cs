using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public List<FishInstance> inventory = new List<FishInstance>();

    public int coins = 0;

    [Header("Stats")]
    public float luck = 0f;
    public float strength = 50f;

    private void Awake()
    {
        Instance = this;
    }

    // add a fish to the players inventory
    public void AddFish(FishInstance fish)
    {
        inventory.Add(fish);
        Debug.Log("Added fish: " + fish.data.fishName + " | Weight: " + fish.weight);
    }
}