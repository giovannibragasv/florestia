using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FontAssigner
{
    [MenuItem("Florestia/Assign TMP Fonts")]
    static void Assign()
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (font == null) return;

        int updated = 0;
        foreach (TextMeshProUGUI text in Object.FindObjectsByType<TextMeshProUGUI>(
            FindObjectsInactive.Include))
            if (AssignFont(text, font)) updated++;

        foreach (TextMeshPro text in Object.FindObjectsByType<TextMeshPro>(
            FindObjectsInactive.Include))
            if (AssignFont(text, font)) updated++;

        if (updated > 0)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log($"Assigned TMP font to {updated} label(s).");
    }

    static bool AssignFont(TMP_Text text, TMP_FontAsset font)
    {
        if (text.font != null) return false;

        Undo.RecordObject(text, "Assign TMP Font");
        text.font = font;
        EditorUtility.SetDirty(text);
        return true;
    }
}
