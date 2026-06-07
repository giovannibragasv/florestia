using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;

public static class DogicaFontInstaller
{
    const string FolderPath = "Assets/Fonts/Dogica";
    const string PixelTTF = "Assets/Fonts/Dogica/TTF/dogicapixel.ttf";
    const string PixelBoldTTF = "Assets/Fonts/Dogica/TTF/dogicapixelbold.ttf";
    const string PixelAsset = "Assets/Fonts/Dogica/Dogica Pixel SDF.asset";
    const string PixelBoldAsset = "Assets/Fonts/Dogica/Dogica Pixel Bold SDF.asset";
    const string LiberationPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

    // Charset pré-baked no atlas estático para não disparar TryAddCharacterInternal
    // em runtime (que crashava com MissingReferenceException de m_AtlasTextures).
    // Inclui ASCII + acentos PT-BR + glifo F$ da moeda fictícia + sinais comuns.
    const string PortugueseCharset =
        " !\"#$%&'()*+,-./0123456789:;<=>?@" +
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`" +
        "abcdefghijklmnopqrstuvwxyz{|}~" +
        "ÀÁÂÃÄÇÈÉÊËÌÍÎÏÒÓÔÕÖÙÚÛÜÝ" +
        "àáâãäçèéêëìíîïòóôõöùúûüý" +
        "ÑñºªÇç§·•←→↑↓×÷±°…«»";

    [MenuItem("Florestia/Fonts/1. Install Dogica TMP Assets")]
    public static void Install()
    {
        bool ok = BuildAsset(PixelTTF, PixelAsset);
        ok &= BuildAsset(PixelBoldTTF, PixelBoldAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        if (ok) Debug.Log($"Dogica TMP assets criados em {FolderPath}/ com charset PT-BR pré-baked.");
    }

    static bool BuildAsset(string ttfPath, string assetPath)
    {
        Font font = AssetDatabase.LoadAssetAtPath<Font>(ttfPath);
        if (font == null)
        {
            Debug.LogError($"Dogica: source TTF missing at {ttfPath}");
            return false;
        }

        // Garante que .meta antigo / sub-assets quebrados não fiquem pendurados.
        if (File.Exists(assetPath))
            AssetDatabase.DeleteAsset(assetPath);

        TMP_FontAsset asset = TMP_FontAsset.CreateFontAsset(
            font,
            samplingPointSize: 32,
            atlasPadding: 4,
            renderMode: GlyphRenderMode.SDFAA,
            atlasWidth: 1024,
            atlasHeight: 1024);

        if (asset == null)
        {
            Debug.LogError($"Dogica: CreateFontAsset retornou null para {ttfPath}");
            return false;
        }

        AssetDatabase.CreateAsset(asset, assetPath);

        // CRÍTICO: atlas texture e material precisam ser persistidos como sub-assets,
        // senão o asset salvo aponta para Texture2D em memória que vira null após
        // o próximo domain reload (raiz da MissingReferenceException m_AtlasTextures).
        string baseName = Path.GetFileNameWithoutExtension(assetPath);
        if (asset.atlasTexture != null && AssetDatabase.GetAssetPath(asset.atlasTexture) != assetPath)
        {
            asset.atlasTexture.name = baseName + " Atlas";
            AssetDatabase.AddObjectToAsset(asset.atlasTexture, asset);
        }
        if (asset.material != null && AssetDatabase.GetAssetPath(asset.material) != assetPath)
        {
            asset.material.name = baseName + " Material";
            AssetDatabase.AddObjectToAsset(asset.material, asset);
        }

        // Enquanto ainda está em modo Dinâmico, pré-baka todo o charset PT-BR
        // (acentos, símbolos, F$). Depois trava em Static para o runtime não
        // tentar adicionar glifos novos (e crashar).
        asset.TryAddCharacters(PortugueseCharset, out string missing);
        if (!string.IsNullOrEmpty(missing))
            Debug.LogWarning($"Dogica: caracteres ausentes na fonte (fallback vai cobrir): {missing}");

        asset.atlasPopulationMode = AtlasPopulationMode.Static;

        // Fallback para LiberationSans cobrir qualquer glifo fora do conjunto.
        TMP_FontAsset fallback = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(LiberationPath);
        if (fallback != null)
        {
            asset.fallbackFontAssetTable ??= new List<TMP_FontAsset>();
            asset.fallbackFontAssetTable.Clear();
            asset.fallbackFontAssetTable.Add(fallback);
        }

        EditorUtility.SetDirty(asset);
        if (asset.atlasTexture != null) EditorUtility.SetDirty(asset.atlasTexture);
        if (asset.material != null) EditorUtility.SetDirty(asset.material);

        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(assetPath);
        return true;
    }

    [MenuItem("Florestia/Fonts/2. Apply Dogica Pixel to Active Scene")]
    public static void ApplyToActiveScene()
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PixelAsset);
        if (font == null)
        {
            Debug.LogError("Dogica Pixel asset não encontrado. Rode '1. Install Dogica TMP Assets' antes.");
            return;
        }

        Scene active = SceneManager.GetActiveScene();
        int count = ApplyFontInScene(active, font);
        EditorSceneManager.MarkSceneDirty(active);
        Debug.Log($"Dogica Pixel aplicado em {count} TMP_Text na cena '{active.name}'.");
    }

    [MenuItem("Florestia/Fonts/3. Apply Dogica Pixel to FarmScene + MarketScene + EndScreen")]
    public static void ApplyToAllScenes()
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PixelAsset);
        if (font == null)
        {
            Debug.LogError("Dogica Pixel asset não encontrado. Rode '1. Install Dogica TMP Assets' antes.");
            return;
        }

        string[] scenePaths = {
            "Assets/Scenes/FarmScene.unity",
            "Assets/Scenes/MarketScene.unity",
            "Assets/Scenes/EndScreen.unity",
            "Assets/Scenes/StartScene.unity"
        };

        int total = 0;
        string previousPath = SceneManager.GetActiveScene().path;

        foreach (string path in scenePaths)
        {
            if (!File.Exists(path)) continue;
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            int count = ApplyFontInScene(scene, font);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            total += count;
            Debug.Log($"Dogica: {count} TMP_Text atualizados em {scene.name}");
        }

        if (!string.IsNullOrEmpty(previousPath) && File.Exists(previousPath))
            EditorSceneManager.OpenScene(previousPath, OpenSceneMode.Single);

        Debug.Log($"Dogica Pixel aplicado em {total} TMP_Text em todas as cenas.");
    }

    static int ApplyFontInScene(Scene scene, TMP_FontAsset font)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (var label in root.GetComponentsInChildren<TMP_Text>(true))
            {
                label.font = font;
                EditorUtility.SetDirty(label);
                count++;
            }
        }
        return count;
    }
}
