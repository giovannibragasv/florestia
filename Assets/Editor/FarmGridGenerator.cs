using UnityEngine;
using UnityEditor;

public static class FarmGridGenerator
{
    const float CellSize = 1f;

    [MenuItem("Florestia/Generate 6x6 Farm Grid")]
    public static void GenerateGrid()
    {
        foreach (var oldSlot in Object.FindObjectsByType<CropSlot>(FindObjectsSortMode.None))
        {
            if (oldSlot != null)
                Undo.DestroyObjectImmediate(oldSlot.gameObject);
        }

        var existing = GameObject.Find("FarmGrid");
        if (existing != null) Undo.DestroyObjectImmediate(existing);

        Sprite soilSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Terrain/terrain_soil.png");
        Sprite soilWateredSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Terrain/terrain_soil_watered.png");

        var cropSystem = GameObject.Find("_CropSystem");
        CropSystem cs = cropSystem != null ? cropSystem.GetComponent<CropSystem>() : null;
        SerializedObject csSO = cs != null ? new SerializedObject(cs) : null;
        SerializedProperty slotsProp = csSO?.FindProperty("slots");
        if (slotsProp != null) slotsProp.arraySize = 36;

        GameObject parent = new GameObject("FarmGrid");
        Undo.RegisterCreatedObjectUndo(parent, "Generate Farm Grid");

        int index = 0;
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                GameObject slot = new GameObject($"CropSlot_{index:D2}");
                slot.transform.SetParent(parent.transform);
                slot.transform.localPosition = new Vector3(col * CellSize, row * CellSize, 0f);

                SpriteRenderer sr = slot.AddComponent<SpriteRenderer>();
                sr.sprite = soilSprite != null ? soilSprite : null;
                if (soilSprite == null) sr.color = new Color(0.55f, 0.35f, 0.15f, 1f);
                sr.sortingOrder = 0;

                var collider = slot.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = new Vector2(0.9f, 0.9f);

                CropSlot cropSlot = slot.AddComponent<CropSlot>();
                cropSlot.SlotIndex = index;

                // Assign soil sprites to CropSlot serialized fields
                SerializedObject slotSO = new SerializedObject(cropSlot);
                slotSO.FindProperty("soilSprite").objectReferenceValue = soilSprite;
                slotSO.FindProperty("soilWateredSprite").objectReferenceValue = soilWateredSprite;
                slotSO.ApplyModifiedPropertiesWithoutUndo();

                if (slotsProp != null)
                    slotsProp.GetArrayElementAtIndex(index).objectReferenceValue = cropSlot;

                Undo.RegisterCreatedObjectUndo(slot, "Generate Farm Grid");
                index++;
            }
        }

        csSO?.ApplyModifiedPropertiesWithoutUndo();
        Selection.activeGameObject = parent;
        EditorUtility.SetDirty(parent);

        string soilMsg  = soilSprite        != null ? "soil sprite assigned."        : "No terrain_soil — assign manually.";
        string waterMsg = soilWateredSprite != null ? "watered sprite assigned."     : "No terrain_soil_watered — assign manually.";
        string csMsg    = cs               != null ? "_CropSystem slots auto-wired." : "_CropSystem not found — assign manually.";
        Debug.Log($"Farm grid generated: 36 CropSlots. {soilMsg} {waterMsg} {csMsg}");
    }
}
