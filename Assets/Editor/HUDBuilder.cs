using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class HUDBuilder
{
    [MenuItem("Florestia/Build Farm HUD")]
    public static void Build()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var cgo = new GameObject("Canvas");
            canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = cgo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            cgo.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(cgo, "Build Farm HUD");
        }
        else
        {
            var scaler = canvas.GetComponent<CanvasScaler>() ?? canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }
        Transform ct = canvas.transform;

        // Remove old HUD panel if present
        var old = ct.Find("HUD");
        if (old != null) Undo.DestroyObjectImmediate(old.gameObject);

        // HUD root (transparent, full-screen)
        var hudGO = new GameObject("HUD");
        hudGO.transform.SetParent(ct, false);
        var hudRT = hudGO.AddComponent<RectTransform>();
        hudRT.anchorMin = Vector2.zero;
        hudRT.anchorMax = Vector2.one;
        hudRT.offsetMin = Vector2.zero;
        hudRT.offsetMax = Vector2.zero;
        Undo.RegisterCreatedObjectUndo(hudGO, "Build Farm HUD");

        // ── Top-left panel ──────────────────────────────────
        var tlPanel = MakeAnchoredRect("TopLeft", hudGO.transform,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(16, -16), new Vector2(210, 98));
        tlPanel.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.055f, 0.035f, 0.78f);

        MakeIcon("CoinIcon", tlPanel, "Assets/Sprites/UI/ui_coin.png",
            new Vector2(14, -14), new Vector2(26, 26));
        MakeIcon("StaminaIcon", tlPanel, "Assets/Sprites/UI/ui_stamina.png",
            new Vector2(14, -62), new Vector2(24, 24));

        var balanceLabel = MakeLabel("BalanceLabel", tlPanel,
            "R$50,00", new Vector2(48, -12), new Vector2(150, 30), 21);

        var dayLabel = MakeLabel("DayLabel", tlPanel,
            "Dia 1/15", new Vector2(48, -40), new Vector2(150, 24), 17);

        var staminaLabel = MakeLabel("StaminaLabel", tlPanel,
            "20/20", new Vector2(48, -68), new Vector2(70, 22), 15);

        // Stamina bar (placed right of the label)
        var sliderRT = MakeAnchoredRect("StaminaBar", tlPanel,
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(116, 13), new Vector2(78, 14));
        var slider = sliderRT.gameObject.AddComponent<Slider>();
        slider.minValue = 0f; slider.maxValue = 1f; slider.value = 1f;

        // ── Top-right: timer ────────────────────────────────
        var trPanel = MakeAnchoredRect("TopRight", hudGO.transform,
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-16, -16), new Vector2(132, 46));
        trPanel.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.055f, 0.035f, 0.78f);
        MakeIcon("SunIcon", trPanel, "Assets/Sprites/UI/ui_sun.png",
            new Vector2(-118, -10), new Vector2(26, 26));

        var timerLabel = MakeLabel("TimerLabel", trPanel,
            "05:00", new Vector2(-86, -10), new Vector2(74, 30), 22);
        timerLabel.alignment = TextAlignmentOptions.Right;

        // ── Wire HUDController ──────────────────────────────
        var hudCtrlGO = GameObject.Find("_HUD");
        if (hudCtrlGO == null) hudCtrlGO = new GameObject("_HUD");
        Undo.RegisterCreatedObjectUndo(hudCtrlGO, "Build Farm HUD");

        var hud = hudCtrlGO.GetComponent<HUDController>() ?? hudCtrlGO.AddComponent<HUDController>();
        SerializedObject so = new SerializedObject(hud);
        so.FindProperty("balanceLabel").objectReferenceValue = balanceLabel;
        so.FindProperty("dayLabel").objectReferenceValue     = dayLabel;
        so.FindProperty("staminaBar").objectReferenceValue   = slider;
        so.FindProperty("staminaLabel").objectReferenceValue = staminaLabel;
        so.FindProperty("timerLabel").objectReferenceValue   = timerLabel;
        so.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeGameObject = hudGO;
        Debug.Log("Farm HUD built and wired. Assign LiberationSans SDF to TMP labels if needed.");
    }

    static RectTransform MakeAnchoredRect(string name, Transform parent,
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
        Undo.RegisterCreatedObjectUndo(go, "Build Farm HUD");
        return rt;
    }

    static TMP_Text MakeLabel(string name, RectTransform parent,
        string text, Vector2 anchoredPos, Vector2 size, int fontSize)
    {
        var rt = MakeAnchoredRect(name, parent,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            anchoredPos, size);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = Color.white;
        return tmp;
    }

    static void MakeIcon(string name, RectTransform parent, string spritePath,
        Vector2 anchoredPos, Vector2 size)
    {
        var rt = MakeAnchoredRect(name, parent,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            anchoredPos, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        image.color = Color.white;
        image.preserveAspect = true;
    }
}
