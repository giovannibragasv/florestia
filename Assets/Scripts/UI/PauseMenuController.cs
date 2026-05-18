using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance { get; private set; }

    GameObject _root;
    TMP_Text _statusLabel;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (StudentProfilePicker.Instance != null && StudentProfilePicker.Instance.IsVisible) return;

        if (_root != null && _root.activeSelf) Hide();
        else Show();
    }

    public void Show()
    {
        if (_root == null) BuildUI();
        if (_statusLabel != null)
            _statusLabel.text = SaveSystem.ActiveProfile != null
                ? $"Aluno: {SaveSystem.ActiveProfile.name}"
                : "Sem aluno ativo";
        Time.timeScale = 0f;
        _root.SetActive(true);
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        if (_root != null) _root.SetActive(false);
    }

    void SaveAndRefreshStatus()
    {
        GameManager.Instance?.SaveNow();
        if (_statusLabel != null)
            _statusLabel.text = "Jogo salvo";
    }

    void SwitchStudent()
    {
        GameManager.Instance?.SaveNow();
        Hide();
        StudentProfilePicker.Instance?.Show();
    }

    void ResetProgress()
    {
        Hide();
        GameManager.Instance?.ResetRun();
        SceneManager.LoadScene("FarmScene");
    }

    void BuildUI()
    {
        Canvas canvas = FindOrCreateCanvas();
        _root = new GameObject("PauseMenu_UI");
        _root.transform.SetParent(canvas.transform, false);
        var rt = _root.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        _root.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.62f);

        var card = MakeRect("Card", _root.transform, Vector2.zero, new Vector2(460f, 430f));
        card.gameObject.AddComponent<Image>().color = new Color(0.15f, 0.10f, 0.065f, 0.98f);

        var title = MakeLabel("Title", card, "Pausa",
            new Vector2(0f, 162f), new Vector2(400f, 46f), 34,
            new Color(0.98f, 0.76f, 0.30f, 1f), TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;

        _statusLabel = MakeLabel("Status", card, "",
            new Vector2(0f, 118f), new Vector2(390f, 26f), 17,
            new Color(0.92f, 0.84f, 0.66f, 1f), TextAlignmentOptions.Center);

        var continueBtn = MakeButton("ContinueButton", "Continuar", card,
            new Vector2(0f, 62f), new Vector2(320f, 52f),
            new Color(0.88f, 0.62f, 0.22f, 1f), new Color(0.10f, 0.07f, 0.04f, 1f));
        continueBtn.onClick.AddListener(Hide);

        var saveBtn = MakeButton("SaveButton", "Salvar agora", card,
            new Vector2(0f, 4f), new Vector2(320f, 52f),
            new Color(0.42f, 0.28f, 0.13f, 1f), new Color(0.96f, 0.88f, 0.68f, 1f));
        saveBtn.onClick.AddListener(SaveAndRefreshStatus);

        var switchBtn = MakeButton("SwitchStudentButton", "Trocar aluno", card,
            new Vector2(0f, -54f), new Vector2(320f, 52f),
            new Color(0.34f, 0.24f, 0.16f, 1f), new Color(0.96f, 0.88f, 0.68f, 1f));
        switchBtn.onClick.AddListener(SwitchStudent);

        var resetBtn = MakeButton("ResetButton", "Reiniciar progresso", card,
            new Vector2(0f, -126f), new Vector2(320f, 48f),
            new Color(0.55f, 0.18f, 0.14f, 1f), new Color(0.98f, 0.90f, 0.80f, 1f));
        resetBtn.onClick.AddListener(ResetProgress);

        _root.SetActive(false);
    }

    static Canvas FindOrCreateCanvas()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null) return canvas;

        var go = new GameObject("PauseCanvas");
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 220;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static RectTransform MakeRect(string name, Transform parent, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return rt;
    }

    static TMP_Text MakeLabel(string name, Transform parent, string text,
        Vector2 anchoredPos, Vector2 size, int fontSize, Color color, TextAlignmentOptions align)
    {
        var rt = MakeRect(name, parent, anchoredPos, size);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = align;
        tmp.raycastTarget = false;
        var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) tmp.font = font;
        return tmp;
    }

    static Button MakeButton(string name, string label, Transform parent,
        Vector2 anchoredPos, Vector2 size, Color bg, Color labelColor)
    {
        var rt = MakeRect(name, parent, anchoredPos, size);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = bg;
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        var text = MakeLabel("Label", rt, label, Vector2.zero, size, 19, labelColor, TextAlignmentOptions.Center);
        text.fontStyle = FontStyles.Bold;
        return btn;
    }
}
