using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class FarmScenePolishBuilder
{
    [MenuItem("Florestia/Apply Farm Scene Polish")]
    public static void Apply()
    {
        ConfigureSpriteImports();
        EnsureCanvas();
        EnsureEventSystem();

        TilemapBuilder.Build();
        FarmGridGenerator.GenerateGrid();
        HouseBuilder.Build();
        BridgeBuilder.Build();
        PlayerBuilder.Build();
        HUDBuilder.Build();
        ToolbarBuilder.Build();
        EnsureDayNightUI();
        CameraConfineryBuilder.Add();
        FontAssigner.Assign();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("FarmScene polish applied: terrain scale, camera, house, bridge, HUD, toolbar, and day/night UI rebuilt.");
    }

    static void ConfigureSpriteImports()
    {
        string[] spritePaths =
        {
            "Assets/Sprites/Terrain/terrain_grass.png",
            "Assets/Sprites/Terrain/terrain_soil.png",
            "Assets/Sprites/Terrain/terrain_soil_watered.png",
            "Assets/Sprites/Terrain/terrain_bridge.png",
            "Assets/Sprites/UI/ui_house.png",
            "Assets/Sprites/UI/ui_coin.png",
            "Assets/Sprites/UI/ui_stamina.png",
            "Assets/Sprites/UI/ui_sun.png",
            "Assets/Sprites/UI/ui_warning.png"
        };

        foreach (string path in spritePaths)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (!Mathf.Approximately(importer.spritePixelsPerUnit, 32f))
            {
                importer.spritePixelsPerUnit = 32f;
                changed = true;
            }
            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (changed)
                importer.SaveAndReimport();
        }
    }

    static Canvas EnsureCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var cgo = new GameObject("Canvas");
            canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cgo.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(cgo, "Apply Farm Scene Polish");
        }

        var scaler = canvas.GetComponent<CanvasScaler>() ?? canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
        Undo.RegisterCreatedObjectUndo(eventSystem, "Apply Farm Scene Polish");
    }

    static void EnsureDayNightUI()
    {
        Canvas canvas = EnsureCanvas();
        Transform ct = canvas.transform;

        var sky = ct.Find("SkyOverlay");
        if (sky == null)
        {
            var skyGO = new GameObject("SkyOverlay");
            skyGO.transform.SetParent(ct, false);
            sky = skyGO.transform;
            var rt = skyGO.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var image = skyGO.AddComponent<Image>();
            image.color = new Color(0.53f, 0.81f, 0.98f, 0f);
            image.raycastTarget = false;
            Undo.RegisterCreatedObjectUndo(skyGO, "Apply Farm Scene Polish");
        }
        sky.SetAsFirstSibling();

        var warning = ct.Find("NightWarningPanel");
        if (warning == null)
        {
            var warningGO = new GameObject("NightWarningPanel");
            warningGO.transform.SetParent(ct, false);
            warning = warningGO.transform;

            var rt = warningGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -22f);
            rt.sizeDelta = new Vector2(560f, 58f);

            var bg = warningGO.AddComponent<Image>();
            bg.color = new Color(0.22f, 0.08f, 0.04f, 0.9f);

            var textGO = new GameObject("Label");
            textGO.transform.SetParent(warning, false);
            var textRT = textGO.AddComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = new Vector2(16f, 8f);
            textRT.offsetMax = new Vector2(-16f, -8f);
            var label = textGO.AddComponent<TextMeshProUGUI>();
            label.text = "A noite chegou. Termine as ações e vá ao mercado.";
            label.fontSize = 22f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(1f, 0.88f, 0.55f, 1f);

            Undo.RegisterCreatedObjectUndo(warningGO, "Apply Farm Scene Polish");
            warningGO.SetActive(false);
        }

        var cycle = Object.FindFirstObjectByType<DayNightCycle>();
        if (cycle == null) return;

        SerializedObject so = new SerializedObject(cycle);
        so.FindProperty("skyOverlay").objectReferenceValue = sky.GetComponent<Image>();
        so.FindProperty("nightWarningPanel").objectReferenceValue = warning.gameObject;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
