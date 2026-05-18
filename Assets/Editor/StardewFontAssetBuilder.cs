using TMPro;
using UnityEditor;
using UnityEngine;

public static class StardewFontAssetBuilder
{
    const string SourceFontPath = "Assets/Fonts/Stardew_Valley.ttf";
    const string OutputFontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/Stardew_Valley SDF.asset";

    [MenuItem("Florestia/Build SDV TMP Font")]
    public static void Build()
    {
        Font source = AssetDatabase.LoadAssetAtPath<Font>(SourceFontPath);
        if (source == null)
        {
            Debug.LogWarning($"Fonte não encontrada: {SourceFontPath}");
            return;
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            source,
            64,
            9,
            UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,
            1024,
            1024,
            AtlasPopulationMode.Dynamic);

        fontAsset.name = "Stardew_Valley SDF";

        TMP_FontAsset fallback = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (fallback != null && !fontAsset.fallbackFontAssetTable.Contains(fallback))
            fontAsset.fallbackFontAssetTable.Add(fallback);

        if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(OutputFontPath) != null)
            AssetDatabase.DeleteAsset(OutputFontPath);

        AssetDatabase.CreateAsset(fontAsset, OutputFontPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        FontAssigner.Assign();
        Debug.Log($"TMP font criado: {OutputFontPath}");
    }
}
