using UnityEngine;
using UnityEditor;

public static class PlayerBuilder
{
    [MenuItem("Florestia/Build Player")]
    static void Build()
    {
        var existing = Object.FindFirstObjectByType<PlayerController>();
        if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);

        var playerGO = new GameObject("Player");
        Undo.RegisterCreatedObjectUndo(playerGO, "Build Player");
        playerGO.transform.position = new Vector3(2.75f, -0.8f, 0f);

        var rb = playerGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = playerGO.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.55f, 0.55f);

        var sr = playerGO.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.45f, 0.72f, 1f); // blue placeholder
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
        so.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeGameObject = playerGO;
        Debug.Log(
            "Player built. Assign your player sprite to the SpriteRenderer. " +
            "WASD to move, E to interact with the highlighted tile.");
    }
}
