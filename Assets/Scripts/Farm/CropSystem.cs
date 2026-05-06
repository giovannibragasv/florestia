using UnityEngine;

public class CropSystem : MonoBehaviour
{
    public static CropSystem Instance { get; private set; }

    [SerializeField] CropSlot[] slots;
    [SerializeField] CropData[] cropCatalog; // assign Mandioca, Cacau, Acai in Inspector

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnDayEnd()
    {
        foreach (var slot in slots) slot.OnDayEnd();
    }

    public CropSlotSaveData[] GetSaveData()
    {
        var data = new CropSlotSaveData[slots.Length];
        for (int i = 0; i < slots.Length; i++) data[i] = slots[i].GetSaveData();
        return data;
    }

    public void LoadSaveData(CropSlotSaveData[] data)
    {
        if (data == null) return;
        foreach (var slotData in data)
        {
            if (slotData.slotIndex >= slots.Length) continue;
            CropData crop = string.IsNullOrEmpty(slotData.cropType)
                ? null
                : System.Array.Find(cropCatalog, c => c.cropName == slotData.cropType);
            slots[slotData.slotIndex].LoadSaveData(slotData, crop);
        }
    }
}
