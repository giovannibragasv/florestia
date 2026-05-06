using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour
{
    public static EndScreenController Instance { get; private set; }

    [Header("Summary")]
    [SerializeField] TMP_Text finalBalanceLabel;
    [SerializeField] TMP_Text outcomeLabel;
    [SerializeField] TMP_Text bestCropLabel;

    [Header("Balance Chart")]
    [SerializeField] RectTransform chartContainer;
    [SerializeField] GameObject barPrefab;

    [Header("Actions")]
    [SerializeField] Button playAgainButton;

    readonly List<float> _balanceHistory = new();
    readonly Dictionary<string, float> _revenuePerCrop = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playAgainButton.onClick.AddListener(OnPlayAgain);
        if (GameManager.Instance.IsGameOver) BuildEndScreen();
    }

    public void RecordDayBalance(float balance) => _balanceHistory.Add(balance);

    public void RecordSale(string cropName, float revenue)
    {
        _revenuePerCrop.TryGetValue(cropName, out float current);
        _revenuePerCrop[cropName] = current + revenue;
    }

    public float[] GetBalanceHistory() => _balanceHistory.ToArray();

    void BuildEndScreen()
    {
        float final = GameManager.Instance.Balance;
        finalBalanceLabel.text = $"Saldo final: R${final:F2}";
        outcomeLabel.text = final >= 0 ? "Você sobreviveu!" : "Falência.";
        outcomeLabel.color = final >= 0 ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.9f, 0.2f, 0.2f);

        string best = GetBestCrop();
        bestCropLabel.text = best != null ? $"Cultura mais rentável: {best}" : "";

        BuildBalanceChart();
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
        if (_balanceHistory.Count == 0) return;

        float max = float.MinValue, min = float.MaxValue;
        foreach (float v in _balanceHistory) { if (v > max) max = v; if (v < min) min = v; }
        float range = Mathf.Max(max - min, 1f);

        float containerHeight = chartContainer.rect.height;
        float barWidth = chartContainer.rect.width / _balanceHistory.Count;

        for (int i = 0; i < _balanceHistory.Count; i++)
        {
            GameObject bar = Instantiate(barPrefab, chartContainer);
            RectTransform rt = bar.GetComponent<RectTransform>();
            float normalizedHeight = (_balanceHistory[i] - min) / range;
            rt.sizeDelta = new Vector2(barWidth - 2f, normalizedHeight * containerHeight);
            rt.anchoredPosition = new Vector2(i * barWidth, 0);

            Image img = bar.GetComponent<Image>();
            img.color = _balanceHistory[i] >= 0
                ? new Color(0.2f, 0.75f, 0.4f)
                : new Color(0.85f, 0.25f, 0.25f);
        }
    }

    void OnPlayAgain()
    {
        _balanceHistory.Clear();
        _revenuePerCrop.Clear();
        SaveSystem.Delete();
        UnityEngine.SceneManagement.SceneManager.LoadScene("FarmScene");
    }
}
