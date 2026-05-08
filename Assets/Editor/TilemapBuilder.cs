using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.IO;

public static class TilemapBuilder
{
    public const float CellSize = 1f; // one 32px sprite = one world unit

    [MenuItem("Florestia/Build Grass Tilemap")]
    public static void Build()
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

        var grassTile = GetOrCreateTile($"{tileDir}/GrassTile.asset", grassSprite);

        Sprite soilSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Terrain/terrain_soil.png");
        Sprite pathSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Terrain/terrain_path.png");
        var pathTile = soilSprite != null
            ? GetOrCreateTile($"{tileDir}/PathTile.asset", pathSprite != null ? pathSprite : soilSprite)
            : grassTile;

        // Grid root
        var gridGO = new GameObject("WorldGrid");
        Undo.RegisterCreatedObjectUndo(gridGO, "Build Grass Tilemap");
        var grid = gridGO.AddComponent<Grid>();
        grid.cellSize = new Vector3(CellSize, CellSize, 0f);
        grid.cellLayout = GridLayout.CellLayout.Rectangle;

        var ground = MakeTilemap(gridGO.transform, "Ground", -20);
        var path = MakeTilemap(gridGO.transform, "Paths", -19);

        for (int x = -10; x <= 14; x++)
            for (int y = -7; y <= 11; y++)
                ground.SetTile(new Vector3Int(x, y, 0), grassTile);

        // Warm dirt walkways make the farm read as a place instead of a debug grid.
        for (int x = -3; x <= 6; x++)
            path.SetTile(new Vector3Int(x, -2, 0), pathTile);
        for (int y = -2; y <= 6; y++)
            path.SetTile(new Vector3Int(-2, y, 0), pathTile);
        for (int x = -4; x <= 0; x++)
            for (int y = 5; y <= 7; y++)
                path.SetTile(new Vector3Int(x, y, 0), pathTile);

        Selection.activeGameObject = gridGO;

        Debug.Log("Polished grass/path tilemap built (WorldGrid). Farm tiles sit on top at sorting order 0+.");
    }

    static Tile GetOrCreateTile(string path, Sprite sprite)
    {
        var tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            AssetDatabase.CreateAsset(tile, path);
        }

        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.None;
        EditorUtility.SetDirty(tile);
        AssetDatabase.SaveAssets();
        return tile;
    }

    static Tilemap MakeTilemap(Transform parent, string name, int sortingOrder)
    {
        var tilemapGO = new GameObject(name);
        tilemapGO.transform.SetParent(parent, false);
        var tilemap = tilemapGO.AddComponent<Tilemap>();
        var tilemapRenderer = tilemapGO.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingOrder = sortingOrder;
        Undo.RegisterCreatedObjectUndo(tilemapGO, "Build Grass Tilemap");
        return tilemap;
    }
}
