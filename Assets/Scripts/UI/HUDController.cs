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
    [SerializeField] TMP_Text phaseLabel;
    [SerializeField] Image phaseProgressFill;

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
        EnsureDayPhaseWidgets();
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
            RefreshDayPhase(DayNightCycle.Instance.NormalizedTime);
        }

        if (GameManager.Instance != null && balanceLabel != null)
            balanceLabel.color = GameManager.Instance.Balance < 20f ? Color.red : Color.white;
    }

    public void RefreshBalance(float balance)
    {
        // Vocabulário 8-11 (Modelo C §3.4): "dinheiro" no lugar de "saldo".
        balanceLabel.text = $"Dinheiro: R${balance:F2}";
        balanceLabel.color = balance < 20f ? Color.red : Color.white;
    }

    public void RefreshDay(int day)
    {
        dayLabel.text = $"Dia {day} de {GameManager.TotalDays}";
    }

    public void RefreshStamina(int current, int max)
    {
        staminaBar.value = (float)current / max;
        staminaLabel.text = $"Energia: {current}/{max}";
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
                    // Antes: "Próxima: Cacau · −R$6 · 4d · ~R$2.50/dia" (jargão financeiro).
                    // Agora: "Vou plantar Cacau. Custa R$6. Demora 4 dias pra colher."
                    cropPreviewLabel.text =
                        $"Vou plantar {crop.cropName}. Custa R${crop.seedCost:F0}. Demora {crop.growthDays} dias pra colher.";
                }
                else
                {
                    cropPreviewLabel.text = "";
                }
                break;
            case ToolType.Water:
                cropPreviewLabel.text = "Vou regar a plantação";
                break;
            case ToolType.Harvest:
                cropPreviewLabel.text = "Vou colher o que tá pronto";
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
        // Antes: "Plantando: 4× Cacau · Investimento R$24 · Receita ~R$64"
        // Agora: "Você tem 4 pés de Cacau · gastou R$24 · pode render até R$64"
        proportionalityLabel.text =
            $"Você tem {count} {PluralCropName(crop.cropName, count)} · gastou R${invest:F0} · pode render até R${expected:F0}";
    }

    static string PluralCropName(string crop, int qty)
    {
        if (qty == 1) return crop switch
        {
            "Mandioca" => "pé de mandioca",
            "Cacau" => "pé de cacau",
            "Acai" => "açaizeiro",
            _ => crop.ToLower()
        };
        return crop switch
        {
            "Mandioca" => "pés de mandioca",
            "Cacau" => "pés de cacau",
            "Acai" => "açaizeiros",
            _ => crop.ToLower() + "s"
        };
    }

    void RefreshAll()
    {
        RefreshBalance(GameManager.Instance.Balance);
        RefreshDay(GameManager.Instance.CurrentDay);
        RefreshStamina(StaminaSystem.Instance.Current, StaminaSystem.MaxStamina);
        if (ToolbarController.Instance != null)
            RefreshCropPreview(ToolbarController.Instance.Selected);
    }

    void RefreshDayPhase(float normalizedTime)
    {
        float t = Mathf.Clamp01(normalizedTime);
        string phase;
        Color color;

        if (t < 0.42f)
        {
            phase = "Manhã";
            color = new Color(0.98f, 0.84f, 0.45f, 1f);
        }
        else if (t < 0.78f)
        {
            phase = "Tarde";
            color = new Color(0.98f, 0.58f, 0.28f, 1f);
        }
        else
        {
            phase = "Anoitecer";
            color = new Color(0.72f, 0.60f, 0.98f, 1f);
        }

        if (phaseLabel != null)
        {
            phaseLabel.text = phase;
            phaseLabel.color = color;
        }

        if (phaseProgressFill != null)
        {
            phaseProgressFill.color = color;
            phaseProgressFill.rectTransform.localScale = new Vector3(t, 1f, 1f);
        }
    }

    void EnsureDayPhaseWidgets()
    {
        if (phaseLabel != null && phaseProgressFill != null) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        var panel = new GameObject("DayPhasePanel");
        panel.transform.SetParent(canvas.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-16f, -70f);
        rt.sizeDelta = new Vector2(180f, 46f);
        panel.AddComponent<Image>().color = new Color(0.08f, 0.055f, 0.035f, 0.78f);

        if (phaseLabel == null)
            phaseLabel = MakePhaseLabel(panel.transform, "PhaseLabel",
                new Vector2(0f, -6f), new Vector2(164f, 22f), 16);

        if (phaseProgressFill == null)
            phaseProgressFill = MakePhaseProgress(panel.transform);
    }

    static TMP_Text MakePhaseLabel(Transform parent, string name, Vector2 anchoredPos, Vector2 size, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var label = go.AddComponent<TextMeshProUGUI>();
        label.text = "Manhã";
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.98f, 0.84f, 0.45f, 1f);
        label.raycastTarget = false;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) label.font = font;
        return label;
    }

    static Image MakePhaseProgress(Transform parent)
    {
        var bgGO = new GameObject("PhaseProgressBg");
        bgGO.transform.SetParent(parent, false);
        var bgRT = bgGO.AddComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0.5f, 1f);
        bgRT.anchorMax = new Vector2(0.5f, 1f);
        bgRT.pivot = new Vector2(0.5f, 1f);
        bgRT.anchoredPosition = new Vector2(0f, -32f);
        bgRT.sizeDelta = new Vector2(146f, 8f);
        bgGO.AddComponent<Image>().color = new Color(0.28f, 0.20f, 0.12f, 1f);

        var fillGO = new GameObject("PhaseProgressFill");
        fillGO.transform.SetParent(bgGO.transform, false);
        var fillRT = fillGO.AddComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0f, 0.5f);
        fillRT.anchorMax = new Vector2(0f, 0.5f);
        fillRT.pivot = new Vector2(0f, 0.5f);
        fillRT.anchoredPosition = Vector2.zero;
        fillRT.sizeDelta = new Vector2(146f, 8f);
        fillRT.localScale = new Vector3(0f, 1f, 1f);
        return fillGO.AddComponent<Image>();
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
