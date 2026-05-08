using UnityEditor;
using UnityEngine;

public static class HouseBuilder
{
    [MenuItem("Florestia/Build Farm House")]
    public static void Build()
    {
        var existing = GameObject.Find("House");
        if (existing != null) Undo.DestroyObjectImmediate(existing);

        var house = new GameObject("House");
        house.transform.position = new Vector3(-2f, 5.85f, 0f);
        house.transform.localScale = new Vector3(1.15f, 1.15f, 1f);

        var sr = house.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/ui_house.png");
        sr.sortingOrder = 8;

        var collider = house.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.offset = new Vector2(0f, -0.9f);
        collider.size = new Vector2(2.1f, 1.1f);

        house.AddComponent<HouseObstacle>();
        Undo.RegisterCreatedObjectUndo(house, "Build Farm House");
    }
}
