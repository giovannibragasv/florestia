using UnityEngine;
using UnityEditor;

public static class PlayerBuilder
{
    [MenuItem("Florestia/Build Player")]
    public static void Build()
    {
        var existing = Object.FindFirstObjectByType<PlayerController>();
        if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);

        var playerGO = new GameObject("Player");
        Undo.RegisterCreatedObjectUndo(playerGO, "Build Player");
        playerGO.transform.position = new Vector3(2.5f, -1.15f, 0f);

        var rb = playerGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = playerGO.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.55f, 0.55f);

        var sr = playerGO.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Player/player_walk_down_0.png");
        sr.color = Color.white;
        sr.sortingOrder = 10;

        // Tile highlight child
        Sprite soilSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Terrain/terrain_soil.png");

        var hlGO = new GameObject("TileHighlight");
        hlGO.transform.SetParent(playerGO.transform, false);
        hlGO.SetActive(false);
        var hlSR = hlGO.AddComponent<SpriteRenderer>();
        hlSR.sprite = soilSprite;
        hlSR.color  = new Color(1f, 0.95f, 0.3f, 0.45f); // yellow, semi-transparent
        hlSR.sortingOrder = 5;
        Undo.RegisterCreatedObjectUndo(hlGO, "Build Player");

        var pc = playerGO.AddComponent<PlayerController>();
        SerializedObject so = new SerializedObject(pc);
        so.FindProperty("tileHighlight").objectReferenceValue = hlSR;
        AssignSprites(so.FindProperty("walkDown"),
            "Assets/Sprites/Player/player_walk_down_0.png",
            "Assets/Sprites/Player/player_walk_down_1.png");
        AssignSprites(so.FindProperty("walkUp"),
            "Assets/Sprites/Player/player_walk_up_0.png",
            "Assets/Sprites/Player/player_walk_up_0.png");
        AssignSprites(so.FindProperty("walkSide"),
            "Assets/Sprites/Player/player_walk_side_0.png",
            "Assets/Sprites/Player/player_walk_side_0.png");
        so.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeGameObject = playerGO;
        Debug.Log(
            "Player built. WASD to move, E to interact with the highlighted tile.");
    }

    static void AssignSprites(SerializedProperty prop, string frame0, string frame1)
    {
        prop.arraySize = 2;
        prop.GetArrayElementAtIndex(0).objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Sprite>(frame0);
        prop.GetArrayElementAtIndex(1).objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Sprite>(frame1);
    }
}
