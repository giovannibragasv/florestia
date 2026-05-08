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

    [Header("Math Coaching")]
    [SerializeField] TMP_Text cropPreviewLabel;
    [SerializeField] TMP_Text proportionalityLabel;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        EnsureCoachingLabels();
        RefreshAll();
    }

    void Update()
    {
        if (DayNightCycle.Instance != null && timerLabel != null)
        {
            float rem = DayNightCycle.Instance.TimeRemaining;
            int mins = Mathf.FloorToInt(rem / 60f);
            int secs = Mathf.FloorToInt(rem % 60f);
            timerLabel.text = $"{mins:D2}:{secs:D2}";
            timerLabel.color = DayNightCycle.Instance.IsNight ? Color.red : Color.white;
        }

        if (GameManager.Instance != null && balanceLabel != null)
            balanceLabel.color = GameManager.Instance.Balance < 20f ? Color.red : Color.white;
    }

    public void RefreshBalance(float balance)
    {
        balanceLabel.text = $"R${balance:F2}";
        balanceLabel.color = balance < 20f ? Color.red : Color.white;
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

    public void RefreshCropPreview(ToolType tool)
    {
        if (cropPreviewLabel == null) return;

        switch (tool)
        {
            case ToolType.Mandioca:
            case ToolType.Cacau:
            case ToolType.Acai:
                CropData crop = CropSystem.Instance != null
                    ? CropSystem.Instance.GetCropData(tool)
                    : null;
                if (crop != null && crop.growthDays > 0)
                {
                    float profitPerDay = (crop.baseMarketValue - crop.seedCost) / crop.growthDays;
                    cropPreviewLabel.text =
                        $"Próxima: {crop.cropName} · −R${crop.seedCost:F0} · {crop.growthDays}d · ~R${profitPerDay:F2}/dia";
                }
                else
                {
                    cropPreviewLabel.text = "";
                }
                break;
            case ToolType.Water:
                cropPreviewLabel.text = "Regar tile";
                break;
            case ToolType.Harvest:
                cropPreviewLabel.text = "Colher tile";
                break;
        }

        RefreshProportionalityCue();
    }

    public void RefreshProportionalityCue()
    {
        if (proportionalityLabel == null) return;
        if (ToolbarController.Instance == null || CropSystem.Instance == null)
        {
            proportionalityLabel.text = "";
            return;
        }

        ToolType tool = ToolbarController.Instance.Selected;
        if (tool != ToolType.Mandioca && tool != ToolType.Cacau && tool != ToolType.Acai)
        {
            proportionalityLabel.text = "";
            return;
        }

        CropData crop = CropSystem.Instance.GetCropData(tool);
        if (crop == null) { proportionalityLabel.text = ""; return; }

        int count = CropSystem.Instance.CountPlantedByCrop(crop.cropName);
        if (count <= 0) { proportionalityLabel.text = ""; return; }

        float invest = count * crop.seedCost;
        float expected = count * crop.baseMarketValue;
        proportionalityLabel.text =
            $"Plantando: {count}× {crop.cropName} · Investimento R${invest:F0} · Receita ~R${expected:F0}";
    }

    void RefreshAll()
    {
        RefreshBalance(GameManager.Instance.Balance);
        RefreshDay(GameManager.Instance.CurrentDay);
        RefreshStamina(StaminaSystem.Instance.Current, StaminaSystem.MaxStamina);
        if (ToolbarController.Instance != null)
            RefreshCropPreview(ToolbarController.Instance.Selected);
    }

    void EnsureCoachingLabels()
    {
        if (cropPreviewLabel != null && proportionalityLabel != null) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        if (cropPreviewLabel == null)
            cropPreviewLabel = MakeCoachingLabel(canvas.transform, "CropPreviewLabel",
                new Vector2(0f, -34f), new Color(0.98f, 0.84f, 0.45f, 1f));

        if (proportionalityLabel == null)
            proportionalityLabel = MakeCoachingLabel(canvas.transform, "ProportionalityLabel",
                new Vector2(0f, -62f), new Color(0.78f, 0.92f, 0.78f, 1f));
    }

    static TMP_Text MakeCoachingLabel(Transform parent, string name, Vector2 anchoredPos, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(720f, 26f);

        var label = go.AddComponent<TextMeshProUGUI>();
        label.text = "";
        label.fontSize = 16;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.raycastTarget = false;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) label.font = font;
        return label;
    }
}
