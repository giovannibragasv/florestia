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

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
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

    public void GoToMarket()
    {
        SceneManager.LoadScene("MarketScene");
    }

    SaveData BuildSaveData()
    {
        return new SaveData
        {
            day = CurrentDay,
            balance = Balance,
            inventory = InventorySystem.Instance.GetSaveData(),
            cropSlots = CropSystem.Instance.GetSaveData(),
            dailyBalanceHistory = DailyBalances.ToArray()
        };
    }

    void ApplySave(SaveData data)
    {
        CurrentDay = data.day;
        Balance = data.balance;
        DailyBalances = data.dailyBalanceHistory != null
            ? new List<float>(data.dailyBalanceHistory)
            : new List<float>();
        InventorySystem.Instance?.LoadSaveData(data.inventory);
        CropSystem.Instance?.LoadSaveData(data.cropSlots);
    }
}
