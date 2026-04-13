using System.Collections.Generic;
using UnityEngine;

public class FishDatabase : MonoBehaviour
{
    public static FishDatabase Instance;

    [SerializeField] private List<FishData> allFish;

    private void Awake()
    {
        Instance = this;
    }

    // get a random fish based on rarity and player luck
    public FishData GetRandomFish(float luck)
    {
        List<FishData> weightedList = new List<FishData>();

        foreach (var fish in allFish)
        {
            int weight = GetRarityWeight(fish.rarity, luck);

            for (int i = 0; i < weight; i++)
            {
                weightedList.Add(fish);
            }
        }

        return weightedList[Random.Range(0, weightedList.Count)];
    }

    // calculate how likely a fish is to appear
    private int GetRarityWeight(FishRarity rarity, float luck)
    {
        int baseWeight = rarity switch
        {
            FishRarity.Common => 50,
            FishRarity.Uncommon => 30,
            FishRarity.Rare => 15,
            FishRarity.Epic => 5,
            _ => 10
        };

        // luck increases chance of better fish
        float modifier = 1f + (luck / 500f);

        return Mathf.Max(1, Mathf.RoundToInt(baseWeight * modifier));
    }

    // roll the weight of a fish based on strength
    public float RollWeight(FishData fish, float strength)
    {
        float t = Random.value;

        // strength pushes weight toward higher values
        t += strength / 100f * 0.25f;
        t = Mathf.Clamp01(t);

        return Mathf.Lerp(fish.minWeight, fish.maxWeight, t);
    }
}