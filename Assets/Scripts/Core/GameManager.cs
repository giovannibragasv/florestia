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
    public List<PlantingRecord> Plantings { get; private set; } = new();
    public List<DailySaleRecord> Sales { get; private set; } = new();
    public List<QuestionAnswerRecord> Questions { get; private set; } = new();
    public bool TutorialCompleted { get; private set; }

    public bool IsGameOver => CurrentDay > TotalDays || IsLoss;
    public StudentProfile ActiveProfile => SaveSystem.ActiveProfile;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }
    void OnDestroy() { if (Instance == this) Instance = null; }

    void Start()
    {
        EnsureProfilePickerExists();
        EnsurePauseMenuExists();
        EnsureActiveProfile();
        EnsureTutorialControllerExists();
        EnsureIntermediadoraDashboardExists();
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

    static void EnsureProfilePickerExists()
    {
        if (StudentProfilePicker.Instance != null) return;
        var go = new GameObject("_StudentProfilePicker");
        go.AddComponent<StudentProfilePicker>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureIntermediadoraDashboardExists();
        EnsurePauseMenuExists();

        if (scene.name == "FarmScene" || scene.name == "MarketScene")
        {
            EnsureTutorialControllerExists();
        }

        if (scene.name != "FarmScene") return;
        SaveData loaded = SaveSystem.Load();
        if (loaded != null) ApplySave(loaded);
    }

    static void EnsureTutorialControllerExists()
    {
        if (TutorialController.Instance != null) return;
        var go = new GameObject("_Tutorial");
        go.AddComponent<TutorialController>();
    }

    static void EnsureIntermediadoraDashboardExists()
    {
        if (IntermediadoraDashboardController.Instance != null) return;
        var go = new GameObject("_IntermediadoraDashboard");
        go.AddComponent<IntermediadoraDashboardController>();
    }

    static void EnsurePauseMenuExists()
    {
        if (PauseMenuController.Instance != null) return;
        var go = new GameObject("_PauseMenu");
        go.AddComponent<PauseMenuController>();
    }

    public void ApplyDailyFixedCost() { Balance -= DailyCost; }
    public void AddRevenue(float amount) { Balance += amount; }
    public void SpendBalance(float amount) { Balance -= amount; }
    public bool CanAfford(float amount) => Balance >= amount;

    public void RecordPlanting(int slotIndex, string cropName)
    {
        Plantings.Add(new PlantingRecord
        {
            day = CurrentDay,
            cropName = cropName,
            slotIndex = slotIndex
        });
    }

    public void RecordSale(string cropName, int quantity, float pricePerUnit, string buyerName)
    {
        Sales.Add(new DailySaleRecord
        {
            day = CurrentDay,
            cropName = cropName,
            quantity = quantity,
            pricePerUnit = pricePerUnit,
            total = quantity * pricePerUnit,
            buyerName = buyerName
        });
    }

    public void RecordQuestionAnswer(string cropName, string questionId, bool correct)
    {
        Questions.Add(new QuestionAnswerRecord
        {
            day = CurrentDay,
            cropName = cropName,
            questionId = questionId,
            correct = correct
        });
    }

    public void CompleteTutorial()
    {
        if (TutorialCompleted) return;
        TutorialCompleted = true;
        SaveSystem.Save(BuildSaveData());
    }

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

    public void SaveNow()
    {
        SaveSystem.Save(BuildSaveData());
    }

    public void GoToMarket()
    {
        if (CropSystem.Instance != null) CropSystem.Instance.OnDayEnd();
        if (StaminaSystem.Instance != null) StaminaSystem.Instance.ResetForNewDay();
        TutorialController.Instance?.NotifyMarketEntered();

        SaveSystem.Save(BuildSaveData());
        SceneManager.LoadScene("MarketScene");
    }

    void EnsureActiveProfile()
    {
        if (SaveSystem.ActiveProfile != null) return;

        var profiles = SaveSystem.ListProfiles();
        if (profiles.Count > 0)
            SaveSystem.SetActiveProfile(profiles[0]);
        // If no profiles exist, deixar null e o picker aparece automaticamente.
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
            tutorialCompleted = TutorialCompleted,
            inventory = inventory,
            cropSlots = cropSlots,
            dailyBalanceHistory = DailyBalances.ToArray(),
            plantings = Plantings.ToArray(),
            sales = Sales.ToArray(),
            questions = Questions.ToArray()
        };
    }

    void ApplySave(SaveData data)
    {
        CurrentDay = data.day;
        Balance = data.balance;
        IsLoss = false;
        TutorialCompleted = data.tutorialCompleted;
        DailyBalances = data.dailyBalanceHistory != null
            ? new List<float>(data.dailyBalanceHistory)
            : new List<float>();
        Plantings = data.plantings != null
            ? new List<PlantingRecord>(data.plantings)
            : new List<PlantingRecord>();
        Sales = data.sales != null
            ? new List<DailySaleRecord>(data.sales)
            : new List<DailySaleRecord>();
        Questions = data.questions != null
            ? new List<QuestionAnswerRecord>(data.questions)
            : new List<QuestionAnswerRecord>();
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
        TutorialCompleted = false;
        DailyBalances = new List<float>();
        Plantings = new List<PlantingRecord>();
        Sales = new List<DailySaleRecord>();
        Questions = new List<QuestionAnswerRecord>();
    }
}
