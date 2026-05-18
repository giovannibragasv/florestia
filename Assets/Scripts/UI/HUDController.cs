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

    [Header("Day Tip")]
    [SerializeField] GameObject dayTipPanel;
    [SerializeField] TMP_Text dayTipLabel;
    [SerializeField] Button dayTipCloseButton;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        EnsureDayPhaseWidgets();
        EnsureCoachingLabels();
        EnsureDayTipPanel();
        ApplyStardewStyleLayout();
        RefreshAll();
        ShowDayStartTipIfNeeded();
    }

    void Update()
    {
        if (DayNightCycle.Instance != null && timerLabel != null)
        {
            float rem = DayNightCycle.Instance.TimeRemaining;
            int mins = Mathf.FloorToInt(rem / 60f);
            int secs = Mathf.FloorToInt(rem % 60f);
            timerLabel.text = $"{mins:D2}:{secs:D2}";
            timerLabel.color = DayNightCycle.Instance.IsNight
                ? new Color(0.75f, 0.10f, 0.08f, 1f)
                : new Color(0.20f, 0.09f, 0.03f, 1f);
            RefreshDayPhase(DayNightCycle.Instance.NormalizedTime);
        }

        if (GameManager.Instance != null && balanceLabel != null)
            balanceLabel.color = GameManager.Instance.Balance < 20f
                ? new Color(0.75f, 0.10f, 0.08f, 1f)
                : new Color(0.20f, 0.09f, 0.03f, 1f);
    }

    public void RefreshBalance(float balance)
    {
        // Vocabulário 8-11 (Modelo C §3.4): "dinheiro" no lugar de "saldo".
        balanceLabel.text = $"Dinheiro: R${balance:F2}";
        balanceLabel.color = balance < 20f
            ? new Color(0.75f, 0.10f, 0.08f, 1f)
            : new Color(0.20f, 0.09f, 0.03f, 1f);
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
                    cropPreviewLabel.text =
                        $"{LabelForCrop(crop.cropName)} · R${crop.seedCost:F0} · {crop.growthDays} dias";
                }
                else
                {
                    cropPreviewLabel.text = "";
                }
                break;
            case ToolType.Water:
                cropPreviewLabel.text = "Regar";
                break;
            case ToolType.Harvest:
                cropPreviewLabel.text = "Colher";
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
            $"{count} {PluralCropName(crop.cropName, count)} · gastou R${invest:F0} · rende até R${expected:F0}";
    }

    static string LabelForCrop(string cropName) => cropName == "Acai" ? "Açaí" : cropName;

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
                new Vector2(0f, 108f), new Color(0.98f, 0.84f, 0.45f, 1f));

        if (proportionalityLabel == null)
            proportionalityLabel = MakeCoachingLabel(canvas.transform, "ProportionalityLabel",
                new Vector2(0f, 84f), new Color(0.78f, 0.92f, 0.78f, 1f));
    }

    static TMP_Text MakeCoachingLabel(Transform parent, string name, Vector2 anchoredPos, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(620f, 24f);

        var label = go.AddComponent<TextMeshProUGUI>();
        label.text = "";
        label.fontSize = 14;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.raycastTarget = false;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) label.font = font;
        return label;
    }

    void ApplyStardewStyleLayout()
    {
        CompactHudPanel(balanceLabel, new Vector2(16f, -16f), new Vector2(208f, 86f));
        CompactHudPanel(timerLabel, new Vector2(-16f, -16f), new Vector2(172f, 86f));
        StyleHudPanel(balanceLabel);
        StyleHudPanel(timerLabel);
        LayoutTopRightPanel(timerLabel);

        if (balanceLabel != null) balanceLabel.fontSize = 18;
        if (dayLabel != null) dayLabel.fontSize = 15;
        if (staminaLabel != null) staminaLabel.fontSize = 14;
        if (timerLabel != null) timerLabel.fontSize = 19;
        if (phaseLabel != null) phaseLabel.fontSize = 14;

        MoveCoachingLabel(cropPreviewLabel, new Vector2(0f, 104f), 13,
            new Color(0.95f, 0.76f, 0.30f, 1f));
        MoveCoachingLabel(proportionalityLabel, new Vector2(0f, 82f), 12,
            new Color(0.78f, 0.92f, 0.78f, 1f));
    }

    static void CompactHudPanel(TMP_Text childLabel, Vector2 anchoredPos, Vector2 size)
    {
        if (childLabel == null) return;
        var rt = childLabel.transform.parent as RectTransform;
        if (rt == null) return;

        bool right = anchoredPos.x < 0f;
        rt.anchorMin = right ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
        rt.anchorMax = rt.anchorMin;
        rt.pivot = right ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

    static void StyleHudPanel(TMP_Text childLabel)
    {
        if (childLabel == null) return;
        if (childLabel.transform.parent.TryGetComponent(out Image panelImage))
            panelImage.color = new Color(0.95f, 0.60f, 0.27f, 0.90f);

        foreach (var label in childLabel.transform.parent.GetComponentsInChildren<TMP_Text>(true))
            label.color = new Color(0.20f, 0.09f, 0.03f, 1f);
    }

    static void LayoutTopRightPanel(TMP_Text timer)
    {
        if (timer == null) return;
        Transform panel = timer.transform.parent;
        PlaceChild(panel.Find("SunIcon") as RectTransform, new Vector2(14f, -10f), new Vector2(26f, 26f));
        PlaceChild(timer.rectTransform, new Vector2(48f, -8f), new Vector2(96f, 26f));
        timer.alignment = TextAlignmentOptions.Left;

        TMP_Text phase = panel.Find("PhaseLabel")?.GetComponent<TMP_Text>();
        if (phase != null)
        {
            PlaceChild(phase.rectTransform, new Vector2(14f, -36f), new Vector2(130f, 22f));
            phase.alignment = TextAlignmentOptions.Left;
        }

        RectTransform bg = panel.Find("PhaseProgressBg") as RectTransform;
        PlaceChild(bg, new Vector2(14f, -64f), new Vector2(130f, 8f));
        if (bg != null && bg.GetComponent<Image>() != null)
            bg.GetComponent<Image>().color = new Color(0.42f, 0.22f, 0.08f, 1f);
        if (bg != null)
        {
            var fill = bg.Find("PhaseProgressFill") as RectTransform;
            if (fill != null) fill.sizeDelta = new Vector2(130f, 8f);
        }
    }

    static void PlaceChild(RectTransform rt, Vector2 anchoredPos, Vector2 size)
    {
        if (rt == null) return;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

    static void MoveCoachingLabel(TMP_Text label, Vector2 anchoredPos, int fontSize, Color color)
    {
        if (label == null) return;
        var rt = label.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(620f, 24f);
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.textWrappingMode = TextWrappingModes.NoWrap;
    }

    void ShowDayStartTipIfNeeded()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentDay <= 1) return;
        EnsureDayTipPanel();
        if (dayTipPanel == null || dayTipLabel == null) return;

        dayTipLabel.text = BuildTipForPreviousDay(GameManager.Instance);
        dayTipPanel.SetActive(true);
    }

    static string BuildTipForPreviousDay(GameManager gm)
    {
        int previousDay = gm.CurrentDay - 1;
        bool hadSale = false;
        bool soldBelowCost = false;
        int atravessadorSales = 0;
        int salesCount = 0;

        foreach (var sale in gm.Sales)
        {
            if (sale.day != previousDay) continue;
            hadSale = true;
            salesCount++;
            if (sale.buyerName != null && sale.buyerName.Contains("travessador"))
                atravessadorSales++;

            float cost = PricingSystem.Instance != null
                ? PricingSystem.Instance.GetSeedCost(sale.cropName)
                : 0f;
            if (cost > 0f && sale.pricePerUnit < cost)
                soldBelowCost = true;
        }

        if (!hadSale)
            return "Dica do dia: tente colher e vender algo hoje. Dinheiro parado na sacola não ajuda no sustento.";
        if (soldBelowCost)
            return "Dica do dia: antes de vender, compare o preço com o que você pagou na semente.";
        if (salesCount > 0 && atravessadorSales * 2 >= salesCount)
            return "Dica do dia: o Atravessador compra rápido, mas outros compradores podem pagar melhor.";

        int plantedKinds = CountPlantingKinds(gm, previousDay);
        if (plantedKinds == 1)
            return "Dica do dia: tente misturar culturas. Cada planta demora e rende de um jeito.";

        return "Dica do dia: continue comparando custo, dinheiro recebido e sobra antes de decidir o preço.";
    }

    static int CountPlantingKinds(GameManager gm, int day)
    {
        bool mandioca = false, cacau = false, acai = false;
        foreach (var planting in gm.Plantings)
        {
            if (planting.day != day) continue;
            if (planting.cropName == "Mandioca") mandioca = true;
            else if (planting.cropName == "Cacau") cacau = true;
            else if (planting.cropName == "Acai") acai = true;
        }
        int count = 0;
        if (mandioca) count++;
        if (cacau) count++;
        if (acai) count++;
        return count;
    }

    void EnsureDayTipPanel()
    {
        if (dayTipPanel != null && dayTipLabel != null && dayTipCloseButton != null) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        var panel = new GameObject("DayTipPanel");
        panel.transform.SetParent(canvas.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -100f);
        rt.sizeDelta = new Vector2(720f, 86f);
        panel.AddComponent<Image>().color = new Color(0.12f, 0.08f, 0.05f, 0.94f);

        dayTipLabel = MakePanelLabel(panel.transform, "TipLabel",
            new Vector2(-58f, 0f), new Vector2(560f, 64f), 18,
            new Color(0.96f, 0.88f, 0.68f, 1f));
        dayTipLabel.alignment = TextAlignmentOptions.Center;
        dayTipLabel.textWrappingMode = TextWrappingModes.Normal;

        dayTipCloseButton = MakePanelButton(panel.transform, "CloseButton",
            new Vector2(292f, 0f), new Vector2(92f, 46f), "OK");
        dayTipCloseButton.onClick.AddListener(() => panel.SetActive(false));

        panel.SetActive(false);
        dayTipPanel = panel;
    }

    static TMP_Text MakePanelLabel(Transform parent, string name, Vector2 anchoredPos,
        Vector2 size, int fontSize, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        var label = go.AddComponent<TextMeshProUGUI>();
        label.text = "";
        label.fontSize = fontSize;
        label.color = color;
        label.raycastTarget = false;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) label.font = font;
        return label;
    }

    static Button MakePanelButton(Transform parent, string name, Vector2 anchoredPos, Vector2 size, string text)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        var image = go.AddComponent<Image>();
        image.color = new Color(0.88f, 0.62f, 0.22f, 1f);
        var button = go.AddComponent<Button>();
        button.targetGraphic = image;

        var label = MakePanelLabel(go.transform, "Label", Vector2.zero, size, 18,
            new Color(0.10f, 0.07f, 0.04f, 1f));
        label.text = text;
        label.alignment = TextAlignmentOptions.Center;
        label.fontStyle = FontStyles.Bold;
        return button;
    }
}
