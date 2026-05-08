using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CameraConfineryBuilder
{
    [MenuItem("Florestia/Add Camera Confiner")]
    static void Add()
    {
        Camera camera = Camera.main;
        if (camera == null) return;

        if (camera.GetComponent<CameraConfiner>() == null)
        {
            Undo.AddComponent<CameraConfiner>(camera.gameObject);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
