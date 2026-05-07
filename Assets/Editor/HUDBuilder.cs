using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class HUDBuilder
{
    [MenuItem("Florestia/Build Farm HUD")]
    static void Build()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene. Add one first.");
            return;
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
            new Vector2(8, -8), new Vector2(180, 110));
        tlPanel.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0.45f);

        var balanceLabel = MakeLabel("BalanceLabel", tlPanel,
            "R$50,00", new Vector2(90, -18), new Vector2(170, 32), 22);

        var dayLabel = MakeLabel("DayLabel", tlPanel,
            "Dia 1/15", new Vector2(90, -52), new Vector2(170, 28), 18);

        var staminaLabel = MakeLabel("StaminaLabel", tlPanel,
            "20/20", new Vector2(90, -84), new Vector2(80, 24), 16);

        // Stamina bar (placed right of the label)
        var sliderRT = MakeAnchoredRect("StaminaBar", tlPanel,
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(110, 10), new Vector2(60, 14));
        var slider = sliderRT.gameObject.AddComponent<Slider>();
        slider.minValue = 0f; slider.maxValue = 1f; slider.value = 1f;

        // ── Top-right: timer ────────────────────────────────
        var trPanel = MakeAnchoredRect("TopRight", hudGO.transform,
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-8, -8), new Vector2(110, 44));
        trPanel.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0.45f);

        var timerLabel = MakeLabel("TimerLabel", trPanel,
            "05:00", new Vector2(-55, -22), new Vector2(100, 36), 24);

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
}
