using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MarketSceneBuilder
{
    static readonly Color Ink = new Color(0.10f, 0.07f, 0.045f, 1f);
    static readonly Color Panel = new Color(0.18f, 0.13f, 0.09f, 0.96f);
    static readonly Color Panel2 = new Color(0.27f, 0.20f, 0.14f, 0.98f);
    static readonly Color Gold = new Color(0.92f, 0.67f, 0.28f, 1f);
    static readonly Color Cream = new Color(0.96f, 0.88f, 0.68f, 1f);

    [MenuItem("Florestia/Build Market Scene UI")]
    public static void Build()
    {
        DestroyIfExists("_BuyerSystem");
        DestroyIfExists("_MarketUI");
        DestroyIfExists("_BuyerSelector");
        DestroyIfExists("Canvas");

        EnsureCamera();
        EnsureEventSystem();

        var buyerSystem = new GameObject("_BuyerSystem");
        var buyerSystemComponent = buyerSystem.AddComponent<BuyerSystem>();
        Undo.RegisterCreatedObjectUndo(buyerSystem, "Build Market Scene");

        var marketUI = new GameObject("_MarketUI");
        Undo.RegisterCreatedObjectUndo(marketUI, "Build Market Scene");

        var buyerSelector = new GameObject("_BuyerSelector");
        Undo.RegisterCreatedObjectUndo(buyerSelector, "Build Market Scene");

        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(canvasGO, "Build Market Scene");

        Transform ct = canvasGO.transform;

        var background = MakeRect("Background", ct, Vector2.zero, Vector2.one,
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        var bgImage = background.gameObject.AddComponent<Image>();
        bgImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Market/market_night_background.png");
        bgImage.color = bgImage.sprite != null
            ? Color.white
            : new Color(0.07f, 0.06f, 0.09f, 1f);

        var groundBand = MakeRect("MarketGlow", ct, new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0.5f, 0f), new Vector2(0f, 0f), new Vector2(0f, 210f));
        groundBand.gameObject.AddComponent<Image>().color = new Color(0.12f, 0.07f, 0.035f, 0.35f);

        var panel = MakeRect("MarketPanel", ct, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1160f, 700f));
        panel.gameObject.AddComponent<Image>().color = Panel;

        var title = MakeLabel("TitleLabel", panel, "Mercado Noturno",
            new Vector2(0f, 300f), new Vector2(760f, 44f), 34, TextAlignmentOptions.Center, Gold);
        title.fontStyle = FontStyles.Bold;
        MakeLabel("SubtitleLabel", panel, "Negocie sua colheita antes do próximo amanhecer",
            new Vector2(0f, 266f), new Vector2(760f, 26f), 17, TextAlignmentOptions.Center, Cream);

        var leftPanel = MakeRect("BuyerPanel", panel, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
            new Vector2(0f, 0.5f), new Vector2(46f, -22f), new Vector2(360f, 540f));
        leftPanel.gameObject.AddComponent<Image>().color = Panel2;

        var portraitRT = MakeRect("BuyerPortrait", leftPanel, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f), new Vector2(0f, -34f), new Vector2(168f, 188f));
        var portrait = portraitRT.gameObject.AddComponent<Image>();
        portrait.color = Color.white;
        portrait.preserveAspect = true;

        var dialogueBubble = MakeRect("BuyerDialogueBubble", leftPanel,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, -154f), new Vector2(300f, 78f));
        dialogueBubble.gameObject.AddComponent<Image>().color = new Color(0.10f, 0.07f, 0.04f, 0.92f);
        var dialogueLabel = MakeLabel("BuyerDialogueLine", dialogueBubble, "Escolha um comprador.",
            Vector2.zero, new Vector2(276f, 62f), 17, TextAlignmentOptions.Center, Cream);
        dialogueLabel.textWrappingMode = TextWrappingModes.Normal;

        Button[] buyerButtons =
        {
            MakeButton("BuyerButton_0", "Comprador da Estrada", leftPanel, new Vector2(0f, -228f), new Vector2(260f, 48f), false),
            MakeButton("BuyerButton_1", "Feirante Local", leftPanel, new Vector2(0f, -286f), new Vector2(260f, 48f), false),
            MakeButton("BuyerButton_2", "Comprador Direto", leftPanel, new Vector2(0f, -344f), new Vector2(260f, 48f), false)
        };

        var tradePanel = MakeRect("TradePanel", panel, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
            new Vector2(1f, 0.5f), new Vector2(-46f, -22f), new Vector2(700f, 540f));
        tradePanel.gameObject.AddComponent<Image>().color = new Color(0.13f, 0.095f, 0.07f, 0.98f);

        var moodLabel = MakeLabel("MarketMoodLabel", tradePanel, "Clima da feira: normal", new Vector2(172f, 218f),
            new Vector2(430f, 26f), 17, TextAlignmentOptions.Right, new Color(0.98f, 0.78f, 0.32f, 1f));

        MakeLabel("CropHeader", tradePanel, "Produto", new Vector2(-230f, 178f),
            new Vector2(160f, 30f), 18, TextAlignmentOptions.Left, Gold);
        MakeLabel("PriceHeader", tradePanel, "Preço pedido", new Vector2(122f, 178f),
            new Vector2(220f, 30f), 18, TextAlignmentOptions.Left, Gold);

        // Three crop selector buttons (replaces the old TMP_Dropdown)
        string[] cropNames = { "Mandioca", "Cacau", "Acai" };
        string[] cropLabels = { "Mandioca", "Cacau", "Açaí" };
        float[] cropYs = { 148f, 102f, 56f };
        Button[] cropButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            cropButtons[i] = MakeButton($"CropButton_{cropNames[i]}", cropLabels[i], tradePanel,
                new Vector2(-220f, cropYs[i]), new Vector2(230f, 40f), false);
        }

        var stockLabel = MakeLabel("StockLabel", tradePanel, "Na sacola: 0",
            new Vector2(-220f, 14f), new Vector2(230f, 30f), 19, TextAlignmentOptions.Center, Cream);

        var sliderRT = MakeRect("PriceSlider", tradePanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(126f, 116f), new Vector2(360f, 38f));
        var slider = MakeSlider(sliderRT);
        slider.minValue = 1f;
        slider.maxValue = 50f;
        slider.value = 7f;

        var costLabel = MakeInfoLabel("CostLabel", tradePanel, "Custo: R$0", new Vector2(-220f, -104f));
        var priceLabel = MakeInfoLabel("PriceLabel", tradePanel, "Seu preço: R$0", new Vector2(0f, -104f));
        var marginLabel = MakeInfoLabel("MarginLabel", tradePanel, "Sobra: −", new Vector2(220f, -104f));

        var chanceLabel = MakeLabel("AcceptanceChanceLabel", tradePanel, "Chance de venda: --",
            new Vector2(126f, -164f), new Vector2(330f, 30f), 19,
            TextAlignmentOptions.Center, new Color(0.98f, 0.78f, 0.32f, 1f));
        chanceLabel.fontStyle = FontStyles.Bold;

        var sellBtn = MakeButton("SellButton", "Vender 1 unidade", tradePanel,
            new Vector2(-126f, -234f), new Vector2(260f, 62f), true);
        var endDayBtn = MakeButton("EndDayButton", "Encerrar dia", tradePanel,
            new Vector2(180f, -234f), new Vector2(220f, 62f), true);

        var muic = marketUI.AddComponent<MarketUIController>();
        SerializedObject so = new SerializedObject(muic);
        var cropButtonsProp = so.FindProperty("cropButtons");
        cropButtonsProp.arraySize = cropButtons.Length;
        for (int i = 0; i < cropButtons.Length; i++)
            cropButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = cropButtons[i];
        so.FindProperty("stockLabel").objectReferenceValue = stockLabel;
        so.FindProperty("priceSlider").objectReferenceValue = slider;
        so.FindProperty("marketMoodLabel").objectReferenceValue = moodLabel;
        so.FindProperty("acceptanceChanceLabel").objectReferenceValue = chanceLabel;
        so.FindProperty("costLabel").objectReferenceValue = costLabel;
        so.FindProperty("priceLabel").objectReferenceValue = priceLabel;
        so.FindProperty("marginLabel").objectReferenceValue = marginLabel;
        so.FindProperty("buyerDialogueLine").objectReferenceValue = dialogueLabel;
        so.FindProperty("buyerPortrait").objectReferenceValue = portrait;
        so.FindProperty("sellButton").objectReferenceValue = sellBtn;
        so.FindProperty("endDayButton").objectReferenceValue = endDayBtn;
        so.ApplyModifiedPropertiesWithoutUndo();

        var bs = buyerSelector.AddComponent<BuyerSelector>();
        BuyerData[] buyers =
        {
            AssetDatabase.LoadAssetAtPath<BuyerData>("Assets/Data/Buyers/Atravessador.asset"),
            AssetDatabase.LoadAssetAtPath<BuyerData>("Assets/Data/Buyers/Feirante.asset"),
            AssetDatabase.LoadAssetAtPath<BuyerData>("Assets/Data/Buyers/CompradorDireto.asset")
        };

        WireBuyerSystem(buyerSystemComponent, buyers);
        WireBuyerSelector(bs, muic, buyers, buyerButtons);

        SerializedObject muicBuyerSO = new SerializedObject(muic);
        muicBuyerSO.FindProperty("buyerSelector").objectReferenceValue = bs;
        muicBuyerSO.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeGameObject = panel.gameObject;
        FontAssigner.Assign();
        Debug.Log("Polished Market Scene UI built and wired.");
    }

    static RectTransform MakeRect(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        Undo.RegisterCreatedObjectUndo(go, "Build Market Scene");
        return rt;
    }

    static TMP_Text MakeLabel(string name, Transform parent, string text,
        Vector2 anchoredPos, Vector2 size, int fontSize,
        TextAlignmentOptions alignment, Color color)
    {
        var rt = MakeRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), anchoredPos, size);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = color;
        tmp.raycastTarget = false;
        return tmp;
    }

    static TMP_Text MakeInfoLabel(string name, Transform parent, string text, Vector2 anchoredPos)
    {
        var rt = MakeRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), anchoredPos, new Vector2(210f, 54f));
        rt.gameObject.AddComponent<Image>().color = new Color(0.22f, 0.16f, 0.11f, 1f);
        var label = MakeLabel(name + "_Text", rt, text, Vector2.zero,
            new Vector2(190f, 38f), 19, TextAlignmentOptions.Center, Cream);
        return label;
    }

    static Slider MakeSlider(RectTransform root)
    {
        var slider = root.gameObject.AddComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.wholeNumbers = false;

        var background = MakeRect("Background", root, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f),
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-18f, 12f));
        background.gameObject.AddComponent<Image>().color = new Color(0.24f, 0.17f, 0.11f, 1f);

        var fillArea = MakeRect("Fill Area", root, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f),
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-28f, 16f));
        var fill = MakeRect("Fill", fillArea, new Vector2(0f, 0f), new Vector2(1f, 1f),
            new Vector2(0f, 0.5f), Vector2.zero, Vector2.zero);
        fill.gameObject.AddComponent<Image>().color = Gold;

        var handleArea = MakeRect("Handle Slide Area", root, new Vector2(0f, 0f), new Vector2(1f, 1f),
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-28f, 0f));
        var handle = MakeRect("Handle", handleArea, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(28f, 34f));
        var handleImage = handle.gameObject.AddComponent<Image>();
        handleImage.color = new Color(1f, 0.79f, 0.26f, 1f);

        slider.fillRect = fill;
        slider.handleRect = handle;
        slider.targetGraphic = handleImage;
        return slider;
    }

    static Button MakeButton(string name, string label, Transform parent,
        Vector2 anchoredPos, Vector2 size, bool primary)
    {
        var rt = MakeRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), anchoredPos, size);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = primary ? Gold : new Color(0.25f, 0.20f, 0.16f, 1f);
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        MakeLabel(name + "_Label", rt, label, Vector2.zero, size, 18,
            TextAlignmentOptions.Center, primary ? Ink : Cream);
        return btn;
    }

    static void WireBuyerSystem(BuyerSystem buyerSystem, BuyerData[] buyers)
    {
        SerializedObject so = new SerializedObject(buyerSystem);
        var buyersProp = so.FindProperty("buyers");
        buyersProp.arraySize = buyers.Length;
        for (int i = 0; i < buyers.Length; i++)
            buyersProp.GetArrayElementAtIndex(i).objectReferenceValue = buyers[i];
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void WireBuyerSelector(BuyerSelector selector, MarketUIController marketUI,
        BuyerData[] buyers, Button[] buttons)
    {
        SerializedObject so = new SerializedObject(selector);
        so.FindProperty("marketUI").objectReferenceValue = marketUI;
        var buyersProp = so.FindProperty("buyers");
        buyersProp.arraySize = buyers.Length;
        for (int i = 0; i < buyers.Length; i++)
            buyersProp.GetArrayElementAtIndex(i).objectReferenceValue = buyers[i];

        var buttonsProp = so.FindProperty("buyerButtons");
        buttonsProp.arraySize = buttons.Length;
        for (int i = 0; i < buttons.Length; i++)
            buttonsProp.GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void EnsureCamera()
    {
        if (Camera.main != null) return;

        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        cameraGO.transform.position = new Vector3(0f, 0f, -10f);
        var camera = cameraGO.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.backgroundColor = new Color(0.07f, 0.06f, 0.09f, 1f);
        cameraGO.AddComponent<AudioListener>();
        Undo.RegisterCreatedObjectUndo(cameraGO, "Build Market Scene");
    }

    static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null) return;

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
        Undo.RegisterCreatedObjectUndo(eventSystem, "Build Market Scene");
    }

    static void DestroyIfExists(string name)
    {
        var existing = GameObject.Find(name);
        if (existing != null)
            Undo.DestroyObjectImmediate(existing);
    }
}
