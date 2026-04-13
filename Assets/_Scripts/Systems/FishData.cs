using UnityEngine;

public enum FishRarity
{
    Common,
    Uncommon,
    Rare,
    Epic
}

[CreateAssetMenu(fileName = "Fish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;

    [Header("Weight")]
    public float minWeight;
    public float maxWeight;

    [Header("Visuals")]
    public GameObject fishPrefab;
    public Color fishColor;

    [Header("Economy")]
    public int baseSellPrice;

    [Header("Rarity")]
    public FishRarity rarity;
}