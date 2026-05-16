using System.Collections.Generic;
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
    int _stepIndex;
    bool _showingCuriosity;
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
        _questions = DailyEducationGenerator.GenerateQuestions(gm);
        _curiosity = DailyEducationGenerator.PickCuriosity(gm);
        _stepIndex = 0;
        _showingCuriosity = false;
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
            if (_stepLabel != null) _stepLabel.text = $"Pergunta {_stepIndex + 1} de {_questions.Count + 1} · {q.theme}";
            if (_bodyLabel != null) _bodyLabel.text = q.statement;
            ShowOptions(q);
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
        var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
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
