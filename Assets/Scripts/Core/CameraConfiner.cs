using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraConfiner : MonoBehaviour
{
    [SerializeField] Vector2 minBounds = new Vector2(-1f, -2.5f);
    [SerializeField] Vector2 maxBounds = new Vector2(7f, 7.5f);

    Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        float halfH = _camera.orthographicSize;
        float halfW = halfH * _camera.aspect;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x + halfW, maxBounds.x - halfW);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + halfH, maxBounds.y - halfH);
        transform.position = pos;
    }
}
