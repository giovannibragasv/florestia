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

        float final = GameManager.Instance.Balance;
        // Vocabulário 8-11 (Modelo C §3.4): "dinheiro no fim" no lugar de "saldo final".
        finalBalanceLabel.text = $"Dinheiro no fim: R${final:F2}";

        if (GameManager.Instance.IsLoss)
        {
            outcomeLabel.text = "Acabou o dinheiro. Tenta de novo!";
            outcomeLabel.color = new Color(0.9f, 0.42f, 0.32f);
            SetEducationalText("Pra não acabar o dinheiro, venda por um preço maior do que pagou na semente.");
        }
        else if (final > GameManager.StartingBalance)
        {
            outcomeLabel.text = "Você plantou, vendeu e sobrou dinheiro!";
            outcomeLabel.color = new Color(0.32f, 0.80f, 0.42f);
            SetEducationalText("Sobra = quanto recebeu − quanto gastou. Você fez essa conta direitinho!");
        }
        else
        {
            outcomeLabel.text = "Você terminou os 15 dias!";
            outcomeLabel.color = new Color(0.95f, 0.84f, 0.35f);
            SetEducationalText("O Açaí demora 6 dias mas paga bem. Tente plantar mais açaí na próxima vez!");
        }

        string best = GetBestCrop();
        bestCropLabel.text = best != null ? $"O que mais te rendeu: {best}" : "";

        BuildBalanceChart();
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

        float max = float.MinValue, min = float.MaxValue;
        foreach (float v in balances) { if (v > max) max = v; if (v < min) min = v; }
        float range = Mathf.Max(max - min, 1f);

        float containerHeight = chartContainer.rect.height;
        float barWidth = chartContainer.rect.width / balances.Count;

        for (int i = 0; i < balances.Count; i++)
        {
            GameObject bar = Instantiate(barPrefab, chartContainer);
            bar.SetActive(true);
            RectTransform rt = bar.GetComponent<RectTransform>();
            float normalizedHeight = (balances[i] - min) / range;
            rt.sizeDelta = new Vector2(barWidth - 2f, normalizedHeight * containerHeight);
            rt.anchoredPosition = new Vector2(i * barWidth, 0);

            Image img = bar.GetComponent<Image>();
            img.color = balances[i] >= 0
                ? new Color(0.2f, 0.75f, 0.4f)
                : new Color(0.85f, 0.25f, 0.25f);
        }
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

        finalBalanceLabel = MakeLabel("FinalBalanceLabel", ct, "Dinheiro no fim: R$0,00",
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

        playAgainButton = MakeButton("PlayAgainButton", "Jogar novamente", ct,
            new Vector2(0f, -224f), new Vector2(210f, 52f));

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
