using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    readonly Dictionary<string, int> _stock = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddCrop(string cropName, int quantity = 1)
    {
        _stock.TryGetValue(cropName, out int current);
        _stock[cropName] = current + quantity;
    }

    public bool TryRemoveCrop(string cropName, int quantity = 1)
    {
        if (!_stock.TryGetValue(cropName, out int current) || current < quantity) return false;
        _stock[cropName] = current - quantity;
        return true;
    }

    public int GetCount(string cropName)
    {
        _stock.TryGetValue(cropName, out int count);
        return count;
    }

    public bool IsEmpty()
    {
        foreach (var v in _stock.Values) if (v > 0) return false;
        return true;
    }

    public IReadOnlyDictionary<string, int> GetAll() => _stock;

    public InventorySaveData GetSaveData() => new InventorySaveData
    {
        mandiocaCount = GetCount("Mandioca"),
        cacauCount = GetCount("Cacau"),
        acaiCount = GetCount("Acai")
    };

    public void LoadSaveData(InventorySaveData data)
    {
        if (data == null) return;
        _stock["Mandioca"] = data.mandiocaCount;
        _stock["Cacau"] = data.cacauCount;
        _stock["Acai"] = data.acaiCount;
    }
}
