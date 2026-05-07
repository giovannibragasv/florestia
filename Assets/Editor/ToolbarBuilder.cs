using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class ToolbarBuilder
{
    static readonly string[] SlotLabels = { "Mandioca", "Cacau", "Açaí", "Água", "Colher" };
    const float SlotSize   = 64f;
    const float SlotGap    = 8f;
    const float BarPadding = 10f;

    [MenuItem("Florestia/Build Farm Toolbar")]
    static void Build()
    {
        // Find or create canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var cgo = new GameObject("Canvas");
            canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cgo.AddComponent<CanvasScaler>();
            cgo.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(cgo, "Build Farm Toolbar");
        }
        Transform ct = canvas.transform;

        // Toolbar background strip
        float barWidth = 5 * SlotSize + 4 * SlotGap + 2 * BarPadding;
        var barGO = new GameObject("Toolbar");
        barGO.transform.SetParent(ct, false);
        var barRT = barGO.AddComponent<RectTransform>();
        barRT.anchorMin = new Vector2(0.5f, 0f);
        barRT.anchorMax = new Vector2(0.5f, 0f);
        barRT.pivot     = new Vector2(0.5f, 0f);
        barRT.anchoredPosition = new Vector2(0f, 10f);
        barRT.sizeDelta = new Vector2(barWidth, SlotSize + 2 * BarPadding);
        barGO.AddComponent<Image>().color = new Color(0.14f, 0.09f, 0.05f, 0.90f);
        Undo.RegisterCreatedObjectUndo(barGO, "Build Farm Toolbar");

        // Toolbar controller object
        var tcGO = new GameObject("_Toolbar");
        Undo.RegisterCreatedObjectUndo(tcGO, "Build Farm Toolbar");
        var tc = tcGO.AddComponent<ToolbarController>();

        // Slots
        var slotBackgrounds = new Image[5];
        float startX = -(2 * (SlotSize + SlotGap));

        for (int i = 0; i < 5; i++)
        {
            var slotGO = new GameObject($"Slot_{i}_{SlotLabels[i]}");
            slotGO.transform.SetParent(barRT, false);
            var slotRT = slotGO.AddComponent<RectTransform>();
            slotRT.anchorMin = new Vector2(0.5f, 0.5f);
            slotRT.anchorMax = new Vector2(0.5f, 0.5f);
            slotRT.pivot     = new Vector2(0.5f, 0.5f);
            slotRT.sizeDelta = new Vector2(SlotSize, SlotSize);
            slotRT.anchoredPosition = new Vector2(startX + i * (SlotSize + SlotGap), 0f);
            var bg = slotGO.AddComponent<Image>();
            bg.color = new Color(0.25f, 0.18f, 0.10f, 0.85f);
            slotBackgrounds[i] = bg;
            Undo.RegisterCreatedObjectUndo(slotGO, "Build Farm Toolbar");

            // Slot label (number + name)
            var lblGO = new GameObject("Label");
            lblGO.transform.SetParent(slotRT, false);
            var lblRT = lblGO.AddComponent<RectTransform>();
            lblRT.anchorMin = Vector2.zero;
            lblRT.anchorMax = Vector2.one;
            lblRT.offsetMin = Vector2.zero;
            lblRT.offsetMax = Vector2.zero;
            var tmp = lblGO.AddComponent<TextMeshProUGUI>();
            tmp.text = $"{i + 1}\n{SlotLabels[i]}";
            tmp.fontSize = 10;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            Undo.RegisterCreatedObjectUndo(lblGO, "Build Farm Toolbar");
        }

        // Wire ToolbarController
        SerializedObject so = new SerializedObject(tc);
        var bgProp = so.FindProperty("slotBackgrounds");
        bgProp.arraySize = 5;
        for (int i = 0; i < 5; i++)
            bgProp.GetArrayElementAtIndex(i).objectReferenceValue = slotBackgrounds[i];
        so.ApplyModifiedPropertiesWithoutUndo();

        // Wire ToolbarController buttons to Select (via Button components on each slot)
        for (int i = 0; i < 5; i++)
        {
            var slotGO = slotBackgrounds[i].gameObject;
            var btn = slotGO.AddComponent<Button>();
            btn.targetGraphic = slotBackgrounds[i];
            int captured = i;
            // Runtime wiring via UnityEvents isn't easily done from Editor scripts;
            // keyboard shortcuts (1-5) already work via ToolbarController.Update()
        }

        Selection.activeGameObject = barGO;
        Debug.Log("Farm Toolbar built. Assign LiberationSans SDF to TMP labels if needed.");
    }
}
