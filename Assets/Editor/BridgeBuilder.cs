using UnityEditor;
using UnityEngine;

public static class BridgeBuilder
{
    [MenuItem("Florestia/Build Bridge")]
    static void Build()
    {
        var bridge = new GameObject("Bridge");
        bridge.transform.position = new Vector3(2.75f, -1.65f, 0f);
        bridge.transform.localScale = new Vector3(2.2f, 1f, 1f);

        var sr = bridge.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Terrain/terrain_bridge.png")
            ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Terrain/terrain_grass.png");
        sr.sortingOrder = 2;

        var collider = bridge.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(2.2f, 0.6f);

        bridge.AddComponent<BridgeTrigger>();
        Undo.RegisterCreatedObjectUndo(bridge, "Build Bridge");
    }
}
