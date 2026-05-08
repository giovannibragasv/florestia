using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraConfiner : MonoBehaviour
{
    [SerializeField] Vector2 minBounds = new Vector2(-4f, -3.25f);
    [SerializeField] Vector2 maxBounds = new Vector2(8f, 8.25f);
    [SerializeField] Transform target;
    [SerializeField] float followDamping = 8f;

    Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null && PlayerController.Instance != null)
            target = PlayerController.Instance.transform;

        if (target != null)
        {
            Vector3 desired = target.position;
            desired.z = transform.position.z;
            transform.position = Vector3.Lerp(
                transform.position, desired, Time.deltaTime * followDamping);
        }

        float halfH = _camera.orthographicSize;
        float halfW = halfH * _camera.aspect;
        Vector3 pos = transform.position;

        float minX = minBounds.x + halfW;
        float maxX = maxBounds.x - halfW;
        float minY = minBounds.y + halfH;
        float maxY = maxBounds.y - halfH;

        pos.x = minX <= maxX ? Mathf.Clamp(pos.x, minX, maxX) : (minBounds.x + maxBounds.x) * 0.5f;
        pos.y = minY <= maxY ? Mathf.Clamp(pos.y, minY, maxY) : (minBounds.y + maxBounds.y) * 0.5f;
        transform.position = pos;
    }
}
