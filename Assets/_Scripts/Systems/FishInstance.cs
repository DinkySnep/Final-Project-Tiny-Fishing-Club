[System.Serializable]
public class FishInstance
{
    public FishData data;
    public float weight;

    public FishInstance(FishData data, float weight)
    {
        this.data = data;
        this.weight = weight;
    }
}