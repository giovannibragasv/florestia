using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour
{
    public static EndScreenController Instance { get; private set; }

    [Header("Summary")]
    [SerializeField] TMP_Text finalBalanceLabel;
    [SerializeField] TMP_Text outcomeLabel;
    [SerializeField] TMP_Text educationalLabel;
    [SerializeField] TMP_Text bestCropLabel;

    [Header("Balance Chart")]
    [SerializeField] RectTransform chartContainer;
    [SerializeField] GameObject barPrefab;

    [Header("Actions")]
    [SerializeField] Button playAgainButton;

    readonly Dictionary<string, float> _revenuePerCrop = new();
    readonly List<DailySale> _currentDaySales = new();

    public struct DailySale
    {
        public string cropName;
        public int quantity;
        public float pricePerUnit;
        public float total;
    }

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

    void Start()
    {
        SceneCameraUtility.EnsureUICamera();
        SceneCameraUtility.EnsureEventSystem();

        WirePlayAgainButton();

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            BuildEndScreen();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "EndScreen")
            BuildEndScreen();
    }

    public void RecordSale(string cropName, float revenue, int quantity, float pricePerUnit)
    {
        _revenuePerCrop.TryGetValue(cropName, out float current);
        _revenuePerCrop[cropName] = current + revenue;
        _currentDaySales.Add(new DailySale
        {
            cropName = cropName,
            quantity = quantity,
            pricePerUnit = pricePerUnit,
            total = revenue
        });
    }

    public IReadOnlyList<DailySale> GetCurrentDaySales() => _currentDaySales;

    public void ClearCurrentDaySales() => _currentDaySales.Clear();

    void BuildEndScreen()
    {
        SceneCameraUtility.EnsureUICamera();
        SceneCameraUtility.EnsureEventSystem();
        EnsureEndScreenUI();
        PolishEndScreenStyle();

        float final = GameManager.Instance.Balance;
        // Vocabulário 8-11 (Modelo C §3.4): "dinheiro no fim" no lugar de "saldo final".
        finalBalanceLabel.text = $"Dinheiro no fim: F${final:F0}";

        if (GameManager.Instance.IsLoss)
        {
            outcomeLabel.text = "Acabou o dinheiro. Tenta de novo!";
            outcomeLabel.color = new Color(0.9f, 0.42f, 0.32f);
        }
        else if (final > GameManager.StartingBalance)
        {
            outcomeLabel.text = "Você plantou, vendeu e sobrou dinheiro!";
            outcomeLabel.color = new Color(0.32f, 0.80f, 0.42f);
        }
        else
        {
            outcomeLabel.text = "Você terminou os 15 dias!";
            outcomeLabel.color = new Color(0.95f, 0.84f, 0.35f);
        }

        // Hero zone com pictograma simples por desfecho; arte final entra em C09/FA05.
        RefreshHeroZone();

        // Tip contextual baseado em comportamento real (A05.7)
        SetEducationalText(GetContextualTip());

        string best = GetBestCrop();
        bestCropLabel.text = best != null ? $"O que mais te rendeu: {best}" : "";

        BuildBalanceChart();
    }

    Image _heroBand;
    Image _heroIllustrationBg;
    TMP_Text _heroIllustrationIcon;
    TMP_Text _heroIllustrationCaption;
    void RefreshHeroZone()
    {
        if (outcomeLabel == null) return;
        if (_heroBand == null)
        {
            var go = new GameObject("HeroBand");
            go.transform.SetParent(outcomeLabel.transform.parent, false);
            go.transform.SetAsFirstSibling();
            var rt = go.AddComponent<RectTransform>();
            var olrt = outcomeLabel.rectTransform;
            rt.anchorMin = olrt.anchorMin;
            rt.anchorMax = olrt.anchorMax;
            rt.pivot = olrt.pivot;
            rt.anchoredPosition = olrt.anchoredPosition;
            rt.sizeDelta = new Vector2(olrt.sizeDelta.x + 80f, olrt.sizeDelta.y + 30f);
            _heroBand = go.AddComponent<Image>();
            _heroBand.raycastTarget = false;
        }
        EnsureHeroIllustration();

        Color c;
        string icon;
        string caption;
        if (GameManager.Instance.IsLoss)
        {
            c = new Color(0.92f, 0.42f, 0.32f, 0.16f);
            icon = "...";
            caption = "pensar e tentar";
        }
        else if (GameManager.Instance.Balance > GameManager.StartingBalance)
        {
            c = new Color(0.32f, 0.80f, 0.42f, 0.18f);
            icon = "F$";
            caption = "sobrou dinheiro";
        }
        else
        {
            c = new Color(0.95f, 0.84f, 0.35f, 0.16f);
            icon = "OK";
            caption = "colheita modesta";
        }
        _heroBand.color = c;
        if (_heroIllustrationBg != null)
            _heroIllustrationBg.color = new Color(c.r, c.g, c.b, 0.35f);
        if (_heroIllustrationIcon != null)
            _heroIllustrationIcon.text = icon;
        if (_heroIllustrationCaption != null)
            _heroIllustrationCaption.text = caption;
    }

    void EnsureHeroIllustration()
    {
        if (_heroIllustrationBg != null) return;

        var go = new GameObject("HeroIllustration");
        go.transform.SetParent(outcomeLabel.transform.parent, false);
        var rt = go.AddComponent<RectTransform>();
        var olrt = outcomeLabel.rectTransform;
        rt.anchorMin = olrt.anchorMin;
        rt.anchorMax = olrt.anchorMax;
        rt.pivot = olrt.pivot;
        rt.anchoredPosition = olrt.anchoredPosition + new Vector2(-270f, 0f);
        rt.sizeDelta = new Vector2(150f, 118f);
        _heroIllustrationBg = go.AddComponent<Image>();
        _heroIllustrationBg.raycastTarget = false;

        _heroIllustrationIcon = MakeLabel("Icon", go.transform,
            "OK",
            new Vector2(0f, 14f), new Vector2(130f, 56f), 34,
            new Color(0.98f, 0.88f, 0.55f, 1f));
        _heroIllustrationIcon.fontStyle = FontStyles.Bold;

        _heroIllustrationCaption = MakeLabel("Caption", go.transform,
            "",
            new Vector2(0f, -34f), new Vector2(132f, 34f), 15,
            new Color(0.95f, 0.92f, 0.84f, 1f));
        _heroIllustrationCaption.textWrappingMode = TextWrappingModes.Normal;
    }

    void SetEducationalText(string text)
    {
        if (educationalLabel != null)
            educationalLabel.text = text;
    }

    string GetBestCrop()
    {
        string best = null;
        float top = float.MinValue;
        foreach (var kv in _revenuePerCrop)
            if (kv.Value > top) { top = kv.Value; best = kv.Key; }
        return best;
    }

    void BuildBalanceChart()
    {
        List<float> balances = GameManager.Instance.DailyBalances;
        if (chartContainer == null || balances.Count == 0) return;

        for (int i = chartContainer.childCount - 1; i >= 0; i--)
            Destroy(chartContainer.GetChild(i).gameObject);

        // Inclui o 0 no range pra que a linha de zero seja sempre visível e
        // as barras negativas saiam pra baixo de forma consistente.
        float max = 0f, min = 0f;
        foreach (float v in balances) { if (v > max) max = v; if (v < min) min = v; }
        float range = Mathf.Max(max - min, 1f);

        float containerHeight = chartContainer.rect.height;
        float containerWidth = chartContainer.rect.width;
        float barWidth = containerWidth / balances.Count;
        float zeroY = -containerHeight * 0.5f + (0f - min) / range * containerHeight;

        // 1. Linha horizontal de zero (A05.4)
        var zeroLine = new GameObject("ZeroLine");
        zeroLine.transform.SetParent(chartContainer, false);
        var zlrt = zeroLine.AddComponent<RectTransform>();
        zlrt.anchorMin = new Vector2(0f, 0.5f);
        zlrt.anchorMax = new Vector2(1f, 0.5f);
        zlrt.pivot = new Vector2(0.5f, 0.5f);
        zlrt.anchoredPosition = new Vector2(0f, zeroY);
        zlrt.sizeDelta = new Vector2(0f, 1.5f);
        zeroLine.AddComponent<Image>().color = new Color(0.85f, 0.78f, 0.45f, 0.55f);

        int bankruptcyDay = -1;
        for (int i = 0; i < balances.Count; i++)
        {
            float v = balances[i];

            // 2. Barras coloridas por sinal (A05.6)
            GameObject bar = Instantiate(barPrefab, chartContainer);
            bar.SetActive(true);
            RectTransform rt = bar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, v >= 0 ? 0f : 1f);

            float barHeightPx = Mathf.Abs(v) / range * containerHeight;
            rt.sizeDelta = new Vector2(barWidth - 4f, barHeightPx);
            rt.anchoredPosition = new Vector2(i * barWidth + 2f, zeroY);

            Image img = bar.GetComponent<Image>();
            img.color = v >= 0
                ? new Color(0.32f, 0.78f, 0.42f, 1f)
                : new Color(0.92f, 0.38f, 0.34f, 1f);

            if (bankruptcyDay < 0 && v < 0f) bankruptcyDay = i;

            // 3. Day labels (A05.3) — exibe apenas a cada 2 dias pra não poluir
            if (i % 2 == 0 || i == balances.Count - 1)
            {
                var dayLabel = new GameObject($"DayLabel_{i}");
                dayLabel.transform.SetParent(chartContainer, false);
                var dlrt = dayLabel.AddComponent<RectTransform>();
                dlrt.anchorMin = new Vector2(0f, 0f);
                dlrt.anchorMax = new Vector2(0f, 0f);
                dlrt.pivot = new Vector2(0.5f, 1f);
                dlrt.anchoredPosition = new Vector2(i * barWidth + barWidth * 0.5f, -2f);
                dlrt.sizeDelta = new Vector2(barWidth, 18f);
                var dlt = dayLabel.AddComponent<TextMeshProUGUI>();
                dlt.text = (i + 1).ToString();
                dlt.fontSize = 11;
                dlt.alignment = TextAlignmentOptions.Center;
                dlt.color = new Color(0.78f, 0.72f, 0.58f, 1f);
                dlt.raycastTarget = false;
                var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                if (font != null) dlt.font = font;
            }
        }

        // 4. Marker do dia da virada (A05.5)
        if (bankruptcyDay >= 0)
        {
            var marker = new GameObject("BankruptcyMarker");
            marker.transform.SetParent(chartContainer, false);
            var mrt = marker.AddComponent<RectTransform>();
            mrt.anchorMin = new Vector2(0f, 0.5f);
            mrt.anchorMax = new Vector2(0f, 0.5f);
            mrt.pivot = new Vector2(0.5f, 0.5f);
            mrt.anchoredPosition = new Vector2(bankruptcyDay * barWidth + barWidth * 0.5f, zeroY + 30f);
            mrt.sizeDelta = new Vector2(barWidth * 2f, 22f);
            var mlt = marker.AddComponent<TextMeshProUGUI>();
            mlt.text = $"↓ aqui acabou o dinheiro";
            mlt.fontSize = 12;
            mlt.alignment = TextAlignmentOptions.Center;
            mlt.color = new Color(0.95f, 0.42f, 0.38f, 1f);
            mlt.fontStyle = FontStyles.Bold;
            mlt.raycastTarget = false;
            var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font != null) mlt.font = font;
        }
    }

    // Tip contextual baseado no comportamento real do aluno (A05.7)
    string GetContextualTip()
    {
        var gm = GameManager.Instance;
        if (gm == null) return "";

        // Padrão 1: vendeu abaixo do custo da semente em alguma venda
        if (gm.Sales != null)
        {
            foreach (var s in gm.Sales)
            {
                float cost = PricingSystem.Instance != null
                    ? PricingSystem.Instance.GetSeedCost(s.cropName)
                    : 0f;
                if (cost > 0 && s.pricePerUnit < cost)
                    return $"Você vendeu {s.cropName} por F${s.pricePerUnit:F0}, mas pagou F${cost:F0} na semente. Tente cobrar mais alto na próxima!";
            }
        }

        // Padrão 2: só plantou uma cultura
        if (gm.Plantings != null && gm.Plantings.Count > 0)
        {
            string first = gm.Plantings[0].cropName;
            bool single = true;
            foreach (var p in gm.Plantings)
                if (!string.Equals(p.cropName, first, System.StringComparison.OrdinalIgnoreCase))
                { single = false; break; }
            if (single)
                return $"Você só plantou {first}. Da próxima, tenta misturar as três culturas pra ver qual rende mais!";
        }

        // Padrão 3: vendeu muito para o comprador de oferta baixa
        if (gm.Sales != null && gm.Sales.Count >= 3)
        {
            int atravessador = 0;
            foreach (var s in gm.Sales)
                if (IsLowPriceBuyer(s.buyerName))
                    atravessador++;
            if (atravessador * 2 > gm.Sales.Count)
                return "O Comprador da Estrada compra rápido. Tente o Feirante ou o Comprador Direto pra comparar os preços!";
        }

        // Default por desfecho
        if (gm.IsLoss)
            return "Não desanima! Plantar variedade e cobrar mais alto ajuda o dinheiro durar.";
        if (gm.Balance > GameManager.StartingBalance + 30f)
            return $"Você terminou com F${gm.Balance:F0}! Bom plano de plantio e venda.";
        return "Você sobreviveu aos 15 dias. Da próxima, tenta render mais!";
    }

    static bool IsLowPriceBuyer(string buyerName)
    {
        if (string.IsNullOrEmpty(buyerName)) return false;
        string lower = buyerName.ToLowerInvariant();
        return lower.Contains("travessador") || lower.Contains("estrada");
    }

    void OnPlayAgain()
    {
        _revenuePerCrop.Clear();
        if (GameManager.Instance != null)
            GameManager.Instance.ResetRun();
        else
            SaveSystem.Delete();

        SceneManager.LoadScene("FarmScene");
    }

    void EnsureEndScreenUI()
    {
        TryFindSceneReferences();

        if (finalBalanceLabel != null &&
            outcomeLabel != null &&
            educationalLabel != null &&
            bestCropLabel != null &&
            chartContainer != null &&
            barPrefab != null &&
            playAgainButton != null)
        {
            WirePlayAgainButton();
            return;
        }

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            var canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        Transform ct = canvas.transform;
        MakePanel("Background", ct, Vector2.zero, Vector2.one, Vector2.zero,
            Vector2.zero, new Color(0.09f, 0.08f, 0.07f, 1f));
        MakePanel("EndPanel", ct, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(760f, 520f), new Color(0.13f, 0.10f, 0.08f, 0.96f));

        finalBalanceLabel = MakeLabel("FinalBalanceLabel", ct, "Dinheiro no fim: F$0",
            new Vector2(0f, 140f), new Vector2(520f, 42f), 26, Color.white);
        outcomeLabel = MakeLabel("OutcomeLabel", ct, "-",
            new Vector2(0f, 88f), new Vector2(560f, 46f), 32, Color.white);
        educationalLabel = MakeLabel("EducationalLabel", ct, "",
            new Vector2(0f, 38f), new Vector2(640f, 44f), 18, new Color(0.94f, 0.84f, 0.62f, 1f));
        bestCropLabel = MakeLabel("BestCropLabel", ct, "",
            new Vector2(0f, -8f), new Vector2(520f, 34f), 20, new Color(0.84f, 0.82f, 0.76f, 1f));

        chartContainer = MakeRect("ChartContainer", ct, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, -116f), new Vector2(540f, 140f));
        chartContainer.gameObject.AddComponent<Image>().color = new Color(0.07f, 0.055f, 0.045f, 1f);

        // A05.8: botão dominante, label maior e centrado.
        playAgainButton = MakeButton("PlayAgainButton", "JOGAR DE NOVO", ct,
            new Vector2(0f, -250f), new Vector2(360f, 78f));
        var pbLabel = playAgainButton.GetComponentInChildren<TMP_Text>();
        if (pbLabel != null)
        {
            pbLabel.fontSize = 26;
            pbLabel.fontStyle = FontStyles.Bold;
        }

        barPrefab = new GameObject("RuntimeChartBarPrefab");
        barPrefab.transform.SetParent(transform, false);
        barPrefab.AddComponent<RectTransform>();
        barPrefab.AddComponent<Image>();
        barPrefab.SetActive(false);

        WirePlayAgainButton();
    }

    void TryFindSceneReferences()
    {
        if (finalBalanceLabel == null) finalBalanceLabel = FindText("FinalBalanceLabel");
        if (outcomeLabel == null) outcomeLabel = FindText("OutcomeLabel");
        if (educationalLabel == null) educationalLabel = FindText("EducationalLabel");
        if (bestCropLabel == null) bestCropLabel = FindText("BestCropLabel");
        if (chartContainer == null) chartContainer = FindRect("ChartContainer");
        if (playAgainButton == null)
        {
            GameObject buttonGO = GameObject.Find("PlayAgainButton");
            if (buttonGO != null) playAgainButton = buttonGO.GetComponent<Button>();
        }
    }

    void WirePlayAgainButton()
    {
        if (playAgainButton == null) return;
        playAgainButton.onClick.RemoveListener(OnPlayAgain);
        playAgainButton.onClick.AddListener(OnPlayAgain);
    }

    static TMP_Text FindText(string name)
    {
        GameObject go = GameObject.Find(name);
        return go != null ? go.GetComponent<TMP_Text>() : null;
    }

    static RectTransform FindRect(string name)
    {
        GameObject go = GameObject.Find(name);
        return go != null ? go.GetComponent<RectTransform>() : null;
    }

    static RectTransform MakeRect(string name, Transform parent, Vector2 anchorMin,
        Vector2 anchorMax, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return rt;
    }

    void PolishEndScreenStyle()
    {
        var bg = GameObject.Find("Background");
        if (bg != null)
        {
            var img = bg.GetComponent<Image>();
            if (img != null) img.color = new Color(0.10f, 0.07f, 0.05f, 1f);
        }

        if (finalBalanceLabel != null)
        {
            EnsureCardBehind(finalBalanceLabel.transform.parent);
            StyleLabel(GameObject.Find("TitleLabel")?.GetComponent<TMP_Text>(),
                new Color(0.98f, 0.84f, 0.45f, 1f), 44, FontStyles.Bold);
            StyleLabel(finalBalanceLabel, new Color(0.96f, 0.92f, 0.78f, 1f), 28, FontStyles.Normal);
            StyleLabel(outcomeLabel, new Color(0.96f, 0.88f, 0.62f, 1f), 30, FontStyles.Bold);
            StyleLabel(educationalLabel, new Color(0.92f, 0.84f, 0.68f, 1f), 18, FontStyles.Normal);
            StyleLabel(bestCropLabel, new Color(0.88f, 0.80f, 0.62f, 1f), 20, FontStyles.Normal);
            StyleLabel(GameObject.Find("ChartLabel")?.GetComponent<TMP_Text>(),
                new Color(0.78f, 0.70f, 0.52f, 1f), 16, FontStyles.Italic);
        }

        if (chartContainer != null)
        {
            var img = chartContainer.GetComponent<Image>();
            if (img != null) img.color = new Color(0.16f, 0.11f, 0.08f, 1f);
            EnsureChartEmptyHint();
        }
    }

    static void StyleLabel(TMP_Text label, Color color, int fontSize, FontStyles style)
    {
        if (label == null) return;
        label.color = color;
        label.fontSize = fontSize;
        label.fontStyle = style;
    }

    void EnsureCardBehind(Transform sibling)
    {
        if (sibling == null) return;
        if (sibling.Find("EndCard") != null) return;
        var card = MakeRect("EndCard", sibling, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(780f, 560f));
        var img = card.gameObject.AddComponent<Image>();
        img.color = new Color(0.16f, 0.11f, 0.07f, 0.94f);
        img.raycastTarget = false;
        card.SetAsFirstSibling();
        var bg = sibling.Find("Background");
        if (bg != null) bg.SetAsFirstSibling();
    }

    GameObject _chartEmptyHint;
    void EnsureChartEmptyHint()
    {
        bool hasData = GameManager.Instance != null
            && GameManager.Instance.DailyBalances != null
            && GameManager.Instance.DailyBalances.Count > 0;
        if (_chartEmptyHint == null && !hasData)
        {
            _chartEmptyHint = MakeLabel("ChartEmptyHint", chartContainer,
                "Ainda não há dias terminados para mostrar.",
                Vector2.zero, new Vector2(480f, 60f), 16,
                new Color(0.70f, 0.62f, 0.48f, 1f)).gameObject;
        }
        if (_chartEmptyHint != null) _chartEmptyHint.SetActive(!hasData);
    }

    static RectTransform MakePanel(string name, Transform parent, Vector2 anchorMin,
        Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, Color color)
    {
        RectTransform rt = MakeRect(name, parent, anchorMin, anchorMax, anchoredPos, size);
        rt.gameObject.AddComponent<Image>().color = color;
        return rt;
    }

    static TMP_Text MakeLabel(string name, Transform parent, string text,
        Vector2 anchoredPos, Vector2 size, int fontSize, Color color)
    {
        RectTransform rt = MakeRect(name, parent, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), anchoredPos, size);
        var label = rt.gameObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.color = color;
        label.raycastTarget = false;

        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) label.font = font;

        return label;
    }

    static Button MakeButton(string name, string labelText, Transform parent,
        Vector2 anchoredPos, Vector2 size)
    {
        RectTransform rt = MakeRect(name, parent, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), anchoredPos, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.color = new Color(0.88f, 0.62f, 0.22f, 1f);
        var button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        MakeLabel(name + "_Label", rt, labelText, Vector2.zero, size, 18,
            new Color(0.10f, 0.07f, 0.04f, 1f));
        return button;
    }
}
