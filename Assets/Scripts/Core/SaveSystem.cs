using System.IO;
using UnityEngine;

[System.Serializable]
public class CropSlotSaveData
{
    public int slotIndex;
    public string cropType;   // "Mandioca" | "Cacau" | "Acai" | ""
    public int daysPlanted;
    public bool isWatered;
}

[System.Serializable]
public class InventorySaveData
{
    public int mandiocaCount;
    public int cacauCount;
    public int acaiCount;
}

[System.Serializable]
public class SaveData
{
    public int day;
    public float balance;
    public InventorySaveData inventory;
    public CropSlotSaveData[] cropSlots;
    public float[] dailyBalanceHistory;
}

public static class SaveSystem
{
    static string SavePath => Path.Combine(Application.persistentDataPath, "florestia_save.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath)) return null;
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Delete()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }
}
