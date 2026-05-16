using System.Collections;
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
    [SerializeField] Button minusButton;
    [SerializeField] Button plusButton;
    [SerializeField] TMP_Text quantityValueLabel;
    [SerializeField] TMP_Text quantityLabel;
    [SerializeField] TMP_Text totalLabel;
    [SerializeField] Image coinIcon;

    int _quantity = 1;
    int _maxQuantity = 1;

    [Header("Buyer")]
    [SerializeField] BuyerSelector buyerSelector;
    [SerializeField] TMP_Text buyerDialogueLine;
    [SerializeField] Image buyerPortrait;

    [Header("Action")]
    [SerializeField] Button sellButton;
    [SerializeField] TMP_Text sellButtonLabel;
    [SerializeField] Button endDayButton;

    [Header("Sell Feedback")]
    [SerializeField] Image sellFeedbackIcon;
    [SerializeField] GameObject insightToastPanel;
    [SerializeField] TMP_Text insightToastLabel;

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
    Coroutine _sellFeedbackRoutine;
    Coroutine _insightToastRoutine;

    void Start()
    {
        SceneCameraUtility.EnsureUICamera();
        SceneCameraUtility.EnsureEventSystem();

        DestroyLegacyCropDropdown();
        DestroyLegacyQuantitySlider();
        EnsureCropButtons();
        EnsureQuantityStepper();
        EnsureSellFeedbackIcon();
        EnsureInsightToast();
        EnsureDailySummaryPanel();
        EnsureDailyEducationFlow();

        if (priceSlider == null || sellButton == null || endDayButton == null) return;

        WireCropButtons();
        priceSlider.onValueChanged.AddListener(_ => RefreshPriceDisplay());
        if (minusButton != null) minusButton.onClick.AddListener(() => ChangeQuantity(-1));
        if (plusButton != null) plusButton.onClick.AddListener(() => ChangeQuantity(+1));
        sellButton.onClick.AddListener(OnSellClicked);
        endDayButton.onClick.AddListener(OnEndDayClicked);
        if (summaryContinueButton != null)
            summaryContinueButton.onClick.AddListener(OnSummaryContinue);

        SelectCropByIndex(0);
    }

    void ChangeQuantity(int delta)
    {
        if (_maxQuantity <= 0) { _quantity = 0; RefreshQuantityDisplay(); return; }
        _quantity = Mathf.Clamp(_quantity + delta, 1, _maxQuantity);
        RefreshQuantityDisplay();
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
        UpdateSellButtonState();
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
        // "Estoque" é jargão adulto; "Na sacola" pra criança 8-11 (Modelo C §3.4).
        stockLabel.text = qty == 1
            ? $"Na sacola: 1"
            : $"Na sacola: {qty}";
    }

    void RefreshQuantityRange()
    {
        int stock = InventorySystem.Instance != null
            ? InventorySystem.Instance.GetCount(_selectedCrop)
            : 0;
        _maxQuantity = Mathf.Max(stock, 0);
        _quantity = stock > 0 ? 1 : 0;
        RefreshQuantityDisplay();
    }

    void RefreshQuantityDisplay()
    {
        float price = priceSlider != null ? priceSlider.value : 0f;
        if (quantityValueLabel != null) quantityValueLabel.text = _quantity.ToString();
        if (quantityLabel != null) quantityLabel.text = "Quantos vender:";
        if (totalLabel != null)
        {
            totalLabel.text = _quantity > 0
                ? $"Total: R${_quantity * price:F2}"
                : "Sem nada na sacola";
        }
        if (minusButton != null) minusButton.interactable = _quantity > 1;
        if (plusButton != null) plusButton.interactable = _quantity < _maxQuantity;
        UpdateSellButtonState();
    }

    void UpdateSellButtonState()
    {
        if (sellButton == null) return;
        if (sellButtonLabel == null)
            sellButtonLabel = sellButton.GetComponentInChildren<TMP_Text>();

        bool canSell = _selectedBuyer != null && _maxQuantity > 0 && _quantity > 0;
        sellButton.interactable = canSell;

        if (sellButtonLabel != null)
        {
            if (_selectedBuyer == null) sellButtonLabel.text = "Escolha um comprador";
            else if (_maxQuantity == 0) sellButtonLabel.text = "Você não colheu nada";
            else sellButtonLabel.text = $"Vender {_quantity} {PluralizeCrop(_selectedCrop, _quantity)}";
        }
    }

    static string PluralizeCrop(string crop, int qty)
    {
        string singular = crop switch
        {
            "Mandioca" => "mandioca",
            "Cacau" => "cacau",
            "Acai" => "açaí",
            _ => crop != null ? crop.ToLower() : ""
        };
        if (qty == 1) return singular;
        return crop switch
        {
            "Mandioca" => "mandiocas",
            "Cacau" => "cacaus",
            "Acai" => "açaís",
            _ => singular + "s"
        };
    }

    void RefreshPriceDisplay()
    {
        float asking = priceSlider.value;
        PricingSystem.Instance.SetAskingPrice(_selectedCrop, asking);

        float cost = PricingSystem.Instance.GetSeedCost(_selectedCrop);
        float margin = PricingSystem.Instance.GetMarginValue(_selectedCrop);

        costLabel.text = $"Custou: R${cost:F2}";
        priceLabel.text = $"Seu preço: R${asking:F2}";
        if (marginLabel != null)
        {
            // Vocabulário 8-11: "sobra" no lugar de "lucro" (Modelo C §3.4).
            string label;
            Color color;
            if (margin > 0)
            {
                label = $"Sobra de R${margin:F2}";
                color = new Color(0.32f, 0.80f, 0.42f, 1f);
            }
            else if (margin < 0)
            {
                label = $"Faltam R${Mathf.Abs(margin):F2}";
                color = new Color(0.92f, 0.36f, 0.32f, 1f);
            }
            else
            {
                label = "Empata: nem sobra nem falta";
                color = new Color(0.92f, 0.85f, 0.55f, 1f);
            }
            marginLabel.text = label;
            marginLabel.color = color;
        }
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
        UpdateSellButtonState();
    }

    void OnSellClicked()
    {
        if (_selectedBuyer == null || _maxQuantity == 0 || _quantity == 0) return;

        int qty = Mathf.Clamp(_quantity, 1, _maxQuantity);
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
        UpdateSellButtonState();
        ShowSellFeedback(sold);
        ShowFirstSaleInsightIfNeeded(sold, qty);
    }

    void ShowSellFeedback(bool success)
    {
        if (sellFeedbackIcon == null) return;
        if (_sellFeedbackRoutine != null) StopCoroutine(_sellFeedbackRoutine);
        _sellFeedbackRoutine = StartCoroutine(AnimateSellFeedback(success));
    }

    IEnumerator AnimateSellFeedback(bool success)
    {
        sellFeedbackIcon.color = success
            ? new Color(0.30f, 0.85f, 0.40f, 1f)
            : new Color(0.95f, 0.40f, 0.35f, 1f);
        var iconLabel = sellFeedbackIcon.GetComponentInChildren<TMP_Text>();
        if (iconLabel != null) iconLabel.text = success ? "✓" : "✗";

        var rt = sellFeedbackIcon.rectTransform;
        var go = sellFeedbackIcon.gameObject;
        go.SetActive(true);

        // Pop-in com scale e fade out
        float dur = 0.85f;
        float t = 0f;
        Color baseColor = sellFeedbackIcon.color;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = t / dur;
            float scale = k < 0.25f
                ? Mathf.Lerp(0.4f, 1.25f, k / 0.25f)
                : Mathf.Lerp(1.25f, 1.0f, (k - 0.25f) / 0.75f);
            rt.localScale = new Vector3(scale, scale, 1f);
            float a = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.5f, 1f, k));
            sellFeedbackIcon.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            if (iconLabel != null)
            {
                var c = iconLabel.color;
                c.a = a;
                iconLabel.color = c;
            }
            yield return null;
        }
        go.SetActive(false);
        _sellFeedbackRoutine = null;
    }

    void ShowFirstSaleInsightIfNeeded(bool sold, int qty)
    {
        if (!sold || PricingSystem.Instance == null || GameManager.Instance == null) return;

        float price = PricingSystem.Instance.GetAskingPrice(_selectedCrop);
        float seedCost = PricingSystem.Instance.GetSeedCost(_selectedCrop);
        float unitSobra = price - seedCost;
        if (Mathf.Approximately(unitSobra, 0f)) return;

        bool isSobra = unitSobra > 0f;
        if (!IsFirstSaleWithSobraState(isSobra)) return;

        float total = Mathf.Abs(unitSobra * qty);
        string text = isSobra
            ? $"Boa! Nessa venda sobrou R${total:F2}: você recebeu mais do que gastou na semente."
            : $"Atenção: nessa venda faltou R${total:F2}. Você vendeu por menos do que pagou na semente.";
        Color color = isSobra
            ? new Color(0.32f, 0.80f, 0.42f, 1f)
            : new Color(0.95f, 0.62f, 0.32f, 1f);
        ShowInsightToast(text, color);
    }

    bool IsFirstSaleWithSobraState(bool isSobra)
    {
        var sales = GameManager.Instance.Sales;
        if (sales == null) return true;

        int matching = 0;
        foreach (var sale in sales)
        {
            float cost = PricingSystem.Instance.GetSeedCost(sale.cropName);
            float sobra = sale.pricePerUnit - cost;
            if (Mathf.Approximately(sobra, 0f)) continue;
            if ((sobra > 0f) == isSobra) matching++;
        }

        // BuyerSystem records the current sale before this method runs.
        return matching == 1;
    }

    void ShowInsightToast(string text, Color accent)
    {
        EnsureInsightToast();
        if (insightToastPanel == null || insightToastLabel == null) return;

        insightToastLabel.text = text;
        insightToastLabel.color = accent;
        insightToastPanel.SetActive(true);
        if (_insightToastRoutine != null) StopCoroutine(_insightToastRoutine);
        _insightToastRoutine = StartCoroutine(HideInsightToastAfterDelay());
    }

    IEnumerator HideInsightToastAfterDelay()
    {
        yield return new WaitForSeconds(3.2f);
        if (insightToastPanel != null) insightToastPanel.SetActive(false);
        _insightToastRoutine = null;
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
        // Vocabulário 8-11 (Modelo C §3.4): receita/custo/saldo viram dinheiro recebido/sustento/no fim.
        if (summaryTitleLabel != null) summaryTitleLabel.text = $"Fim do dia {day}";

        var sales = EndScreenController.Instance?.GetCurrentDaySales();
        var sb = new StringBuilder();
        float receita = 0f;
        if (sales == null || sales.Count == 0)
        {
            sb.AppendLine("Você não vendeu nada hoje.");
        }
        else
        {
            sb.AppendLine("Você vendeu:");
            foreach (var s in sales)
            {
                sb.AppendLine($"  {s.quantity} × {s.cropName} a R${s.pricePerUnit:F2} = R${s.total:F2}");
                receita += s.total;
            }
        }
        if (summaryTableLabel != null) summaryTableLabel.text = sb.ToString();

        float saldoAposCusto = GameManager.Instance.Balance - GameManager.DailyCost;
        if (summaryRevenueLabel != null) summaryRevenueLabel.text = $"Você ganhou: R${receita:F2}";
        if (summaryFixedCostLabel != null) summaryFixedCostLabel.text = $"Custo do sustento: −R${GameManager.DailyCost:F2}";
        if (summaryBalanceLabel != null)
        {
            summaryBalanceLabel.text = $"Dinheiro no fim do dia: R${saldoAposCusto:F2}";
            summaryBalanceLabel.color = saldoAposCusto >= 0
                ? new Color(0.30f, 0.78f, 0.40f)
                : new Color(0.92f, 0.30f, 0.30f);
        }

        dailySummaryPanel.SetActive(true);
    }

    void OnSummaryContinue()
    {
        if (dailySummaryPanel != null) dailySummaryPanel.SetActive(false);
        if (DailyEducationFlow.Instance != null)
        {
            DailyEducationFlow.Instance.StartFlow();
            return;
        }

        EndScreenController.Instance?.ClearCurrentDaySales();
        GameManager.Instance.AdvanceDay();
    }

    // ---------- Runtime fallback construction ----------

    void DestroyLegacyCropDropdown()
    {
        var legacy = GameObject.Find("CropDropdown");
        if (legacy != null) Destroy(legacy);
    }

    void DestroyLegacyQuantitySlider()
    {
        var legacy = GameObject.Find("QuantitySlider");
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

    void EnsureQuantityStepper()
    {
        if (minusButton != null && plusButton != null && quantityValueLabel != null) return;
        if (priceSlider == null) return;

        Transform parent = priceSlider.transform.parent;
        if (parent == null) return;

        Vector2 baseAnchor = priceSlider.GetComponent<RectTransform>().anchoredPosition
                             + new Vector2(0f, -68f);

        if (quantityLabel == null)
            quantityLabel = MakeLabelNear(parent, "QuantityLabel",
                baseAnchor + new Vector2(-200f, 0f),
                new Vector2(220f, 30f), 18, "Quantos vender:");

        if (minusButton == null)
            minusButton = MakeStepperButton(parent, "MinusButton",
                baseAnchor + new Vector2(-30f, 0f), "−");

        if (quantityValueLabel == null)
            quantityValueLabel = MakeLabelNear(parent, "QuantityValueLabel",
                baseAnchor + new Vector2(30f, 0f),
                new Vector2(80f, 60f), 36, "1");

        if (quantityValueLabel != null)
        {
            quantityValueLabel.alignment = TextAlignmentOptions.Center;
            quantityValueLabel.color = new Color(0.98f, 0.88f, 0.55f, 1f);
            quantityValueLabel.fontStyle = FontStyles.Bold;
        }

        if (plusButton == null)
            plusButton = MakeStepperButton(parent, "PlusButton",
                baseAnchor + new Vector2(90f, 0f), "+");

        if (totalLabel == null)
            totalLabel = MakeLabelNear(parent, "TotalLabel",
                baseAnchor + new Vector2(0f, -42f),
                new Vector2(360f, 26f), 18, "");
        if (totalLabel != null)
            totalLabel.alignment = TextAlignmentOptions.Center;
    }

    void EnsureSellFeedbackIcon()
    {
        if (sellFeedbackIcon != null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        var go = new GameObject("SellFeedbackIcon");
        go.transform.SetParent(canvas.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(-280f, 30f);
        rt.sizeDelta = new Vector2(120f, 120f);
        sellFeedbackIcon = go.AddComponent<Image>();
        sellFeedbackIcon.color = new Color(0.30f, 0.85f, 0.40f, 1f);
        sellFeedbackIcon.raycastTarget = false;

        var labelGO = new GameObject("Glyph");
        labelGO.transform.SetParent(go.transform, false);
        var lrt = labelGO.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;
        var tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "✓";
        tmp.fontSize = 90;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.10f, 0.07f, 0.04f, 1f);
        tmp.raycastTarget = false;
        var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) tmp.font = font;

        go.SetActive(false);
    }

    void EnsureInsightToast()
    {
        if (insightToastPanel != null && insightToastLabel != null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        var panel = new GameObject("InsightToast");
        panel.transform.SetParent(canvas.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -84f);
        rt.sizeDelta = new Vector2(680f, 74f);
        var bg = panel.AddComponent<Image>();
        bg.color = new Color(0.12f, 0.08f, 0.05f, 0.94f);

        insightToastLabel = MakeLabelInRect(panel.transform, "Label",
            Vector2.zero, new Vector2(620f, 60f), 18, "");
        insightToastLabel.alignment = TextAlignmentOptions.Center;
        insightToastLabel.textWrappingMode = TextWrappingModes.Normal;

        panel.SetActive(false);
        insightToastPanel = panel;
    }

    Button MakeStepperButton(Transform parent, string name, Vector2 anchoredPos, string glyph)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(60f, 60f);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.88f, 0.62f, 0.22f, 1f);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        var lrt = labelGO.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;
        var tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text = glyph;
        tmp.fontSize = 38;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.10f, 0.07f, 0.04f, 1f);
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;
        var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) tmp.font = font;

        return btn;
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
            new Vector2(0f, -86f), new Vector2(500f, 26f), 18, "Você ganhou: R$0,00");
        summaryRevenueLabel.alignment = TextAlignmentOptions.Right;

        summaryFixedCostLabel = MakeLabelInRect(card.transform, "FixedCost",
            new Vector2(0f, -114f), new Vector2(500f, 26f), 18, "Custo do sustento: −R$2,00");
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
            new Vector2(0f, -160f), new Vector2(500f, 32f), 22, "Dinheiro no fim do dia: R$0,00");
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

    void EnsureDailyEducationFlow()
    {
        if (DailyEducationFlow.Instance != null) return;
        var go = new GameObject("DailyEducationFlow");
        go.transform.SetParent(transform, false);
        go.AddComponent<DailyEducationFlow>();
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
