using UnityEngine;

public static class SceneCameraUtility
{
    public static Camera EnsureUICamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
            camera = Object.FindAnyObjectByType<Camera>();

        if (camera != null) return camera;

        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        cameraGO.transform.position = new Vector3(0f, 0f, -10f);

        camera = cameraGO.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.07f, 0.06f, 1f);

        if (Object.FindAnyObjectByType<AudioListener>() == null)
            cameraGO.AddComponent<AudioListener>();

        return camera;
    }
}
