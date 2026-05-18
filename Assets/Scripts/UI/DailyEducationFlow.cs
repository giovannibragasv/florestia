using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Fluxo educacional do fim do dia (Modelo C §3.3):
///   Resumo → Pergunta de Custo → Pergunta de Receita → Pergunta de Sobra →
///   Curiosidade cultural → AdvanceDay.
///
/// As perguntas são geradas a partir do que o aluno fez no dia (plantings/sales
/// persistidos via B01). Versão MVP é hardcoded; a versão adaptativa fica em FA02.
/// </summary>
public class DailyEducationFlow : MonoBehaviour
{
    public static DailyEducationFlow Instance { get; private set; }

    GameObject _root;
    TMP_Text _stepLabel;
    TMP_Text _bodyLabel;
    Button[] _optionButtons;
    TMP_Text[] _optionLabels;
    TMP_Text _feedbackLabel;
    Button _continueButton;
    TMP_Text _continueLabel;

    List<DailyQuestion> _questions;
    DailyCuriosity _curiosity;
    string _reviewText;
    int _stepIndex;
    bool _showingCuriosity;
    bool _showingReview;
    bool _hasAnswered;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void StartFlow()
    {
        BuildUI();
        var gm = GameManager.Instance;
        if (_root == null || gm == null) { Finish(); return; }
        _reviewText = DailyEducationGenerator.BuildReviewIfNoActivityToday(gm);
        _questions = string.IsNullOrEmpty(_reviewText)
            ? DailyEducationGenerator.GenerateQuestions(gm)
            : new List<DailyQuestion>();
        _curiosity = string.IsNullOrEmpty(_reviewText)
            ? DailyEducationGenerator.PickCuriosity(gm)
            : null;
        _stepIndex = 0;
        _showingCuriosity = false;
        _showingReview = false;
        _hasAnswered = false;
        _root.SetActive(true);
        ShowCurrent();
    }

    void ShowCurrent()
    {
        _hasAnswered = false;
        if (_feedbackLabel != null) _feedbackLabel.text = "";
        if (_continueButton != null) _continueButton.gameObject.SetActive(false);

        if (_stepIndex < _questions.Count)
        {
            var q = _questions[_stepIndex];
            _showingCuriosity = false;
            _showingReview = false;
            if (_stepLabel != null) _stepLabel.text = $"Pergunta {_stepIndex + 1} de {_questions.Count + 1} · {q.theme}";
            if (_bodyLabel != null) _bodyLabel.text = q.statement;
            ShowOptions(q);
        }
        else if (!_showingReview && !string.IsNullOrEmpty(_reviewText))
        {
            _showingReview = true;
            if (_stepLabel != null) _stepLabel.text = "Revisão do dia";
            if (_bodyLabel != null) _bodyLabel.text = _reviewText;
            ShowOptions(null);
            if (_continueLabel != null) _continueLabel.text = "Próximo dia";
            if (_continueButton != null) _continueButton.gameObject.SetActive(true);
        }
        else if (!_showingCuriosity && _curiosity != null)
        {
            _showingCuriosity = true;
            if (_stepLabel != null) _stepLabel.text = "Curiosidade · " + _curiosity.cropName;
            if (_bodyLabel != null) _bodyLabel.text = _curiosity.text;
            ShowOptions(null);
            if (_continueLabel != null) _continueLabel.text = "Próximo dia";
            if (_continueButton != null) _continueButton.gameObject.SetActive(true);
        }
        else
        {
            Finish();
        }
    }

    void ShowOptions(DailyQuestion q)
    {
        if (_optionButtons == null) return;
        for (int i = 0; i < _optionButtons.Length; i++)
        {
            if (_optionButtons[i] == null) continue;
            bool active = q != null && q.options != null && i < q.options.Length;
            _optionButtons[i].gameObject.SetActive(active);
            if (!active) continue;
            if (_optionLabels != null && _optionLabels[i] != null) _optionLabels[i].text = q.options[i];
            ResetOptionColor(_optionButtons[i]);
            int idx = i;
            _optionButtons[i].onClick.RemoveAllListeners();
            _optionButtons[i].onClick.AddListener(() => OnOptionClicked(idx));
        }
    }

