using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [SerializeField] TMP_Text balanceLabel;
    [SerializeField] TMP_Text dayLabel;
    [SerializeField] Slider staminaBar;
    [SerializeField] TMP_Text staminaLabel;
    [SerializeField] TMP_Text timerLabel;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        RefreshAll();
    }

    void Update()
    {
        if (DayNightCycle.Instance == null) return;
        float rem = DayNightCycle.Instance.TimeRemaining;
        int mins = Mathf.FloorToInt(rem / 60f);
        int secs = Mathf.FloorToInt(rem % 60f);
        timerLabel.text = $"{mins:D2}:{secs:D2}";
    }

    public void RefreshBalance(float balance)
    {
        balanceLabel.text = $"R${balance:F2}";
        balanceLabel.color = balance >= 0 ? Color.white : new Color(1f, 0.3f, 0.3f);
    }

    public void RefreshDay(int day)
    {
        dayLabel.text = $"Dia {day}/{GameManager.TotalDays}";
    }

    public void RefreshStamina(int current, int max)
    {
        staminaBar.value = (float)current / max;
        staminaLabel.text = $"{current}/{max}";
    }

    void RefreshAll()
    {
        RefreshBalance(GameManager.Instance.Balance);
        RefreshDay(GameManager.Instance.CurrentDay);
        RefreshStamina(StaminaSystem.Instance.Current, StaminaSystem.MaxStamina);
    }
}
