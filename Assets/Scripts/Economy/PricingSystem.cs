using System.Collections.Generic;
using UnityEngine;

public class PricingSystem : MonoBehaviour
{
    public static PricingSystem Instance { get; private set; }

    // Default asking prices (player adjusts via slider)
    readonly Dictionary<string, float> _askingPrices = new()
    {
        { "Mandioca", 7f },
        { "Cacau",    16f },
        { "Acai",     28f }
    };

    // Seed costs — mirrors CropData but available without a CropData reference
    static readonly Dictionary<string, float> SeedCosts = new()
    {
        { "Mandioca", 3f },
        { "Cacau",    6f },
        { "Acai",     10f }
    };

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public float GetAskingPrice(string cropName)
    {
        _askingPrices.TryGetValue(cropName, out float price);
        return price;
    }

    public void SetAskingPrice(string cropName, float price)
    {
        _askingPrices[cropName] = Mathf.Max(0f, price);
    }

    public float GetSeedCost(string cropName)
    {
        SeedCosts.TryGetValue(cropName, out float cost);
        return cost;
    }

    public float GetMarginValue(string cropName)
    {
        return GetAskingPrice(cropName) - GetSeedCost(cropName);
    }

    public float GetMarginPercent(string cropName)
    {
        float cost = GetSeedCost(cropName);
        if (cost == 0f) return 0f;
        return (GetMarginValue(cropName) / cost) * 100f;
    }
}