    void OnOptionClicked(int idx)
    {
        if (_hasAnswered) return;
        var q = _questions[_stepIndex];
        bool correct = idx == q.correctIndex;
        _hasAnswered = true;

        HighlightOption(idx, correct);
        if (!correct) HighlightOption(q.correctIndex, true);

        if (_feedbackLabel != null)
        {
            _feedbackLabel.text = correct
                ? $"Isso aí! {q.explanation}"
                : $"A resposta era: {q.options[q.correctIndex]}. {q.explanation}";
            _feedbackLabel.color = correct
                ? new Color(0.32f, 0.80f, 0.42f)
                : new Color(0.92f, 0.55f, 0.45f);
        }

        GameManager.Instance?.RecordQuestionAnswer(q.cropName, q.questionId, correct);

        if (_continueLabel != null)
            _continueLabel.text = (_stepIndex >= _questions.Count - 1) ? "Curiosidade" : "Próxima pergunta";
        if (_continueButton != null) _continueButton.gameObject.SetActive(true);
    }

    void OnContinueClicked()
    {
        if (_showingReview) { Finish(); return; }
        if (_showingCuriosity) { Finish(); return; }
        _stepIndex++;
        ShowCurrent();
    }

    void Finish()
    {
        if (_root != null) _root.SetActive(false);
        EndScreenController.Instance?.ClearCurrentDaySales();
        GameManager.Instance?.AdvanceDay();
    }

    void HighlightOption(int idx, bool correct)
    {
        if (idx < 0 || idx >= _optionButtons.Length) return;
        var img = _optionButtons[idx].targetGraphic as Image;
        if (img == null) return;
        img.color = correct
            ? new Color(0.32f, 0.78f, 0.42f, 1f)
            : new Color(0.92f, 0.38f, 0.34f, 1f);
    }

    static void ResetOptionColor(Button b)
    {
        if (b.targetGraphic is Image img)
            img.color = new Color(0.30f, 0.22f, 0.14f, 1f);
    }

    // ---------- Runtime UI fallback ----------

    void BuildUI()
    {
        if (_root != null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        _root = new GameObject("DailyEducationFlow_UI");
        _root.transform.SetParent(canvas.transform, false);
        var rt = _root.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        var dim = _root.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.80f);

        var card = Rect("Card", _root.transform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 540f));
        card.gameObject.AddComponent<Image>().color = new Color(0.14f, 0.10f, 0.07f, 1f);

        _stepLabel = Label("StepLabel", card, "Pergunta 1 de 4", new Vector2(0f, 220f),
            new Vector2(640f, 28f), 16, new Color(0.78f, 0.72f, 0.58f), TextAlignmentOptions.Center);

        _bodyLabel = Label("BodyLabel", card, "—", new Vector2(0f, 130f),
            new Vector2(640f, 130f), 22, new Color(0.96f, 0.92f, 0.84f), TextAlignmentOptions.Center);
        _bodyLabel.textWrappingMode = TextWrappingModes.Normal;

        _optionButtons = new Button[3];
        _optionLabels = new TMP_Text[3];
        float[] xs = { -210f, 0f, 210f };
        for (int i = 0; i < 3; i++)
        {
            var btn = MakeOptionButton(card, $"OptionButton_{i}", new Vector2(xs[i], 0f));
            _optionButtons[i] = btn;
            _optionLabels[i] = btn.GetComponentInChildren<TMP_Text>();
        }

        _feedbackLabel = Label("FeedbackLabel", card, "", new Vector2(0f, -120f),
            new Vector2(640f, 60f), 16, new Color(0.96f, 0.88f, 0.68f), TextAlignmentOptions.Center);
        _feedbackLabel.textWrappingMode = TextWrappingModes.Normal;

        _continueButton = ButtonAt("ContinueButton", card, new Vector2(0f, -210f),
            new Vector2(280f, 60f), "Próxima pergunta", new Color(0.88f, 0.62f, 0.22f, 1f),
            new Color(0.10f, 0.07f, 0.04f, 1f));
        _continueLabel = _continueButton.GetComponentInChildren<TMP_Text>();
        _continueButton.onClick.AddListener(OnContinueClicked);
        _continueButton.gameObject.SetActive(false);

