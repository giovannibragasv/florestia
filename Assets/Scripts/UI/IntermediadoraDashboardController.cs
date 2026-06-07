using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntermediadoraDashboardController : MonoBehaviour
{
    public static IntermediadoraDashboardController Instance { get; private set; }

    GameObject _root;
    TMP_Text _studentLabel;
    TMP_Text _dayLabel;
    TMP_Text _moneyLabel;
    TMP_Text _bestCropLabel;
    TMP_Text _accuracyLabel;
    RectTransform _chartContainer;
    Button _closeButton;
    Button _exportCsvButton;
    TMP_Text _exportFeedbackLabel;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
            Toggle();
    }

    void Toggle()
    {
        BuildUI();
        if (_root == null) return;

        bool show = !_root.activeSelf;
        _root.SetActive(show);
        if (show) Refresh();
    }

    void BuildUI()
    {
        if (_root != null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        _root = new GameObject("IntermediadoraDashboard");
        _root.transform.SetParent(canvas.transform, false);
        var rt = _root.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        _root.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.72f);

        var panel = Rect("Panel", _root.transform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 560f));
        panel.gameObject.AddComponent<Image>().color = new Color(0.11f, 0.09f, 0.075f, 0.98f);

        var title = Label("Title", panel.transform, "Visão da intermediadora",
            new Vector2(0f, 230f), new Vector2(800f, 46f), 30, new Color(0.98f, 0.84f, 0.45f));
        title.fontStyle = FontStyles.Bold;

        Label("Subtitle", panel.transform, "Resumo visual para acompanhar o aluno durante o protótipo",
            new Vector2(0f, 196f), new Vector2(800f, 28f), 16, new Color(0.78f, 0.72f, 0.58f));

        _studentLabel = Metric(panel.transform, "Student", "Aluno", new Vector2(-300f, 116f));
        _dayLabel = Metric(panel.transform, "Day", "Dia", new Vector2(-100f, 116f));
        _moneyLabel = Metric(panel.transform, "Money", "Dinheiro", new Vector2(100f, 116f));
        _accuracyLabel = Metric(panel.transform, "Accuracy", "Perguntas", new Vector2(300f, 116f));
        _bestCropLabel = Label("BestCrop", panel.transform, "O que mais rendeu: -",
            new Vector2(0f, 42f), new Vector2(760f, 34f), 20, new Color(0.95f, 0.92f, 0.84f));

        _chartContainer = Rect("BalanceChart", panel.transform, new Vector2(0.5f, 0.5f),
            new Vector2(0f, -94f), new Vector2(760f, 190f));
        _chartContainer.gameObject.AddComponent<Image>().color = new Color(0.07f, 0.055f, 0.045f, 1f);
        Label("ChartTitle", panel.transform, "Dinheiro em cada dia",
            new Vector2(0f, 18f), new Vector2(760f, 24f), 16, new Color(0.78f, 0.72f, 0.58f));

        _closeButton = ButtonAt("CloseButton", panel.transform, new Vector2(140f, -246f), new Vector2(180f, 54f), "Fechar");
        _closeButton.onClick.AddListener(() => _root.SetActive(false));

        _exportCsvButton = ButtonAt("ExportCsvButton", panel.transform, new Vector2(-140f, -246f), new Vector2(220f, 54f), "Exportar CSV");
        _exportCsvButton.onClick.AddListener(ExportCsvAndConfirm);
        _exportFeedbackLabel = Label("ExportFeedback", panel.transform, "",
            new Vector2(0f, -198f), new Vector2(760f, 22f), 14, new Color(0.85f, 0.94f, 0.78f));

        _root.SetActive(false);
    }

    void ExportCsvAndConfirm()
    {
        string path = DashboardExporter.WriteCsvToFile();
        if (_exportFeedbackLabel != null)
            _exportFeedbackLabel.text = $"CSV salvo em: {path}";
        Debug.Log($"Florestia Dashboard CSV exportado para: {path}");
    }

    TMP_Text Metric(Transform parent, string name, string title, Vector2 anchoredPos)
    {
        var box = Rect(name, parent, new Vector2(0.5f, 0.5f), anchoredPos, new Vector2(170f, 86f));
        box.gameObject.AddComponent<Image>().color = new Color(0.18f, 0.13f, 0.09f, 1f);
        Label("Title", box.transform, title, new Vector2(0f, 22f), new Vector2(150f, 22f), 14,
            new Color(0.78f, 0.72f, 0.58f));
        return Label("Value", box.transform, "-", new Vector2(0f, -12f), new Vector2(150f, 34f), 22,
            new Color(0.98f, 0.84f, 0.45f));
    }

    void Refresh()
    {
        var gm = GameManager.Instance;
        string student = gm?.ActiveProfile != null ? gm.ActiveProfile.name : "Aluno teste";
        int day = gm != null ? gm.CurrentDay : 1;
        float money = gm != null ? gm.Balance : GameManager.StartingBalance;

        _studentLabel.text = student;
        _dayLabel.text = $"{day}/{GameManager.TotalDays}";
        _moneyLabel.text = $"F${money:F0}";
        _accuracyLabel.text = BuildAccuracy(gm);
        _bestCropLabel.text = $"O que mais rendeu: {BestCrop(gm)}";
        BuildChart(gm);
    }

    static string BuildAccuracy(GameManager gm)
    {
        if (gm == null || gm.Questions == null || gm.Questions.Count == 0) return "0%";
        int correct = 0;
        foreach (var q in gm.Questions)
            if (q.correct) correct++;
        return $"{Mathf.RoundToInt((float)correct / gm.Questions.Count * 100f)}%";
    }

    static string BestCrop(GameManager gm)
    {
        if (gm == null || gm.Sales == null || gm.Sales.Count == 0) return "-";
        float mandioca = 0f, cacau = 0f, acai = 0f;
        foreach (var sale in gm.Sales)
        {
            if (sale.cropName == "Mandioca") mandioca += sale.total;
            else if (sale.cropName == "Cacau") cacau += sale.total;
            else if (sale.cropName == "Acai") acai += sale.total;
        }
        if (mandioca >= cacau && mandioca >= acai) return "Mandioca";
        if (cacau >= acai) return "Cacau";
        return "Açaí";
    }

    void BuildChart(GameManager gm)
    {
        for (int i = _chartContainer.childCount - 1; i >= 0; i--)
            Destroy(_chartContainer.GetChild(i).gameObject);

        int count = gm != null && gm.DailyBalances != null && gm.DailyBalances.Count > 0
            ? gm.DailyBalances.Count
            : 5;
        float max = GameManager.StartingBalance;
        for (int i = 0; gm != null && gm.DailyBalances != null && i < gm.DailyBalances.Count; i++)
            if (gm.DailyBalances[i] > max) max = gm.DailyBalances[i];

        float width = _chartContainer.rect.width / Mathf.Max(count, 1);
        for (int i = 0; i < count; i++)
        {
            float value = gm != null && gm.DailyBalances != null && i < gm.DailyBalances.Count
                ? gm.DailyBalances[i]
                : GameManager.StartingBalance - i * 3f;
            float height = Mathf.Clamp01(value / Mathf.Max(max, 1f)) * (_chartContainer.rect.height - 28f);

            var bar = Rect($"Bar_{i + 1}", _chartContainer, new Vector2(0f, 0f),
                new Vector2(8f + i * width, 18f), new Vector2(width - 8f, height));
            bar.anchorMin = new Vector2(0f, 0f);
            bar.anchorMax = new Vector2(0f, 0f);
            bar.pivot = new Vector2(0f, 0f);
            bar.gameObject.AddComponent<Image>().color = value >= 0f
                ? new Color(0.32f, 0.78f, 0.42f, 1f)
                : new Color(0.92f, 0.38f, 0.34f, 1f);

            Label($"Day_{i + 1}", _chartContainer, (i + 1).ToString(),
                new Vector2(8f + i * width + width * 0.5f, 6f), new Vector2(width, 16f), 11,
                new Color(0.78f, 0.72f, 0.58f));
        }
    }

    static RectTransform Rect(string name, Transform parent, Vector2 pivot, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return rt;
    }

    static TMP_Text Label(string name, Transform parent, string text, Vector2 anchoredPos,
        Vector2 size, int fontSize, Color color)
    {
        var rt = Rect(name, parent, new Vector2(0.5f, 0.5f), anchoredPos, size);
        var label = rt.gameObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.raycastTarget = false;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) label.font = font;
        return label;
    }

    static Button ButtonAt(string name, Transform parent, Vector2 anchoredPos, Vector2 size, string text)
    {
        var rt = Rect(name, parent, new Vector2(0.5f, 0.5f), anchoredPos, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.color = new Color(0.88f, 0.62f, 0.22f, 1f);
        var button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var label = Label("Label", rt, text, Vector2.zero, size, 20,
            new Color(0.10f, 0.07f, 0.04f, 1f));
        label.fontStyle = FontStyles.Bold;
        return button;
    }
}
