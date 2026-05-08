using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.IO;

public static class TilemapBuilder
{
    const float CellSize = 1.1f; // matches FarmGrid tile spacing

    [MenuItem("Florestia/Build Grass Tilemap")]
    static void Build()
    {
        // Remove existing grid
        var existingGrid = GameObject.Find("WorldGrid");
        if (existingGrid != null) Undo.DestroyObjectImmediate(existingGrid);

        Sprite grassSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Terrain/terrain_grass.png");
        if (grassSprite == null)
        {
            Debug.LogError("terrain_grass.png not found at Assets/Sprites/Terrain/. Import the sprite first.");
            return;
        }

        // Ensure tile asset directory exists
        string tileDir = "Assets/Data/Tiles";
        if (!Directory.Exists(tileDir))
            Directory.CreateDirectory(tileDir);

        // Create grass tile asset
        string tilePath = $"{tileDir}/GrassTile.asset";
        var grassTile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
        if (grassTile == null)
        {
            grassTile = ScriptableObject.CreateInstance<Tile>();
            grassTile.sprite = grassSprite;
            grassTile.colliderType = Tile.ColliderType.None;
            AssetDatabase.CreateAsset(grassTile, tilePath);
            AssetDatabase.SaveAssets();
        }

        // Grid root
        var gridGO = new GameObject("WorldGrid");
        Undo.RegisterCreatedObjectUndo(gridGO, "Build Grass Tilemap");
        var grid = gridGO.AddComponent<Grid>();
        grid.cellSize = new Vector3(CellSize, CellSize, 0f);
        grid.cellLayout = GridLayout.CellLayout.Rectangle;

        // Ground tilemap child
        var tilemapGO = new GameObject("Ground");
        tilemapGO.transform.SetParent(gridGO.transform, false);
        var tilemap = tilemapGO.AddComponent<Tilemap>();
        var tilemapRenderer = tilemapGO.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingOrder = -1; // behind everything

        // Paint grass from (-4,-4) to (9,9) in tile coordinates
        // Farm occupies tiles (0,0) to (5,5) — grass fills the surrounding world
        for (int x = -4; x <= 9; x++)
            for (int y = -4; y <= 9; y++)
                tilemap.SetTile(new Vector3Int(x, y, 0), grassTile);

        Undo.RegisterCreatedObjectUndo(tilemapGO, "Build Grass Tilemap");
        Selection.activeGameObject = gridGO;

        Debug.Log("Grass tilemap built (WorldGrid). Farm tiles (0,0)–(5,5) sit on top at sorting order 0+.");
    }
}