        _root.SetActive(false);
    }

    Button MakeOptionButton(Transform parent, string name, Vector2 anchored)
    {
        return ButtonAt(name, parent, anchored, new Vector2(190f, 80f),
            "—", new Color(0.30f, 0.22f, 0.14f, 1f),
            new Color(0.96f, 0.88f, 0.68f, 1f));
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
        Vector2 size, int fontSize, Color color, TextAlignmentOptions align)
    {
        var rt = Rect(name, parent, new Vector2(0.5f, 0.5f), anchoredPos, size);
        var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = fontSize;
        t.color = color;
        t.alignment = align;
        t.raycastTarget = false;
        var font = FlorestiaFontUtility.LoadPreferredTMPFont();
        if (font != null) t.font = font;
        return t;
    }

    static Button ButtonAt(string name, Transform parent, Vector2 anchoredPos, Vector2 size,
        string label, Color bg, Color labelColor)
    {
        var rt = Rect(name, parent, new Vector2(0.5f, 0.5f), anchoredPos, size);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = bg;
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        var lbl = Label("Label", rt, label, Vector2.zero, size, 18, labelColor, TextAlignmentOptions.Center);
        lbl.textWrappingMode = TextWrappingModes.Normal;
        return btn;
    }
}

public class DailyQuestion
{
    public string questionId;
    public string cropName;
    public string theme;
    public string statement;
    public string[] options;
    public int correctIndex;
    public string explanation;
}

public class DailyCuriosity
{
    public string cropName;
    public string text;
}

/// <summary>
/// Gera 3 perguntas matemáticas (custo/receita/sobra) + 1 curiosidade cultural
/// a partir do que o aluno fez no dia. Versão hardcoded; adaptativa em FA02.
/// </summary>
public static class DailyEducationGenerator
{
    static readonly Dictionary<string, DailyQuestion[]> HardcodedQuestions = new()
    {
        ["Mandioca"] = new[]
        {
            Q("mandioca-custo-2", "Mandioca", "Custo",
                "Duas mudas de mandioca custam R$3 cada. Quanto você gasta para plantar as duas?",
                new[] { "R$3,00", "R$6,00", "R$9,00" }, 1,
                "R$3,00 + R$3,00 = R$6,00."),
            Q("mandioca-recebe-3", "Mandioca", "Quanto você recebeu",
                "Você vendeu 3 mandiocas por R$7 cada. Quanto dinheiro recebeu?",
                new[] { "R$14,00", "R$21,00", "R$24,00" }, 1,
                "3 × R$7,00 = R$21,00."),
            Q("mandioca-sobra-1", "Mandioca", "Sobra",
                "Uma mandioca custou R$3 para plantar e foi vendida por R$7. Quanto sobrou?",
                new[] { "R$3,00", "R$4,00", "R$7,00" }, 1,
                "R$7,00 − R$3,00 = R$4,00."),
            Q("mandioca-tempo", "Mandioca", "Tempo",
                "A mandioca demora 2 dias para colher. Se você planta hoje, quantos dias precisa cuidar dela?",
                new[] { "1 dia", "2 dias", "4 dias" }, 1,
                "A mandioca fica pronta depois de 2 dias de cuidado."),
            Q("mandioca-compara", "Mandioca", "Comparar",
                "Você tem R$9. Cada mandioca custa R$3 para plantar. Quantas mandiocas dá para plantar?",
                new[] { "2", "3", "4" }, 1,
                "R$9,00 dividido por R$3,00 dá 3 mandiocas.")
        },
        ["Cacau"] = new[]
        {
            Q("cacau-custo-2", "Cacau", "Custo",
                "Duas sementes de cacau custam R$6 cada. Quanto você gasta?",
                new[] { "R$6,00", "R$12,00", "R$18,00" }, 1,
                "R$6,00 + R$6,00 = R$12,00."),
            Q("cacau-recebe-2", "Cacau", "Quanto você recebeu",
                "Você vendeu 2 cacaus por R$16 cada. Quanto recebeu?",
                new[] { "R$22,00", "R$32,00", "R$36,00" }, 1,
                "2 × R$16,00 = R$32,00."),
            Q("cacau-sobra-1", "Cacau", "Sobra",
                "Um cacau custou R$6 para plantar e foi vendido por R$16. Quanto sobrou?",
                new[] { "R$6,00", "R$10,00", "R$16,00" }, 1,
                "R$16,00 − R$6,00 = R$10,00."),
            Q("cacau-tempo", "Cacau", "Tempo",
                "O cacau demora 4 dias para colher. Ele demora mais que a mandioca de 2 dias por quantos dias?",
                new[] { "1 dia", "2 dias", "4 dias" }, 1,
                "4 dias − 2 dias = 2 dias a mais."),
            Q("cacau-compara", "Cacau", "Comparar",
                "Você tem R$18. Cada cacau custa R$6 para plantar. Quantos cacaus dá para plantar?",
                new[] { "2", "3", "4" }, 1,
                "R$18,00 dividido por R$6,00 dá 3 cacaus.")
        },
        ["Acai"] = new[]
        {
            Q("acai-custo-2", "Acai", "Custo",
                "Dois açaizeiros custam R$10 cada para plantar. Quanto você gasta?",
                new[] { "R$10,00", "R$20,00", "R$28,00" }, 1,
                "R$10,00 + R$10,00 = R$20,00."),
            Q("acai-recebe-2", "Acai", "Quanto você recebeu",
                "Você vendeu 2 açaís por R$28 cada. Quanto recebeu?",
                new[] { "R$38,00", "R$46,00", "R$56,00" }, 2,
                "2 × R$28,00 = R$56,00."),
            Q("acai-sobra-1", "Acai", "Sobra",
                "Um açaí custou R$10 para plantar e foi vendido por R$28. Quanto sobrou?",
                new[] { "R$10,00", "R$18,00", "R$28,00" }, 1,
                "R$28,00 − R$10,00 = R$18,00."),
            Q("acai-tempo", "Acai", "Tempo",
                "O açaí demora 6 dias para colher. Ele demora mais que o cacau de 4 dias por quantos dias?",
                new[] { "2 dias", "4 dias", "6 dias" }, 0,
                "6 dias − 4 dias = 2 dias a mais."),
            Q("acai-compara", "Acai", "Comparar",
                "Você tem R$30. Cada açaí custa R$10 para plantar. Quantos açaís dá para plantar?",
                new[] { "2", "3", "4" }, 1,
                "R$30,00 dividido por R$10,00 dá 3 açaís.")
        }
    };

