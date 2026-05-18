using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StudentProfilePicker : MonoBehaviour
{
    public static StudentProfilePicker Instance { get; private set; }

    GameObject _root;
    TMP_InputField _nameInput;
    Transform _profilesContainer;
    GameObject _activeActionsPanel;
    TMP_Text _activeLabel;
    bool _firstFrameDone;
    public bool IsVisible => _root != null && _root.activeSelf;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        BuildUI();
        if (SaveSystem.ActiveProfile == null && SaveSystem.ListProfiles().Count == 0)
            Show();
        else
            Hide();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)
            && _root != null && _root.activeSelf
            && SaveSystem.ActiveProfile != null)
            Hide();
    }

    public void Show()
    {
        if (_root == null) BuildUI();
        RefreshProfilesList();
        RefreshActiveActions();
        _root.SetActive(true);
    }

    public void Hide()
    {
        if (_root != null) _root.SetActive(false);
    }

    void OnPickProfile(StudentProfile p)
    {
        SaveSystem.SetActiveProfile(p);
        Hide();
        SceneManager.LoadScene("FarmScene");
    }

    void OnCreateProfile()
    {
        string name = _nameInput != null ? _nameInput.text : "";
        if (string.IsNullOrWhiteSpace(name)) return;
        var p = SaveSystem.FindOrCreateProfile(name);
        if (p == null) return;
        if (_nameInput != null) _nameInput.text = "";
        OnPickProfile(p);
    }

    void OnResetCurrentProgress()
    {
        if (SaveSystem.ActiveProfile == null) return;
        GameManager.Instance?.ResetRun();
        Hide();
        SceneManager.LoadScene("FarmScene");
    }

    void OnDeleteCurrentProfile()
    {
        var current = SaveSystem.ActiveProfile;
        if (current == null) return;
        SaveSystem.DeleteProfile(current);
        SaveSystem.ClearActiveProfile();
        GameManager.Instance?.ResetRun();
        RefreshProfilesList();
        RefreshActiveActions();
        // Don't hide — user still needs to pick or create a profile.
        if (SaveSystem.ListProfiles().Count == 0) return;
    }

    // ---------- UI ----------

    void BuildUI()
    {
        if (_root != null) return;

        Canvas canvas = FindOrCreateCanvas();
        _root = new GameObject("StudentProfilePicker_UI");
        _root.transform.SetParent(canvas.transform, false);
        var rt = _root.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var dim = _root.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.86f);

        // Card
        var card = MakeRect("Card", _root.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(680f, 620f));
        card.gameObject.AddComponent<Image>().color = new Color(0.14f, 0.10f, 0.07f, 1f);

        MakeLabel("Title", card, "Quem está jogando?",
            new Vector2(0f, 265f), new Vector2(620f, 50f), 32,
            new Color(0.98f, 0.84f, 0.45f, 1f), TextAlignmentOptions.Center);

        MakeLabel("Subtitle", card, "Escolha seu nome ou crie um novo aluno",
            new Vector2(0f, 222f), new Vector2(620f, 28f), 16,
            new Color(0.92f, 0.85f, 0.68f, 1f), TextAlignmentOptions.Center);

        // Profiles list
        var listGO = new GameObject("ProfilesList");
        listGO.transform.SetParent(card, false);
        var listRT = listGO.AddComponent<RectTransform>();
        listRT.anchorMin = new Vector2(0.5f, 0.5f);
        listRT.anchorMax = new Vector2(0.5f, 0.5f);
        listRT.pivot = new Vector2(0.5f, 1f);
        listRT.anchoredPosition = new Vector2(0f, 180f);
        listRT.sizeDelta = new Vector2(600f, 220f);
        var vlg = listGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6f;
        vlg.padding = new RectOffset(8, 8, 8, 8);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        _profilesContainer = listGO.transform;

        // New profile section
        MakeLabel("NewLabel", card, "Ou comece como novo aluno:",
            new Vector2(0f, -90f), new Vector2(600f, 28f), 16,
            new Color(0.92f, 0.85f, 0.68f, 1f), TextAlignmentOptions.Center);

        var inputRT = MakeRect("NameInput", card, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-100f, -140f), new Vector2(380f, 54f));
        inputRT.gameObject.AddComponent<Image>().color = new Color(0.88f, 0.78f, 0.52f, 1f);
        _nameInput = inputRT.gameObject.AddComponent<TMP_InputField>();
        BuildInputField(inputRT, _nameInput);

        var beginBtn = MakeButton("BeginButton", "Começar", card,
            new Vector2(170f, -140f), new Vector2(180f, 54f),
            new Color(0.88f, 0.62f, 0.22f, 1f), new Color(0.10f, 0.07f, 0.04f, 1f));
        beginBtn.onClick.AddListener(OnCreateProfile);

        // Active profile actions (reset / delete)
        _activeActionsPanel = new GameObject("ActiveActions");
        _activeActionsPanel.transform.SetParent(card, false);
        var aart = _activeActionsPanel.AddComponent<RectTransform>();
        aart.anchorMin = new Vector2(0.5f, 0.5f);
        aart.anchorMax = new Vector2(0.5f, 0.5f);
        aart.pivot = new Vector2(0.5f, 0.5f);
        aart.anchoredPosition = new Vector2(0f, -230f);
        aart.sizeDelta = new Vector2(620f, 90f);

        _activeLabel = MakeLabel("ActiveLabel", _activeActionsPanel.transform, "",
            new Vector2(0f, 32f), new Vector2(600f, 24f), 14,
            new Color(0.70f, 0.62f, 0.50f, 1f), TextAlignmentOptions.Center);

        var resetBtn = MakeButton("ResetButton", "Reiniciar progresso", _activeActionsPanel.transform,
            new Vector2(-150f, -8f), new Vector2(260f, 44f),
            new Color(0.50f, 0.32f, 0.16f, 1f), new Color(0.96f, 0.88f, 0.68f, 1f));
        resetBtn.onClick.AddListener(OnResetCurrentProgress);

        var deleteBtn = MakeButton("DeleteButton", "Apagar este aluno", _activeActionsPanel.transform,
            new Vector2(150f, -8f), new Vector2(260f, 44f),
            new Color(0.55f, 0.18f, 0.14f, 1f), new Color(0.98f, 0.90f, 0.80f, 1f));
        deleteBtn.onClick.AddListener(OnDeleteCurrentProfile);

        _root.SetActive(false);
    }

    void RefreshProfilesList()
    {
        if (_profilesContainer == null) return;
        for (int i = _profilesContainer.childCount - 1; i >= 0; i--)
            Destroy(_profilesContainer.GetChild(i).gameObject);

        var profiles = SaveSystem.ListProfiles();
        foreach (var p in profiles)
        {
            var captured = p;
            bool isActive = SaveSystem.ActiveProfile != null
                && string.Equals(SaveSystem.ActiveProfile.name, p.name, System.StringComparison.OrdinalIgnoreCase);
            string label = (isActive ? "▶  " : "    ") + p.name;
            Color bg = isActive
                ? new Color(0.45f, 0.32f, 0.15f, 1f)
                : new Color(0.25f, 0.20f, 0.16f, 1f);
            var btn = MakeButton($"Profile_{p.name}", label, _profilesContainer,
                Vector2.zero, new Vector2(560f, 48f), bg, new Color(0.96f, 0.88f, 0.68f, 1f));
            btn.onClick.AddListener(() => OnPickProfile(captured));
        }
    }

    void RefreshActiveActions()
    {
        if (_activeActionsPanel == null || _activeLabel == null) return;
        bool hasActive = SaveSystem.ActiveProfile != null;
        _activeActionsPanel.SetActive(hasActive);
        if (hasActive)
            _activeLabel.text = $"Aluno atual: {SaveSystem.ActiveProfile.name}";
    }

    // ---------- helpers ----------

    static Canvas FindOrCreateCanvas()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null) return canvas;

        var go = new GameObject("PickerCanvas");
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static RectTransform MakeRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 anchoredPos, Vector2 size)
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

    static TMP_Text MakeLabel(string name, Transform parent, string text,
        Vector2 anchoredPos, Vector2 size, int fontSize, Color color, TextAlignmentOptions align)
    {
        var rt = MakeRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            anchoredPos, size);
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
        var rt = MakeRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            anchoredPos, size);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = bg;
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        MakeLabel("Label", rt, label, Vector2.zero, size, 18, labelColor, TextAlignmentOptions.Center);
        return btn;
    }

    static void BuildInputField(RectTransform parent, TMP_InputField input)
    {
        var textArea = new GameObject("TextArea");
        textArea.transform.SetParent(parent, false);
        var taRT = textArea.AddComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(12f, 8f);
        taRT.offsetMax = new Vector2(-12f, -8f);
        textArea.AddComponent<RectMask2D>();

        var placeholder = MakeLabel("Placeholder", textArea.transform, "Seu nome",
            Vector2.zero, taRT.sizeDelta, 20,
            new Color(0.45f, 0.35f, 0.20f, 1f), TextAlignmentOptions.Left);
        var textTMP = MakeLabel("Text", textArea.transform, "",
            Vector2.zero, taRT.sizeDelta, 20,
            new Color(0.10f, 0.07f, 0.04f, 1f), TextAlignmentOptions.Left);

        input.textViewport = taRT;
        input.textComponent = textTMP;
        input.placeholder = placeholder;
        input.characterLimit = 24;
    }
}
