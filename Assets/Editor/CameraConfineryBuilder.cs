using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CameraConfineryBuilder
{
    [MenuItem("Florestia/Add Camera Confiner")]
    public static void Add()
    {
        Camera camera = Camera.main;
        if (camera == null) return;

        Undo.RecordObject(camera, "Polish Farm Camera");
        camera.orthographic = true;
        camera.orthographicSize = 3.35f;
        camera.backgroundColor = new Color(0.32f, 0.52f, 0.38f, 1f);
        camera.transform.position = new Vector3(2f, 2.5f, -10f);

        var confiner = camera.GetComponent<CameraConfiner>() ??
            Undo.AddComponent<CameraConfiner>(camera.gameObject);

        SerializedObject so = new SerializedObject(confiner);
        so.FindProperty("minBounds").vector2Value = new Vector2(-4f, -3.25f);
        so.FindProperty("maxBounds").vector2Value = new Vector2(8f, 8.25f);
        so.FindProperty("followDamping").floatValue = 8f;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