    static readonly Dictionary<string, string[]> Curiosities = new()
    {
        ["Mandioca"] = new[]
        {
            "A mandioca é uma raiz que veio dos povos indígenas da Amazônia e virou base da farinha do nosso prato.",
            "Da mandioca a gente faz farinha, tapioca, tucupi e bolo. Uma só planta, muitas receitas.",
            "A mandioca cresce até em terra mais seca — é por isso que ela aguenta firme no quintal da Vila Jutaí."
        },
        ["Cacau"] = new[]
        {
            "O cacau é uma fruta brasileira. Sua semente vira chocolate depois de seca e torrada.",
            "Antes do chocolate ser doce, os povos da Amazônia tomavam o cacau como uma bebida amarga e forte.",
            "Cada fruto de cacau tem por volta de 30 sementes — cada uma delas vira um pedacinho de chocolate."
        },
        ["Acai"] = new[]
        {
            "O açaí cresce em palmeiras altas. Os ribeirinhos sobem nelas pra colher os cachos.",
            "Na Vila Jutaí, o açaí é tomado com farinha e peixe — não só como sobremesa.",
            "Cada açaizeiro pode dar de 3 a 6 cachos por ano, e cada cacho tem centenas de frutinhos."
        }
    };

    public static List<DailyQuestion> GenerateQuestions(GameManager gm)
    {
        int day = gm.CurrentDay;
        var todayPlantings = gm.Plantings.FindAll(p => p.day == day);
        var todaySales = gm.Sales.FindAll(s => s.day == day);

        string focusCrop = ChooseFocusCrop(todayPlantings, todaySales);
        if (string.IsNullOrEmpty(focusCrop)) focusCrop = "Mandioca";
        var bankQuestions = PickQuestionsFromBank(focusCrop, day);
        if (bankQuestions.Count > 0) return bankQuestions;

        int qty;
        float pricePerUnit;
        float seedCost;

        if (todaySales.Count > 0)
        {
            var firstSale = todaySales[0];
            focusCrop = firstSale.cropName;
            qty = firstSale.quantity;
            pricePerUnit = firstSale.pricePerUnit;
            seedCost = PricingSystem.Instance != null
                ? PricingSystem.Instance.GetSeedCost(focusCrop)
                : 1f;
        }
        else if (todayPlantings.Count > 0)
        {
            qty = todayPlantings.Count;
            seedCost = PricingSystem.Instance != null
                ? PricingSystem.Instance.GetSeedCost(focusCrop)
                : 1f;
            pricePerUnit = PricingSystem.Instance != null
                ? PricingSystem.Instance.GetAskingPrice(focusCrop)
                : seedCost * 2f;
        }
        else
        {
            // Sem dados do dia: usa um exemplo padrão pra não deixar a tela vazia.
            qty = 3;
            seedCost = 2f;
            pricePerUnit = 5f;
        }

        float totalCusto = qty * seedCost;
        float totalReceita = qty * pricePerUnit;
        float sobra = totalReceita - totalCusto;
        bool hasSale = todaySales.Count > 0;

        var list = new List<DailyQuestion>
        {
            BuildCustoQuestion(focusCrop, qty, seedCost, totalCusto),
            BuildReceitaQuestion(focusCrop, qty, pricePerUnit, totalReceita, hasSale),
            BuildSobraQuestion(focusCrop, totalCusto, totalReceita, sobra, hasSale)
        };
        return list;
    }

