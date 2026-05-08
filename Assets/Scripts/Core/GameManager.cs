using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const int TotalDays = 15;
    public const float DayDurationSeconds = 300f; // 5 minutes
    public const float StartingBalance = 50f;
    public const float DailyCost = 2f;

    public int CurrentDay { get; private set; } = 1;
    public float Balance { get; private set; } = StartingBalance;
    public bool IsLoss { get; private set; }
    public List<float> DailyBalances { get; private set; } = new();
    public bool IsGameOver => CurrentDay > TotalDays || IsLoss;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        SaveData loaded = SaveSystem.Load();
        if (IsPlayableSave(loaded))
        {
            ApplySave(loaded);
        }
        else if (loaded != null)
        {
            SaveSystem.Delete();
            ResetRunState();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "FarmScene") return;

        SaveData loaded = SaveSystem.Load();
        if (loaded != null) ApplySave(loaded);
    }

    public void ApplyDailyFixedCost()
    {
        Balance -= DailyCost;
    }

    public void AddRevenue(float amount)
    {
        Balance += amount;
    }

    public void SpendBalance(float amount)
    {
        Balance -= amount;
    }

    public bool CanAfford(float amount) => Balance >= amount;

    public void AdvanceDay()
    {
        CurrentDay++;
        ApplyDailyFixedCost();
        DailyBalances.Add(Balance);

        if (Balance < 0)
        {
            IsLoss = true;
            SceneManager.LoadScene("EndScreen");
            return;
        }

        SaveSystem.Save(BuildSaveData());

        if (IsGameOver)
        {
            SceneManager.LoadScene("EndScreen");
            return;
        }

        SceneManager.LoadScene("FarmScene");
    }

    public void ResetRun()
    {
        SaveSystem.Delete();
        ResetRunState();
    }

    public void GoToMarket()
    {
        if (CropSystem.Instance != null)
            CropSystem.Instance.OnDayEnd();

        if (StaminaSystem.Instance != null)
            StaminaSystem.Instance.ResetForNewDay();

        SaveSystem.Save(BuildSaveData());
        SceneManager.LoadScene("MarketScene");
    }

    SaveData BuildSaveData()
    {
        SaveData existing = SaveSystem.Load();
        InventorySaveData inventory = existing?.inventory;
        CropSlotSaveData[] cropSlots = existing?.cropSlots;

        if (InventorySystem.Instance != null)
            inventory = InventorySystem.Instance.GetSaveData();

        if (CropSystem.Instance != null)
            cropSlots = CropSystem.Instance.GetSaveData();

        return new SaveData
        {
            day = CurrentDay,
            balance = Balance,
            inventory = inventory,
            cropSlots = cropSlots,
            dailyBalanceHistory = DailyBalances.ToArray()
        };
    }

    void ApplySave(SaveData data)
    {
        CurrentDay = data.day;
        Balance = data.balance;
        IsLoss = false;
        DailyBalances = data.dailyBalanceHistory != null
            ? new List<float>(data.dailyBalanceHistory)
            : new List<float>();
        InventorySystem.Instance?.LoadSaveData(data.inventory);
        CropSystem.Instance?.LoadSaveData(data.cropSlots);
    }

    static bool IsPlayableSave(SaveData data)
    {
        return data != null
            && data.day >= 1
            && data.day <= TotalDays
            && data.balance >= 0f;
    }

    void ResetRunState()
    {
        CurrentDay = 1;
        Balance = StartingBalance;
        IsLoss = false;
        DailyBalances = new List<float>();
    }
}
