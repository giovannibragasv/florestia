using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketUIController : MonoBehaviour
{
    [Header("Crop Selection")]
    [SerializeField] Button[] cropButtons;
    [SerializeField] TMP_Text stockLabel;

    static readonly string[] CropNames = { "Mandioca", "Cacau", "Acai" };
    static readonly Color CropButtonNormal = new Color(0.25f, 0.20f, 0.16f, 1f);
    static readonly Color CropButtonSelected = new Color(0.92f, 0.67f, 0.28f, 1f);
    int _selectedCropIndex;

    [Header("Pricing")]
    [SerializeField] Slider priceSlider;
    [SerializeField] TMP_Text costLabel;
    [SerializeField] TMP_Text priceLabel;
    [SerializeField] TMP_Text marginLabel;

    [Header("Quantity")]
    [SerializeField] Slider quantitySlider;
    [SerializeField] TMP_Text quantityLabel;
    [SerializeField] TMP_Text totalLabel;

    [Header("Buyer")]
    [SerializeField] BuyerSelector buyerSelector;
    [SerializeField] TMP_Text buyerDialogueLine;
    [SerializeField] Image buyerPortrait;

    [Header("Action")]
    [SerializeField] Button sellButton;
    [SerializeField] Button endDayButton;

    [Header("Daily Summary Modal")]
    [SerializeField] GameObject dailySummaryPanel;
    [SerializeField] TMP_Text summaryTitleLabel;
    [SerializeField] TMP_Text summaryTableLabel;
    [SerializeField] TMP_Text summaryRevenueLabel;
    [SerializeField] TMP_Text summaryFixedCostLabel;
    [SerializeField] TMP_Text summaryBalanceLabel;
    [SerializeField] Button summaryContinueButton;

    string _selectedCrop;
    BuyerData _selectedBuyer;

    void Start()
    {
        SceneCameraUtility.EnsureUICamera();
        SceneCameraUtility.EnsureEventSystem();

        DestroyLegacyCropDropdown();
        EnsureCropButtons();
        EnsureQuantitySlider();
        EnsureDailySummaryPanel();

        if (priceSlider == null || sellButton == null || endDayButton == null) return;

        WireCropButtons();
        priceSlider.onValueChanged.AddListener(_ => RefreshPriceDisplay());
        if (quantitySlider != null)
            quantitySlider.onValueChanged.AddListener(_ => RefreshQuantityDisplay());
        sellButton.onClick.AddListener(OnSellClicked);
        endDayButton.onClick.AddListener(OnEndDayClicked);
        if (summaryContinueButton != null)
            summaryContinueButton.onClick.AddListener(OnSummaryContinue);

        SelectCropByIndex(0);
    }

    void WireCropButtons()
    {
        if (cropButtons == null) return;
        for (int i = 0; i < cropButtons.Length && i < CropNames.Length; i++)
        {
            int idx = i;
            if (cropButtons[i] == null) continue;
            cropButtons[i].onClick.RemoveAllListeners();
            cropButtons[i].onClick.AddListener(() => SelectCropByIndex(idx));
        }
    }

    public void SelectCropByIndex(int index)
    {
        if (index < 0 || index >= CropNames.Length) return;
        _selectedCropIndex = index;
        _selectedCrop = CropNames[index];
        RefreshCropButtonHighlight();

        if (PricingSystem.Instance != null)
            priceSlider.value = PricingSystem.Instance.GetAskingPrice(_selectedCrop);
        RefreshStockLabel();
        RefreshQuantityRange();
        RefreshPriceDisplay();
    }

    void RefreshCropButtonHighlight()
    {
        if (cropButtons == null) return;
        for (int i = 0; i < cropButtons.Length; i++)
        {
            if (cropButtons[i] == null) continue;
            if (cropButtons[i].targetGraphic is Image img)
                img.color = i == _selectedCropIndex ? CropButtonSelected : CropButtonNormal;
        }
    }

    void RefreshStockLabel()
    {
        int qty = InventorySystem.Instance.GetCount(_selectedCrop);
        stockLabel.text = $"Estoque: {qty}";
    }

    void RefreshQuantityRange()
    {
        if (quantitySlider == null) return;
        int stock = InventorySystem.Instance.GetCount(_selectedCrop);
        quantitySlider.wholeNumbers = true;
        quantitySlider.minValue = stock > 0 ? 1 : 0;
        quantitySlider.maxValue = Mathf.Max(stock, 1);
        quantitySlider.value = quantitySlider.minValue;
        RefreshQuantityDisplay();
    }

    void RefreshQuantityDisplay()
    {
        if (quantitySlider == null) return;
        int qty = Mathf.RoundToInt(quantitySlider.value);
        float price = priceSlider.value;
        if (quantityLabel != null) quantityLabel.text = $"Qtd: {qty}";
        if (totalLabel != null) totalLabel.text = $"{qty} × R${price:F2} = R${qty * price:F2}";
    }

    void RefreshPriceDisplay()
    {
        float asking = priceSlider.value;
        PricingSystem.Instance.SetAskingPrice(_selectedCrop, asking);

        float cost = PricingSystem.Instance.GetSeedCost(_selectedCrop);
        float margin = PricingSystem.Instance.GetMarginValue(_selectedCrop);
        float pct = PricingSystem.Instance.GetMarginPercent(_selectedCrop);

        costLabel.text = $"Custo: R${cost:F2}";
        priceLabel.text = $"Seu preço: R${asking:F2}";
        marginLabel.text = margin >= 0
            ? $"Margem: R${margin:F2} (+{pct:F0}%)"
            : $"Margem: R${margin:F2} ({pct:F0}%)";
        RefreshQuantityDisplay();
    }

    public void OnBuyerSelected(BuyerData buyer)
    {
        _selectedBuyer = buyer;
        buyerPortrait.sprite = buyer.portrait;
        buyerPortrait.color = Color.white;
        buyerPortrait.preserveAspect = true;
        buyerDialogueLine.color = new Color(0.96f, 0.88f, 0.68f, 1f);
        buyerDialogueLine.text = buyer.buyerName;
    }

    void OnSellClicked()
    {
        int stock = InventorySystem.Instance.GetCount(_selectedCrop);
        if (_selectedBuyer == null || stock == 0) return;

        int qty = quantitySlider != null
            ? Mathf.Clamp(Mathf.RoundToInt(quantitySlider.value), 1, stock)
            : 1;

        bool sold = BuyerSystem.Instance.TrySell(
            _selectedBuyer, _selectedCrop, qty,
            PricingSystem.Instance.GetAskingPrice(_selectedCrop));

        buyerDialogueLine.color = sold
            ? new Color(0.55f, 0.9f, 0.45f, 1f)
            : new Color(1f, 0.48f, 0.36f, 1f);
        buyerDialogueLine.text = sold ? _selectedBuyer.acceptLine : _selectedBuyer.rejectLine;
        HUDController.Instance?.RefreshBalance(GameManager.Instance.Balance);
        RefreshStockLabel();
        RefreshQuantityRange();
    }

    void OnEndDayClicked()
    {
        ShowDailySummary();
    }

    void ShowDailySummary()
    {
        EnsureDailySummaryPanel();
        if (dailySummaryPanel == null) { GameManager.Instance.AdvanceDay(); return; }

        int day = GameManager.Instance.CurrentDay;
        if (summaryTitleLabel != null) summaryTitleLabel.text = $"Dia {day} — Resumo";

        var sales = EndScreenController.Instance?.GetCurrentDaySales();
        var sb = new StringBuilder();
        float receita = 0f;
        if (sales == null || sales.Count == 0)
        {
            sb.AppendLine("Nada vendido hoje.");
        }
        else
        {
            sb.AppendLine("Vendido:");
            foreach (var s in sales)
            {
                sb.AppendLine($"  {s.cropName} ×{s.quantity} @ R${s.pricePerUnit:F2} = R${s.total:F2}");
                receita += s.total;
            }
        }
        if (summaryTableLabel != null) summaryTableLabel.text = sb.ToString();

        float saldoAposCusto = GameManager.Instance.Balance - GameManager.DailyCost;
        if (summaryRevenueLabel != null) summaryRevenueLabel.text = $"Receita: R${receita:F2}";
        if (summaryFixedCostLabel != null) summaryFixedCostLabel.text = $"Custo fixo: −R${GameManager.DailyCost:F2}";
        if (summaryBalanceLabel != null)
        {
            summaryBalanceLabel.text = $"Saldo: R${saldoAposCusto:F2}";
            summaryBalanceLabel.color = saldoAposCusto >= 0
                ? new Color(0.30f, 0.78f, 0.40f)
                : new Color(0.92f, 0.30f, 0.30f);
        }

        dailySummaryPanel.SetActive(true);
    }

    void OnSummaryContinue()
    {
        if (dailySummaryPanel != null) dailySummaryPanel.SetActive(false);
        EndScreenController.Instance?.ClearCurrentDaySales();
        GameManager.Instance.AdvanceDay();
    }

    // ---------- Runtime fallback construction ----------

    void DestroyLegacyCropDropdown()
    {
        var legacy = GameObject.Find("CropDropdown");
        if (legacy != null) Destroy(legacy);
    }

    void EnsureCropButtons()
    {
        if (cropButtons != null && cropButtons.Length >= 3
            && cropButtons[0] != null && cropButtons[1] != null && cropButtons[2] != null)
            return;

        Transform tradePanel = null;
        var tradeGO = GameObject.Find("TradePanel");
        if (tradeGO != null) tradePanel = tradeGO.transform;
        if (tradePanel == null)
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null) return;
            tradePanel = canvas.transform;
        }

        var made = new Button[3];
        float[] yPositions = { 148f, 108f, 68f };
        for (int i = 0; i < 3; i++)
        {
            var go = new GameObject($"CropButton_{CropNames[i]}");
            go.transform.SetParent(tradePanel, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(-170f, yPositions[i]);
            rt.sizeDelta = new Vector2(210f, 36f);

            var img = go.AddComponent<Image>();
            img.color = CropButtonNormal;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var lrt = labelGO.AddComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            var label = labelGO.AddComponent<TextMeshProUGUI>();
            label.text = LabelForCrop(CropNames[i]);
            label.fontSize = 18;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(0.96f, 0.88f, 0.68f, 1f);
            label.raycastTarget = false;
            TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font != null) label.font = font;

            made[i] = btn;
        }

        cropButtons = made;
    }

    static string LabelForCrop(string cropName) => cropName == "Acai" ? "Açaí" : cropName;

    void EnsureQuantitySlider()
    {
        if (quantitySlider != null) return;
        if (priceSlider == null) return;

        Transform parent = priceSlider.transform.parent;
        if (parent == null) return;

        var go = new GameObject("QuantitySlider");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        var priceRT = priceSlider.GetComponent<RectTransform>();
        rt.anchorMin = priceRT.anchorMin;
        rt.anchorMax = priceRT.anchorMax;
        rt.pivot = priceRT.pivot;
        rt.sizeDelta = priceRT.sizeDelta;
        rt.anchoredPosition = priceRT.anchoredPosition + new Vector2(0f, -56f);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(0.32f, 0.22f, 0.12f, 0.85f);

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        var faRT = fillArea.AddComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0f, 0.25f);
        faRT.anchorMax = new Vector2(1f, 0.75f);
        faRT.offsetMin = new Vector2(8f, 0f);
        faRT.offsetMax = new Vector2(-8f, 0f);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.85f, 0.65f, 0.25f, 1f);

        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        var haRT = handleArea.AddComponent<RectTransform>();
        haRT.anchorMin = Vector2.zero;
        haRT.anchorMax = Vector2.one;
        haRT.offsetMin = Vector2.zero;
        haRT.offsetMax = Vector2.zero;

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var hRT = handle.AddComponent<RectTransform>();
        hRT.sizeDelta = new Vector2(20f, 28f);
        var hImg = handle.AddComponent<Image>();
        hImg.color = new Color(0.98f, 0.88f, 0.55f, 1f);

        var slider = go.AddComponent<Slider>();
        slider.fillRect = fillRT;
        slider.handleRect = hRT;
        slider.targetGraphic = hImg;
        slider.direction = Slider.Direction.LeftToRight;
        slider.wholeNumbers = true;
        slider.minValue = 1;
        slider.maxValue = 10;
        slider.value = 1;
        quantitySlider = slider;

        if (quantityLabel == null)
            quantityLabel = MakeLabelNear(parent, "QuantityLabel",
                rt.anchoredPosition + new Vector2(-180f, 0f),
                new Vector2(140f, 28f), 16, "Qtd: 1");
        if (totalLabel == null)
            totalLabel = MakeLabelNear(parent, "TotalLabel",
                rt.anchoredPosition + new Vector2(0f, -32f),
                new Vector2(360f, 24f), 16, "");
    }

    void EnsureDailySummaryPanel()
    {
        if (dailySummaryPanel != null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        var panel = new GameObject("DailySummaryPanel");
        panel.transform.SetParent(canvas.transform, false);
        var prt = panel.AddComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;
        var dim = panel.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.72f);

        var card = new GameObject("Card");
        card.transform.SetParent(panel.transform, false);
        var crt = card.AddComponent<RectTransform>();
        crt.anchorMin = new Vector2(0.5f, 0.5f);
        crt.anchorMax = new Vector2(0.5f, 0.5f);
        crt.pivot = new Vector2(0.5f, 0.5f);
        crt.sizeDelta = new Vector2(560f, 460f);
        var cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(0.13f, 0.10f, 0.07f, 0.98f);

        summaryTitleLabel = MakeLabelInRect(card.transform, "Title",
            new Vector2(0f, 175f), new Vector2(520f, 44f), 28, "Dia — Resumo");
        summaryTitleLabel.color = new Color(0.98f, 0.84f, 0.45f);

        summaryTableLabel = MakeLabelInRect(card.transform, "Table",
            new Vector2(0f, 30f), new Vector2(500f, 220f), 18, "");
        summaryTableLabel.alignment = TextAlignmentOptions.TopLeft;
        summaryTableLabel.color = new Color(0.95f, 0.92f, 0.84f);

        summaryRevenueLabel = MakeLabelInRect(card.transform, "Revenue",
            new Vector2(0f, -86f), new Vector2(500f, 26f), 18, "Receita: R$0,00");
        summaryRevenueLabel.alignment = TextAlignmentOptions.Right;

        summaryFixedCostLabel = MakeLabelInRect(card.transform, "FixedCost",
            new Vector2(0f, -114f), new Vector2(500f, 26f), 18, "Custo fixo: −R$2,00");
        summaryFixedCostLabel.alignment = TextAlignmentOptions.Right;
        summaryFixedCostLabel.color = new Color(0.92f, 0.55f, 0.45f);

        var divider = new GameObject("Divider");
        divider.transform.SetParent(card.transform, false);
        var drt = divider.AddComponent<RectTransform>();
        drt.anchorMin = new Vector2(0.5f, 0.5f);
        drt.anchorMax = new Vector2(0.5f, 0.5f);
        drt.pivot = new Vector2(0.5f, 0.5f);
        drt.anchoredPosition = new Vector2(0f, -136f);
        drt.sizeDelta = new Vector2(500f, 2f);
        divider.AddComponent<Image>().color = new Color(0.6f, 0.45f, 0.22f, 0.6f);

        summaryBalanceLabel = MakeLabelInRect(card.transform, "Balance",
            new Vector2(0f, -160f), new Vector2(500f, 32f), 22, "Saldo: R$0,00");
        summaryBalanceLabel.alignment = TextAlignmentOptions.Right;

        var btnGO = new GameObject("ContinueButton");
        btnGO.transform.SetParent(card.transform, false);
        var brt = btnGO.AddComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.5f, 0.5f);
        brt.anchorMax = new Vector2(0.5f, 0.5f);
        brt.pivot = new Vector2(0.5f, 0.5f);
        brt.anchoredPosition = new Vector2(0f, -210f);
        brt.sizeDelta = new Vector2(220f, 50f);
        var btnImg = btnGO.AddComponent<Image>();
        btnImg.color = new Color(0.88f, 0.62f, 0.22f, 1f);
        summaryContinueButton = btnGO.AddComponent<Button>();
        summaryContinueButton.targetGraphic = btnImg;
        var btnLabel = MakeLabelInRect(btnGO.transform, "Label",
            Vector2.zero, new Vector2(200f, 40f), 18, "Continuar");
        btnLabel.color = new Color(0.10f, 0.07f, 0.04f);

        panel.SetActive(false);
        dailySummaryPanel = panel;
    }

    static TMP_Text MakeLabelNear(Transform parent, string name,
        Vector2 anchoredPos, Vector2 size, int fontSize, string text)
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
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Left;
        label.color = new Color(0.96f, 0.88f, 0.68f, 1f);
        label.raycastTarget = false;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) label.font = font;
        return label;
    }

    static TMP_Text MakeLabelInRect(Transform parent, string name,
        Vector2 anchoredPos, Vector2 size, int fontSize, string text)
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
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) label.font = font;
        return label;
    }
}
