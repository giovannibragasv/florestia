using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class MarketSceneBuilder
{
    [MenuItem("Florestia/Build Market Scene UI")]
    static void Build()
    {
        // --- Systems ---
        var buyerSystem = new GameObject("_BuyerSystem");
        buyerSystem.AddComponent<BuyerSystem>();
        Undo.RegisterCreatedObjectUndo(buyerSystem, "Build Market Scene");

        var marketUI = new GameObject("_MarketUI");
        Undo.RegisterCreatedObjectUndo(marketUI, "Build Market Scene");

        var buyerSelector = new GameObject("_BuyerSelector");
        Undo.RegisterCreatedObjectUndo(buyerSelector, "Build Market Scene");

        // --- Canvas ---
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(canvasGO, "Build Market Scene");

        // helper
        RectTransform MakeRect(string name, Transform parent,
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

        TMP_Text MakeLabel(string name, Transform parent, string text,
            Vector2 anchoredPos, Vector2 size, int fontSize = 24)
        {
            var rt = MakeRect(name, parent,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), anchoredPos, size);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.black;
            return tmp;
        }

        Button MakeButton(string name, string label, Transform parent,
            Vector2 anchoredPos, Vector2 size)
        {
            var rt = MakeRect(name, parent,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), anchoredPos, size);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = new Color(0.85f, 0.85f, 0.85f);
            var btn = rt.gameObject.AddComponent<Button>();
            var lbl = MakeLabel(name + "_Label", rt, label,
                Vector2.zero, size, 20);
            return btn;
        }

        Transform ct = canvasGO.transform;

        // Background panel
        var bg = MakeRect("Background", ct,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero);
        bg.gameObject.AddComponent<Image>().color = new Color(0.96f, 0.93f, 0.85f);

        // Title
        MakeLabel("TitleLabel", ct, "Mercado Noturno",
            new Vector2(0, 200), new Vector2(400, 50), 32);

        // Stock
        var stockLabel = MakeLabel("StockLabel", ct, "Estoque: 0",
            new Vector2(-200, 120), new Vector2(200, 40));

        // Price slider
        var sliderRT = MakeRect("PriceSlider", ct,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0, 60), new Vector2(300, 40));
        var slider = sliderRT.gameObject.AddComponent<Slider>();
        slider.minValue = 1f;
        slider.maxValue = 50f;
        slider.value = 7f;

        // Price info labels
        var costLabel   = MakeLabel("CostLabel",   ct, "Custo: R$0",   new Vector2(-150, 10), new Vector2(180, 35), 20);
        var priceLabel  = MakeLabel("PriceLabel",  ct, "Seu preço: R$0", new Vector2(0, 10),  new Vector2(200, 35), 20);
        var marginLabel = MakeLabel("MarginLabel", ct, "Margem: —",    new Vector2(160, 10),  new Vector2(200, 35), 20);

        // Crop dropdown
        var dropdownRT = MakeRect("CropDropdown", ct,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0, 160), new Vector2(240, 45));
        var dropdown = dropdownRT.gameObject.AddComponent<TMP_Dropdown>();
        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData("Mandioca"));
        dropdown.options.Add(new TMP_Dropdown.OptionData("Cacau"));
        dropdown.options.Add(new TMP_Dropdown.OptionData("Acai"));

        // Buyer portrait
        var portraitRT = MakeRect("BuyerPortrait", ct,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(100, 100));
        var portrait = portraitRT.gameObject.AddComponent<Image>();
        portrait.color = Color.grey;

        // Buyer dialogue
        var dialogueLabel = MakeLabel("BuyerDialogueLine", ct, "",
            new Vector2(0, -170), new Vector2(400, 50), 18);

        // Buyer buttons (3)
        var btn0 = MakeButton("BuyerButton_0", "Atravessador", ct, new Vector2(-170, -130), new Vector2(130, 45));
        var btn1 = MakeButton("BuyerButton_1", "Feirante",     ct, new Vector2(0,    -130), new Vector2(130, 45));
        var btn2 = MakeButton("BuyerButton_2", "Comprador",    ct, new Vector2(170,  -130), new Vector2(130, 45));

        // Sell / End Day buttons
        var sellBtn   = MakeButton("SellButton",   "Vender",      ct, new Vector2(-80, -240), new Vector2(140, 50));
        var endDayBtn = MakeButton("EndDayButton", "Encerrar dia", ct, new Vector2(80, -240), new Vector2(160, 50));

        // --- Wire MarketUIController ---
        var muic = marketUI.AddComponent<MarketUIController>();
        SerializedObject so = new SerializedObject(muic);
        so.FindProperty("cropDropdown").objectReferenceValue     = dropdown;
        so.FindProperty("stockLabel").objectReferenceValue       = stockLabel;
        so.FindProperty("priceSlider").objectReferenceValue      = slider;
        so.FindProperty("costLabel").objectReferenceValue        = costLabel;
        so.FindProperty("priceLabel").objectReferenceValue       = priceLabel;
        so.FindProperty("marginLabel").objectReferenceValue      = marginLabel;
        so.FindProperty("buyerDialogueLine").objectReferenceValue = dialogueLabel;
        so.FindProperty("buyerPortrait").objectReferenceValue    = portrait;
        so.FindProperty("sellButton").objectReferenceValue       = sellBtn;
        so.FindProperty("endDayButton").objectReferenceValue     = endDayBtn;
        so.ApplyModifiedPropertiesWithoutUndo();

        // --- Wire BuyerSelector ---
        var bs = buyerSelector.AddComponent<BuyerSelector>();
        SerializedObject bsSO = new SerializedObject(bs);
        bsSO.FindProperty("marketUI").objectReferenceValue = muic;
        var btnsProp = bsSO.FindProperty("buyerButtons");
        btnsProp.arraySize = 3;
        btnsProp.GetArrayElementAtIndex(0).objectReferenceValue = btn0;
        btnsProp.GetArrayElementAtIndex(1).objectReferenceValue = btn1;
        btnsProp.GetArrayElementAtIndex(2).objectReferenceValue = btn2;
        bsSO.ApplyModifiedPropertiesWithoutUndo();

        Debug.Log("Market Scene UI built. Assign BuyerData to _BuyerSystem → Buyers in the Inspector.");
    }
}
