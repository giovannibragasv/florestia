using UnityEngine;
using UnityEditor;

public static class FarmGridGenerator
{
    [MenuItem("Florestia/Generate 6x6 Farm Grid")]
    static void GenerateGrid()
    {
        // Remove any existing FarmGrid
        var existing = GameObject.Find("FarmGrid");
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing);
        }

        Sprite soilSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Sprites/Terrain/terrain_soil.png");

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
                slot.transform.localPosition = new Vector3(col * 1.1f, row * 1.1f, 0f);

                SpriteRenderer sr = slot.AddComponent<SpriteRenderer>();
                if (soilSprite != null)
                    sr.sprite = soilSprite;
                else
                    sr.color = new Color(0.55f, 0.35f, 0.15f, 1f);

                CropSlot cropSlot = slot.AddComponent<CropSlot>();
                cropSlot.SlotIndex = index;

                if (slotsProp != null)
                    slotsProp.GetArrayElementAtIndex(index).objectReferenceValue = cropSlot;

                Undo.RegisterCreatedObjectUndo(slot, "Generate Farm Grid");
                index++;
            }
        }

        csSO?.ApplyModifiedPropertiesWithoutUndo();
        Selection.activeGameObject = parent;
        EditorUtility.SetDirty(parent);

        string spriteMsg = soilSprite != null ? "terrain_soil sprite assigned." : "No terrain_soil found — assign sprites manually.";
        string csMsg = cs != null ? "_CropSystem Slots auto-wired." : "_CropSystem not found — assign slots manually.";
        Debug.Log($"Farm grid generated: 36 CropSlot objects under FarmGrid. {spriteMsg} {csMsg}");
    }
}
