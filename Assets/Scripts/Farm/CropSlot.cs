using UnityEngine;

public class CropSlot : MonoBehaviour
{
    public int SlotIndex;

    [SerializeField] Sprite soilSprite;
    [SerializeField] Sprite soilWateredSprite;

    CropData _crop;
    int _daysPlanted;
    bool _wateredToday;

    SpriteRenderer _soilRenderer;  // on this GO — soil / watered soil
    SpriteRenderer _cropRenderer;  // child GO — crop growth stage

    public bool IsEmpty  => _crop == null;
    public bool IsReady  => _crop != null && _daysPlanted >= _crop.growthDays;
    public bool NeedsWater => _crop != null && !_wateredToday && !IsReady;
    public CropData Crop => _crop;

    void Awake()
    {
        _soilRenderer = GetComponent<SpriteRenderer>();

        var cropGO = new GameObject("CropSprite");
        cropGO.transform.SetParent(transform, false);
        _cropRenderer = cropGO.AddComponent<SpriteRenderer>();
        _cropRenderer.sortingOrder = _soilRenderer.sortingOrder + 1;

        var col = GetComponent<BoxCollider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.9f, 0.9f);
    }

    void Start() => RefreshSprite();

    public bool TryPlant(CropData crop)
    {
        if (!IsEmpty) return false;
        if (!StaminaSystem.Instance.TrySpend(crop.staminaCostToPlant)) return false;
        if (!GameManager.Instance.CanAfford(crop.seedCost)) return false;

        GameManager.Instance.SpendBalance(crop.seedCost);
        _crop = crop;
        _daysPlanted = 0;
        _wateredToday = false;
        RefreshSprite();
        return true;
    }

    public bool TryWater()
    {
        if (IsEmpty || _wateredToday || IsReady) return false;
        if (!StaminaSystem.Instance.TrySpend(_crop.staminaCostToWater)) return false;

        _wateredToday = true;
        RefreshSprite();
        return true;
    }

    public CropData TryHarvest()
    {
        if (!IsReady) return null;
        if (!StaminaSystem.Instance.TrySpend(_crop.staminaCostToHarvest)) return null;

        CropData harvested = _crop;
        _crop = null;
        _daysPlanted = 0;
        _wateredToday = false;
        RefreshSprite();
        return harvested;
    }

    public void OnDayEnd()
    {
        if (_crop == null) return;
        if (_wateredToday) _daysPlanted++;
        _wateredToday = false;
        RefreshSprite();
    }

    void RefreshSprite()
    {
        // Soil layer — always visible; darken when watered
        if (_soilRenderer != null)
        {
            bool showWatered = _wateredToday && _crop != null && soilWateredSprite != null;
            _soilRenderer.sprite = showWatered ? soilWateredSprite : soilSprite;
        }

        // Crop layer
        if (_cropRenderer != null)
        {
            if (_crop == null)
            {
                _cropRenderer.sprite = null;
            }
            else
            {
                int stage = Mathf.Min(_daysPlanted, _crop.growthStageSprites.Length - 1);
                _cropRenderer.sprite = _crop.growthStageSprites[stage];
            }
        }
    }

    public void Interact()
    {
        if (ToolbarController.Instance == null) return;

        switch (ToolbarController.Instance.Selected)
        {
            case ToolType.Mandioca:
            case ToolType.Cacau:
            case ToolType.Acai:
                CropData crop = CropSystem.Instance?.GetCropData(ToolbarController.Instance.Selected);
                if (crop != null) TryPlant(crop);
                break;
            case ToolType.Water:
                TryWater();
                break;
            case ToolType.Harvest:
                CropData harvested = TryHarvest();
                if (harvested != null) InventorySystem.Instance?.AddCrop(harvested.cropName);
                break;
        }
    }

    public CropSlotSaveData GetSaveData() => new CropSlotSaveData
    {
        slotIndex    = SlotIndex,
        cropType     = _crop != null ? _crop.cropName : "",
        daysPlanted  = _daysPlanted,
        isWatered    = _wateredToday
    };

    public void LoadSaveData(CropSlotSaveData data, CropData resolvedCrop)
    {
        _crop        = resolvedCrop;
        _daysPlanted = data.daysPlanted;
        _wateredToday = data.isWatered;
        RefreshSprite();
    }
}
