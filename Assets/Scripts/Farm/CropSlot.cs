using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
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
    SpriteRenderer _waterOverlayRenderer; // child GO — clear watered feedback above growth art

    public bool IsEmpty  => _crop == null;
    public bool IsReady  => _crop != null && _daysPlanted >= _crop.growthDays;
    public bool NeedsWater => _crop != null && !_wateredToday && !IsReady;
    public CropData Crop => _crop;

    void Awake()
    {
        _soilRenderer = GetComponent<SpriteRenderer>();
        if (_soilRenderer == null)
        {
            Debug.LogError($"CropSlot '{name}' needs a SpriteRenderer.");
            enabled = false;
            return;
        }

        var cropGO = new GameObject("CropSprite");
        cropGO.transform.SetParent(transform, false);
        _cropRenderer = cropGO.AddComponent<SpriteRenderer>();
        _cropRenderer.sortingOrder = _soilRenderer.sortingOrder + 2;

        var waterGO = new GameObject("WateredOverlay");
        waterGO.transform.SetParent(transform, false);
        _waterOverlayRenderer = waterGO.AddComponent<SpriteRenderer>();
        _waterOverlayRenderer.sortingOrder = _soilRenderer.sortingOrder + 3;
        _waterOverlayRenderer.color = new Color(0.85f, 0.95f, 1f, 0.62f);

        if (!TryGetComponent(out BoxCollider2D col) || col == null)
            col = gameObject.AddComponent<BoxCollider2D>();

        if (col == null)
        {
            Debug.LogError($"CropSlot '{name}' could not create a BoxCollider2D.");
            enabled = false;
            return;
        }

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
        RefreshSprite();
        return true;
    }

    public bool TryWater()
    {
        if (_wateredToday || IsReady) return false;

        int staminaCost = _crop != null ? _crop.staminaCostToWater : 1;
        if (!StaminaSystem.Instance.TrySpend(staminaCost)) return false;

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
        if (_crop != null && _wateredToday) _daysPlanted++;
        _wateredToday = false;
        RefreshSprite();
    }

    void RefreshSprite()
    {
        // Soil layer — always visible; darken when watered
        if (_soilRenderer != null)
        {
            bool showWatered = _wateredToday && soilWateredSprite != null;
            _soilRenderer.sprite = showWatered ? soilWateredSprite : soilSprite;
        }

        if (_waterOverlayRenderer != null)
        {
            bool showOverlay = _wateredToday && soilWateredSprite != null;
            _waterOverlayRenderer.sprite = showOverlay ? soilWateredSprite : null;
            _waterOverlayRenderer.gameObject.SetActive(showOverlay);
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
