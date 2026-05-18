using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance { get; private set; }

    GameObject _root;
    TMP_Text _titleLabel;
    TMP_Text _bodyLabel;
    Button _okButton;
    TMP_Text _okButtonLabel;
    Button _skipButton;
    Button _closeButton;

    bool _planted;
    bool _watered;
    bool _harvested;
    bool _pendingMarketTip;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        if (ShouldShowTutorial())
            ShowWelcome();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!ShouldShowTutorial()) { Hide(); return; }

        if (scene.name == "FarmScene" && !_planted)
            ShowTip("Bem-vindo à roça!", "Escolha uma semente nos botões de baixo e clique em um canteiro vazio para plantar.", "Entendi");
        else if (scene.name == "MarketScene" && _pendingMarketTip)
            ShowMarketTip();
    }

    bool ShouldShowTutorial()
    {
        return GameManager.Instance != null && !GameManager.Instance.TutorialCompleted;
    }

    void ShowWelcome()
    {
        ShowTip("Bem-vindo à Florestia!", "Você vai plantar, regar, colher e vender para cuidar do dinheiro da família. Eu vou te guiar nos primeiros passos.", "Começar");
    }

    public void NotifyPlanted()
    {
        if (!ShouldShowTutorial() || _planted) return;
        _planted = true;
        ShowTip("Boa plantação!", "Agora escolha o regador e clique na plantação. Planta regada cresce no fim do dia.", "Vou regar");
    }

    public void NotifyWatered()
    {
        if (!ShouldShowTutorial() || _watered) return;
        _watered = true;
        ShowTip("Terra molhada!", "Quando a barra da planta encher, escolha a ferramenta de colher e pegue sua produção.", "Certo");
    }

    public void NotifyHarvested()
    {
        if (!ShouldShowTutorial() || _harvested) return;
        _harvested = true;
        ShowTip("Colheita na sacola!", "Agora atravesse a ponte ou espere o dia acabar para ir ao mercado vender.", "Ir vender");
    }

    public void NotifyMarketEntered()
    {
        if (!ShouldShowTutorial()) return;
        _pendingMarketTip = true;
    }

    void ShowMarketTip()
    {
        _pendingMarketTip = false;
        ShowTip("Hora da feira!", "Escolha uma cultura, converse com um comprador e teste o preço. Se vender por mais do que gastou, sobra dinheiro.", "Terminar tutorial", CompleteTutorial);
    }

    void ShowTip(string title, string body, string okText, UnityEngine.Events.UnityAction extraOkAction = null)
    {
        BuildUI();
        if (_root == null) return;

        _titleLabel.text = title;
        _bodyLabel.text = body;
        _okButtonLabel.text = okText;
        _okButton.onClick.RemoveAllListeners();
        if (extraOkAction != null) _okButton.onClick.AddListener(extraOkAction);
        _okButton.onClick.AddListener(Hide);
        _root.SetActive(true);
    }

    void CompleteTutorial()
    {
        GameManager.Instance?.CompleteTutorial();
    }

    void SkipTutorial()
    {
        CompleteTutorial();
        Hide();
    }

    void Hide()
    {
        if (_root != null) _root.SetActive(false);
    }

    void BuildUI()
    {
        if (_root != null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        _root = new GameObject("TutorialOverlay");
        _root.transform.SetParent(canvas.transform, false);
        var rt = _root.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var frame = Rect("CardFrame", _root.transform, new Vector2(0.5f, 0f), new Vector2(0f, 116f), new Vector2(636f, 186f));
        frame.gameObject.AddComponent<Image>().color = new Color(0.60f, 0.30f, 0.08f, 0.98f);

        var card = Rect("Card", frame.transform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(616f, 166f));
        card.gameObject.AddComponent<Image>().color = new Color(0.96f, 0.67f, 0.34f, 0.98f);

        _titleLabel = Label("Title", card.transform, "Bem-vindo à Florestia!",
            new Vector2(0f, 50f), new Vector2(520f, 34f), 23, new Color(0.36f, 0.15f, 0.04f));
        _titleLabel.fontStyle = FontStyles.Bold;
        _bodyLabel = Label("Body", card.transform, "",
            new Vector2(0f, 2f), new Vector2(552f, 56f), 17, new Color(0.24f, 0.10f, 0.03f));
        _bodyLabel.textWrappingMode = TextWrappingModes.Normal;

        _okButton = ButtonAt("OkButton", card.transform, new Vector2(-76f, -58f), new Vector2(150f, 40f), "Entendi");
        _okButtonLabel = _okButton.GetComponentInChildren<TMP_Text>();
        _skipButton = ButtonAt("SkipButton", card.transform, new Vector2(112f, -58f), new Vector2(190f, 40f), "Pular tutorial");
        _skipButton.onClick.AddListener(SkipTutorial);
        _closeButton = ButtonAt("CloseButton", card.transform, new Vector2(284f, 58f), new Vector2(32f, 32f), "X");
        _closeButton.onClick.AddListener(Hide);

        _root.SetActive(false);
    }

    static RectTransform Rect(string name, Transform parent, Vector2 pivot, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = parent.name == "TutorialOverlay" ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 0.5f);
        rt.anchorMax = rt.anchorMin;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return rt;
    }

    static TMP_Text Label(string name, Transform parent, string text, Vector2 anchoredPos, Vector2 size, int fontSize, Color color)
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
        image.color = name == "CloseButton"
            ? new Color(0.75f, 0.18f, 0.12f, 1f)
            : name == "SkipButton"
                ? new Color(0.72f, 0.40f, 0.16f, 1f)
                : new Color(0.93f, 0.78f, 0.38f, 1f);
        var button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var label = Label("Label", rt.transform, text, Vector2.zero, size, 18,
            name == "CloseButton" ? Color.white : new Color(0.16f, 0.07f, 0.02f));
        label.fontStyle = FontStyles.Bold;
        return button;
    }
}
