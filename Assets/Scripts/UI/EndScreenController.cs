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
    [SerializeField] TMP_Text educationalLabel;
    [SerializeField] TMP_Text bestCropLabel;

    [Header("Balance Chart")]
    [SerializeField] RectTransform chartContainer;
    [SerializeField] GameObject barPrefab;

    [Header("Actions")]
    [SerializeField] Button playAgainButton;

    readonly Dictionary<string, float> _revenuePerCrop = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgain);

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            BuildEndScreen();
    }

    public void RecordSale(string cropName, float revenue)
    {
        _revenuePerCrop.TryGetValue(cropName, out float current);
        _revenuePerCrop[cropName] = current + revenue;
    }

    void BuildEndScreen()
    {
        float final = GameManager.Instance.Balance;
        finalBalanceLabel.text = $"Saldo final: R${final:F2}";

        if (GameManager.Instance.IsLoss)
        {
            outcomeLabel.text = "Falência! O saldo chegou a zero.";
            outcomeLabel.color = new Color(0.9f, 0.2f, 0.2f);
            SetEducationalText("Dica: venda sempre acima do custo da semente para não ter prejuízo.");
        }
        else if (final > GameManager.StartingBalance)
        {
            outcomeLabel.text = "Você lucrou! Ótima gestão.";
            outcomeLabel.color = new Color(0.2f, 0.8f, 0.2f);
            SetEducationalText("Você dominou a precificação! Margem = Preço − Custo. É assim na roça de verdade.");
        }
        else
        {
            outcomeLabel.text = "Você sobreviveu! Dá pra melhorar.";
            outcomeLabel.color = new Color(0.9f, 0.8f, 0.2f);
            SetEducationalText("Dica: Açaí tem margem de R$18 por unidade. Vale imobilizar capital por 6 dias?");
        }

        string best = GetBestCrop();
        bestCropLabel.text = best != null ? $"Cultura mais rentável: {best}" : "";

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
        if (balances.Count == 0) return;

        float max = float.MinValue, min = float.MaxValue;
        foreach (float v in balances) { if (v > max) max = v; if (v < min) min = v; }
        float range = Mathf.Max(max - min, 1f);

        float containerHeight = chartContainer.rect.height;
        float barWidth = chartContainer.rect.width / balances.Count;

        for (int i = 0; i < balances.Count; i++)
        {
            GameObject bar = Instantiate(barPrefab, chartContainer);
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
        SaveSystem.Delete();
        UnityEngine.SceneManagement.SceneManager.LoadScene("FarmScene");
    }
}
