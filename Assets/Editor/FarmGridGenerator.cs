using UnityEngine;
using UnityEditor;

public static class FarmGridGenerator
{
    [MenuItem("Florestia/Generate 6x6 Farm Grid")]
    static void GenerateGrid()
    {
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
                sr.color = new Color(0.55f, 0.35f, 0.15f, 1f); // brown placeholder

                CropSlot cs = slot.AddComponent<CropSlot>();
                cs.SlotIndex = index;

                Undo.RegisterCreatedObjectUndo(slot, "Generate Farm Grid");
                index++;
            }
        }

        Selection.activeGameObject = parent;
        EditorUtility.SetDirty(parent);
        Debug.Log("Farm grid generated: 36 CropSlot objects under FarmGrid.");
    }
}
