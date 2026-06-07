using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RestoreDefaultFont
{
    const string LiberationPath =
        "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

    [MenuItem("Florestia/Restore Default Font (Active Scene)")]
    public static void RestoreActive()
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(LiberationPath);
        if (font == null)
        {
            Debug.LogError($"LiberationSans SDF not found at {LiberationPath}");
            return;
        }

        int updated = RestoreInOpenScenes(font);
        Debug.Log($"Restored default font on {updated} label(s) across open scene(s).");
    }

    [MenuItem("Florestia/Restore Default Font (All Scenes)")]
    public static void RestoreAllScenes()
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(LiberationPath);
        if (font == null)
        {
            Debug.LogError($"LiberationSans SDF not found at {LiberationPath}");
            return;
        }

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        int totalUpdated = 0;
        int scenesTouched = 0;

        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.StartsWith("Assets/")) continue;

            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            int updated = RestoreInScene(scene, font);
            if (updated > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                scenesTouched++;
                totalUpdated += updated;
            }
        }

        Debug.Log($"Restored default font on {totalUpdated} label(s) across {scenesTouched} scene(s).");
    }

    static int RestoreInOpenScenes(TMP_FontAsset font)
    {
        int updated = 0;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            int n = RestoreInScene(scene, font);
            if (n > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                updated += n;
            }
        }
        return updated;
    }

    static int RestoreInScene(Scene scene, TMP_FontAsset font)
    {
        int updated = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
            {
                Undo.RecordObject(text, "Restore Default Font");
                text.font = font;
                EditorUtility.SetDirty(text);
                updated++;
            }
        }
        return updated;
    }
}