    static List<DailyQuestion> PickQuestionsFromBank(string crop, int day)
    {
        var list = new List<DailyQuestion>();
        if (!HardcodedQuestions.TryGetValue(crop, out var pool) || pool.Length == 0)
            return list;

        int start = day % pool.Length;
        int count = Mathf.Min(3, pool.Length);
        for (int i = 0; i < count; i++)
            list.Add(CloneQuestion(pool[(start + i) % pool.Length]));
        return list;
    }

    static DailyQuestion CloneQuestion(DailyQuestion q)
    {
        return new DailyQuestion
        {
            questionId = q.questionId,
            cropName = q.cropName,
            theme = q.theme,
            statement = q.statement,
            options = (string[])q.options.Clone(),
            correctIndex = q.correctIndex,
            explanation = q.explanation
        };
    }

    static DailyQuestion Q(string id, string crop, string theme, string statement,
        string[] options, int correctIndex, string explanation)
    {
        return new DailyQuestion
        {
            questionId = id,
            cropName = crop,
            theme = theme,
            statement = statement,
            options = options,
            correctIndex = correctIndex,
            explanation = explanation
        };
    }

    public static string BuildReviewIfNoActivityToday(GameManager gm)
    {
        int day = gm.CurrentDay;
        bool planted = gm.Plantings.Exists(p => p.day == day);
        bool sold = gm.Sales.Exists(s => s.day == day);
        if (planted || sold) return null;

        var sb = new StringBuilder();
        sb.AppendLine("Hoje você não plantou nem vendeu.");
        sb.AppendLine("Vamos olhar sua jornada antes de começar outro dia.");
        sb.AppendLine();

        DailySaleRecord bestSale = null;
        foreach (var sale in gm.Sales)
        {
            if (bestSale == null || sale.total > bestSale.total)
                bestSale = sale;
        }

        if (bestSale != null)
        {
            sb.AppendLine($"Melhor venda até agora: {bestSale.quantity} × {bestSale.cropName} = R${bestSale.total:F2}.");
        }
        else
        {
            sb.AppendLine("Você ainda não fez uma venda. Amanhã tente colher e conversar com um comprador.");
        }

        if (gm.DailyBalances != null && gm.DailyBalances.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Dinheiro nos últimos dias:");
            int start = Mathf.Max(0, gm.DailyBalances.Count - 5);
            float max = Mathf.Max(1f, GameManager.StartingBalance);
            for (int i = start; i < gm.DailyBalances.Count; i++)
                if (gm.DailyBalances[i] > max) max = gm.DailyBalances[i];

            for (int i = start; i < gm.DailyBalances.Count; i++)
            {
                float value = gm.DailyBalances[i];
                int bars = Mathf.Clamp(Mathf.RoundToInt(value / max * 10f), 1, 10);
                sb.AppendLine($"Dia {i + 1}: R${value:F0} {new string('#', bars)}");
            }
        }

        return sb.ToString();
    }

    public static DailyCuriosity PickCuriosity(GameManager gm)
    {
        int day = gm.CurrentDay;
        var plantings = gm.Plantings.FindAll(p => p.day == day);
        var sales = gm.Sales.FindAll(s => s.day == day);
        string focus = ChooseFocusCrop(plantings, sales);
        if (string.IsNullOrEmpty(focus) || !Curiosities.ContainsKey(focus))
            focus = "Mandioca";
        var pool = Curiosities[focus];
        int idx = day % pool.Length; // alterna a curiosidade conforme avança o ciclo
        return new DailyCuriosity { cropName = focus, text = pool[idx] };
    }

