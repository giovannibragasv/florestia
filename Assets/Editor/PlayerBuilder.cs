using UnityEngine;
using UnityEditor;

public static class PlayerBuilder
{
    [MenuItem("Florestia/Build Player")]
    static void Build()
    {
        // Remove existing player
        var existing = Object.FindFirstObjectByType<PlayerController>();
        if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);

        var playerGO = new GameObject("Player");
        Undo.RegisterCreatedObjectUndo(playerGO, "Build Player");

        // Start just south of the farm grid center (grid center X = 2.75)
        playerGO.transform.position = new Vector3(2.75f, -0.8f, 0f);

        // Rigidbody2D — top-down, no gravity
        var rb = playerGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Collider (non-trigger — will collide with walls/fences later)
        var col = playerGO.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.55f, 0.55f);

        // SpriteRenderer — light blue placeholder until sprite is assigned
        var sr = playerGO.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.45f, 0.72f, 1f);
        sr.sortingOrder = 10; // render above tiles and crops

        // PlayerController
        playerGO.AddComponent<PlayerController>();

        Selection.activeGameObject = playerGO;
        Debug.Log(
            "Player built at (2.75, -0.8). " +
            "Assign your player_character.png sprite to the SpriteRenderer. " +
            "WASD to move, E to interact with the tile you're facing.");
    }
}
