using UnityEngine;

public class BuyerSystem : MonoBehaviour
{
    public static BuyerSystem Instance { get; private set; }

    [SerializeField] BuyerData[] buyers; // assign Atravessador, Feirante, Comprador in Inspector

    [Header("Night Market Mood")]
    [SerializeField] int moodSeedOffset = 7301;
    [SerializeField] NightMarketMood[] nightlyMoods =
    {
        new NightMarketMood
        {
            label = "Calma",
            description = "Pouca gente comprando hoje.",
            acceptanceBonus = -0.08f,
            weight = 1f
        },
        new NightMarketMood
        {
            label = "Normal",
            description = "Movimento comum na feira.",
            acceptanceBonus = 0f,
            weight = 2f
        },
        new NightMarketMood
        {
            label = "Animada",
            description = "Mais compradores procurando comida.",
            acceptanceBonus = 0.10f,
            weight = 1.35f
        },
        new NightMarketMood
        {
            label = "Cheia",
            description = "Noite boa para negociar.",
            acceptanceBonus = 0.18f,
            weight = 0.65f
        }
    };

    NightMarketMood _tonightMood;
    int _tonightMoodDay = -1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        RollMoodForCurrentDay();
    }

    public BuyerData[] GetAllBuyers() => buyers;

    public string TonightMoodLabel
    {
        get { EnsureMoodForCurrentDay(); return _tonightMood != null ? _tonightMood.label : "Normal"; }
    }

    public string TonightMoodDescription
    {
        get { EnsureMoodForCurrentDay(); return _tonightMood != null ? _tonightMood.description : ""; }
    }

    public float TonightAcceptanceBonus
    {
        get { EnsureMoodForCurrentDay(); return _tonightMood != null ? _tonightMood.acceptanceBonus : 0f; }
    }

    public float GetAcceptanceChance(BuyerData buyer, string cropName, float askingPricePerUnit)
    {
        if (buyer == null || string.IsNullOrEmpty(cropName) || askingPricePerUnit <= 0f)
            return 0f;

        float maxPrice = buyer.GetMaxPrice(cropName);
        if (maxPrice <= 0f) return 0f;

        float ratio = askingPricePerUnit / maxPrice;
        float baseChance;
        if (ratio <= 0.75f)
        {
            baseChance = 0.97f;
        }
        else if (ratio <= 1f)
        {
            baseChance = Mathf.Lerp(0.92f, 0.68f, Mathf.InverseLerp(0.75f, 1f, ratio));
        }
        else if (ratio <= 1.2f)
        {
            baseChance = Mathf.Lerp(0.34f, 0.06f, Mathf.InverseLerp(1f, 1.2f, ratio));
        }
        else
        {
            baseChance = 0f;
        }

        return Mathf.Clamp01(baseChance + TonightAcceptanceBonus);
    }

    // Returns true and processes sale if buyer accepts the asking price.
    public bool TrySell(BuyerData buyer, string cropName, int quantity, float askingPricePerUnit)
    {
        float chance = GetAcceptanceChance(buyer, cropName, askingPricePerUnit);
        if (Random.value > chance) return false;
        if (!InventorySystem.Instance.TryRemoveCrop(cropName, quantity)) return false;

        float revenue = askingPricePerUnit * quantity;
        GameManager.Instance.AddRevenue(revenue);
        GameManager.Instance.RecordSale(cropName, quantity, askingPricePerUnit, buyer.buyerName);
        EndScreenController.Instance?.RecordSale(cropName, revenue, quantity, askingPricePerUnit);
        return true;
    }

    void EnsureMoodForCurrentDay()
    {
        int day = GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1;
        if (_tonightMoodDay != day || _tonightMood == null)
            RollMoodForCurrentDay();
    }

    void RollMoodForCurrentDay()
    {
        int day = GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1;
        _tonightMoodDay = day;

        NightMarketMood[] moods = nightlyMoods;
        if (moods == null || moods.Length == 0)
        {
            _tonightMood = new NightMarketMood
            {
                label = "Normal",
                description = "Movimento comum na feira.",
                acceptanceBonus = 0f,
                weight = 1f
            };
            return;
        }

        float totalWeight = 0f;
        foreach (var mood in moods)
            if (mood != null)
                totalWeight += Mathf.Max(0f, mood.weight);

        if (totalWeight <= 0f)
        {
            _tonightMood = moods[0];
            return;
        }

        var rng = new System.Random(day * 1009 + moodSeedOffset);
        float pick = (float)rng.NextDouble() * totalWeight;
        foreach (var mood in moods)
        {
            if (mood == null) continue;
            pick -= Mathf.Max(0f, mood.weight);
            if (pick <= 0f)
            {
                _tonightMood = mood;
                return;
            }
        }

        _tonightMood = moods[moods.Length - 1];
    }

    [System.Serializable]
    public class NightMarketMood
    {
        public string label;
        [TextArea] public string description;
        [Range(-0.25f, 0.35f)] public float acceptanceBonus;
        [Min(0f)] public float weight = 1f;
    }
}