    static string ChooseFocusCrop(List<PlantingRecord> plantings, List<DailySaleRecord> sales)
    {
        if (sales.Count > 0) return sales[0].cropName;
        if (plantings.Count > 0) return plantings[0].cropName;
        return null;
    }

    static DailyQuestion BuildCustoQuestion(string crop, int qty, float seedCost, float total)
    {
        var options = OptionsAroundCorrect(total);
        int correct = System.Array.IndexOf(options, FormatBRL(total));
        return new DailyQuestion
        {
            questionId = $"custo-{crop}",
            cropName = crop,
            theme = "Custo",
            statement = $"Você plantou {qty} {PluralCrop(crop, qty)}. Cada um custou R${seedCost:F2}. Quanto você gastou no total?",
            options = options,
            correctIndex = correct,
            explanation = $"{qty} × R${seedCost:F2} = R${total:F2}."
        };
    }

    static DailyQuestion BuildReceitaQuestion(string crop, int qty, float pricePerUnit, float total, bool hasSale)
    {
        var options = OptionsAroundCorrect(total);
        int correct = System.Array.IndexOf(options, FormatBRL(total));
        string verb = hasSale ? "Você vendeu" : "Se você vender";
        string explanationVerb = hasSale ? "recebeu" : "receberia";
        return new DailyQuestion
        {
            questionId = $"receita-{crop}",
            cropName = crop,
            theme = "Quanto você recebeu",
            statement = $"{verb} {qty} {PluralCrop(crop, qty)} por R${pricePerUnit:F2} cada. Quanto {(hasSale ? "recebeu" : "receberia")} no total?",
            options = options,
            correctIndex = correct,
            explanation = $"{qty} × R${pricePerUnit:F2} = R${total:F2}. Esse é o dinheiro que você {explanationVerb}."
        };
    }

    static DailyQuestion BuildSobraQuestion(string crop, float gasto, float recebido, float sobra, bool hasSale)
    {
        var options = OptionsAroundCorrect(sobra);
        int correct = System.Array.IndexOf(options, FormatBRL(sobra));
        string prompt = hasSale
            ? $"Você gastou R${gasto:F2} e recebeu R${recebido:F2}. Quanto sobrou?"
            : $"Você gastou R${gasto:F2}. Se receber R${recebido:F2}, quanto sobra?";
        return new DailyQuestion
        {
            questionId = $"sobra-{crop}",
            cropName = crop,
            theme = "Sobra",
            statement = prompt,
            options = options,
            correctIndex = correct,
            explanation = $"R${recebido:F2} − R${gasto:F2} = R${sobra:F2}."
        };
    }

    static string[] OptionsAroundCorrect(float value)
    {
        // Gera 3 alternativas: a certa + duas próximas (uma menor, uma maior).
        // Para sobra negativa preserva o sinal.
        float spread = Mathf.Max(Mathf.Abs(value) * 0.25f, 2f);
        float lower = value - spread;
        float higher = value + spread;
        // Garante que distractors não sejam iguais ao correto após arredondar.
        if (Mathf.Approximately(Mathf.Round(lower * 100f), Mathf.Round(value * 100f))) lower = value - spread - 1f;
        if (Mathf.Approximately(Mathf.Round(higher * 100f), Mathf.Round(value * 100f))) higher = value + spread + 1f;

        var raw = new[] { lower, value, higher };
        // Embaralha de forma determinística por valor pra não revelar posição.
        int seed = Mathf.RoundToInt(Mathf.Abs(value) * 100f) % 3;
        var output = new string[3];
        for (int i = 0; i < 3; i++)
            output[i] = FormatBRL(raw[(i + seed) % 3]);
        return output;
    }

    static string FormatBRL(float v) => $"R${v:F2}";

    static string PluralCrop(string crop, int qty)
    {
        string singular = crop switch
        {
            "Mandioca" => "pé de mandioca",
            "Cacau" => "pé de cacau",
            "Acai" => "açaizeiro",
            _ => crop.ToLower()
        };
        if (qty == 1) return singular;
        return crop switch
        {
            "Mandioca" => "pés de mandioca",
            "Cacau" => "pés de cacau",
            "Acai" => "açaizeiros",
            _ => singular + "s"
        };
    }
}
