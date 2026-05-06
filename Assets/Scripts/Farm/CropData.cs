using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Florestia/Crop Data")]
public class CropData : ScriptableObject
{
    public string cropName;
    public int growthDays;
    public float seedCost;
    public float baseMarketValue;
    public Sprite[] growthStageSprites; // index = days planted (0 = seed, last = ready)
    public int staminaCostToPlant = 3;
    public int staminaCostToWater = 2;
    public int staminaCostToHarvest = 3;
}
