using UnityEditor;
using UnityEngine;

public static class HouseBuilder
{
    [MenuItem("Florestia/Build Farm House")]
    static void Build()
    {
        var house = new GameObject("House");
        house.transform.position = new Vector3(-0.55f, 6.6f, 0f);
        house.transform.localScale = new Vector3(2.2f, 2.2f, 1f);

        var sr = house.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/ui_house.png");
        sr.sortingOrder = 3;

        var collider = house.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.size = new Vector2(1.8f, 1.8f);

        house.AddComponent<HouseObstacle>();
        Undo.RegisterCreatedObjectUndo(house, "Build Farm House");
    }
}
