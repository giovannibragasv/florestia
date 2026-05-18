using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using TMPro;

public static class ToolbarBuilder
{
    static readonly string[] SlotLabels = { "Mandioca", "Cacau", "Açaí", "Água", "Colher" };
    const float SlotSize   = 72f;
    const float SlotGap    = 6f;
    const float BarPadding = 8f;

    [MenuItem("Florestia/Build Farm Toolbar")]
    public static void Build()
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

        var oldBar = ct.Find("Toolbar");
        if (oldBar != null) Undo.DestroyObjectImmediate(oldBar.gameObject);

        var oldController = GameObject.Find("_Toolbar");
        if (oldController != null) Undo.DestroyObjectImmediate(oldController);

        // Toolbar background strip
        float barWidth = 5 * SlotSize + 4 * SlotGap + 2 * BarPadding;
        var barGO = new GameObject("Toolbar");
        barGO.transform.SetParent(ct, false);
        var barRT = barGO.AddComponent<RectTransform>();
        barRT.anchorMin = new Vector2(0.5f, 0f);
        barRT.anchorMax = new Vector2(0.5f, 0f);
        barRT.pivot     = new Vector2(0.5f, 0f);
        barRT.anchoredPosition = new Vector2(0f, 18f);
        barRT.sizeDelta = new Vector2(barWidth, SlotSize + 2 * BarPadding);
        barGO.AddComponent<Image>().color = new Color(0.60f, 0.30f, 0.08f, 0.96f);
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
            bg.color = new Color(0.96f, 0.68f, 0.34f, 0.96f);
            slotBackgrounds[i] = bg;
            Undo.RegisterCreatedObjectUndo(slotGO, "Build Farm Toolbar");

            var iconGO = new GameObject("Icon");
            iconGO.transform.SetParent(slotRT, false);
            var iconRT = iconGO.AddComponent<RectTransform>();
            iconRT.anchorMin = new Vector2(0.5f, 0.5f);
            iconRT.anchorMax = new Vector2(0.5f, 0.5f);
            iconRT.pivot = new Vector2(0.5f, 0.5f);
            iconRT.sizeDelta = new Vector2(48f, 48f);
            iconRT.anchoredPosition = new Vector2(0f, -2f);
            var icon = iconGO.AddComponent<Image>();
            icon.sprite = LoadSlotIcon(i);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Undo.RegisterCreatedObjectUndo(iconGO, "Build Farm Toolbar");

            var keyGO = new GameObject("Key");
            keyGO.transform.SetParent(slotRT, false);
            var keyRT = keyGO.AddComponent<RectTransform>();
            keyRT.anchorMin = new Vector2(0f, 1f);
            keyRT.anchorMax = new Vector2(0f, 1f);
            keyRT.pivot = new Vector2(0f, 1f);
            keyRT.sizeDelta = new Vector2(20f, 18f);
            keyRT.anchoredPosition = new Vector2(5f, -4f);
            var key = keyGO.AddComponent<TextMeshProUGUI>();
            key.text = (i + 1).ToString();
            key.fontSize = 12;
            key.fontStyle = FontStyles.Bold;
            key.alignment = TextAlignmentOptions.Left;
            key.color = new Color(0.62f, 0.17f, 0.06f, 1f);
            key.raycastTarget = false;
            Undo.RegisterCreatedObjectUndo(keyGO, "Build Farm Toolbar");

            var lblGO = new GameObject("Label");
            lblGO.transform.SetParent(slotRT, false);
            lblGO.SetActive(false);
            var lblRT = lblGO.AddComponent<RectTransform>();
            lblRT.anchorMin = new Vector2(0f, 0f);
            lblRT.anchorMax = new Vector2(1f, 0f);
            lblRT.pivot = new Vector2(0.5f, 0f);
            lblRT.offsetMin = new Vector2(2f, 3f);
            lblRT.offsetMax = new Vector2(-2f, 21f);
            var tmp = lblGO.AddComponent<TextMeshProUGUI>();
            tmp.text = SlotLabels[i];
            tmp.fontSize = 9;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.28f, 0.10f, 0.02f, 1f);
            tmp.raycastTarget = false;
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
            if (i == 0) UnityEventTools.AddPersistentListener(btn.onClick, tc.SelectMandioca);
            else if (i == 1) UnityEventTools.AddPersistentListener(btn.onClick, tc.SelectCacau);
            else if (i == 2) UnityEventTools.AddPersistentListener(btn.onClick, tc.SelectAcai);
            else if (i == 3) UnityEventTools.AddPersistentListener(btn.onClick, tc.SelectWater);
            else UnityEventTools.AddPersistentListener(btn.onClick, tc.SelectHarvest);
        }

        Selection.activeGameObject = barGO;
        Debug.Log("Farm Toolbar built. Assign LiberationSans SDF to TMP labels if needed.");
    }

    static Sprite LoadSlotIcon(int index)
    {
        string path = index switch
        {
            0 => "Assets/Sprites/Crops/Mandioca/crop_mandioca_02.png",
            1 => "Assets/Sprites/Crops/Cacau/crop_cacau_04.png",
            2 => "Assets/Sprites/Crops/Acai/crop_acai_06.png",
            3 => "Assets/Sprites/UI/ui_watering_can.png",
            _ => "Assets/Sprites/UI/ui_coin.png"
        };

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
