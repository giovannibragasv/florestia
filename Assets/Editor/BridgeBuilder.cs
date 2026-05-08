using UnityEditor;
using UnityEngine;

public static class BridgeBuilder
{
    [MenuItem("Florestia/Build Bridge")]
    public static void Build()
    {
        var existing = GameObject.Find("Bridge");
        if (existing != null) Undo.DestroyObjectImmediate(existing);

        var bridge = new GameObject("Bridge");
        bridge.transform.position = new Vector3(2.5f, -2.15f, 0f);
        bridge.transform.localScale = new Vector3(2f, 1f, 1f);

        var sr = bridge.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Terrain/terrain_bridge.png")
            ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Terrain/terrain_grass.png");
        sr.sortingOrder = 2;

        var collider = bridge.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(2f, 0.65f);

        bridge.AddComponent<BridgeTrigger>();
        Undo.RegisterCreatedObjectUndo(bridge, "Build Bridge");
    }
}
