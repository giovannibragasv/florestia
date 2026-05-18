using System.Collections;
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
    SpriteRenderer _growthBarBg;    // child GO — dark background of life bar
    SpriteRenderer _growthBarFill;  // child GO — colored fill scaled by progress
    GameObject _readyMarkerRoot;
    GameObject _actionFeedbackRoot;
    Coroutine _actionFeedbackRoutine;

    const float BarWidth = 0.72f;
    const float BarHeight = 0.10f;
    const float BarYOffset = 0.42f;
    static Sprite _whitePixel;

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

        BuildGrowthBar();
        BuildReadyMarker();

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

    void Update()
    {
        if (_readyMarkerRoot == null || !_readyMarkerRoot.activeSelf) return;

        float pulse = 1f + Mathf.Sin(Time.time * 7f + SlotIndex) * 0.10f;
        _readyMarkerRoot.transform.localScale = new Vector3(pulse, pulse, 1f);
    }

    public bool TryPlant(CropData crop)
    {
        if (!IsEmpty) return false;
        if (!StaminaSystem.Instance.TrySpend(crop.staminaCostToPlant)) return false;
        if (!GameManager.Instance.CanAfford(crop.seedCost)) return false;

        GameManager.Instance.SpendBalance(crop.seedCost);
        GameManager.Instance.RecordPlanting(SlotIndex, crop.cropName);
        _crop = crop;
        _daysPlanted = 0;
        RefreshSprite();
        ShowActionFeedback(new Color(0.36f, 0.95f, 0.42f, 1f));
        return true;
    }

    public bool TryWater()
    {
        if (_wateredToday || IsReady) return false;

        int staminaCost = _crop != null ? _crop.staminaCostToWater : 1;
        if (!StaminaSystem.Instance.TrySpend(staminaCost)) return false;

        _wateredToday = true;
        RefreshSprite();
        ShowActionFeedback(new Color(0.35f, 0.78f, 1f, 1f));
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
        ShowActionFeedback(new Color(1f, 0.78f, 0.24f, 1f));
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

        RefreshGrowthBar();
        RefreshReadyMarker();
    }

    void BuildGrowthBar()
    {
        var bgGO = new GameObject("GrowthBarBg");
        bgGO.transform.SetParent(transform, false);
        bgGO.transform.localPosition = new Vector3(0f, BarYOffset, 0f);
        bgGO.transform.localScale = new Vector3(BarWidth, BarHeight, 1f);
        _growthBarBg = bgGO.AddComponent<SpriteRenderer>();
        _growthBarBg.sprite = GetWhitePixel();
        _growthBarBg.color = new Color(0.08f, 0.06f, 0.04f, 0.78f);
        _growthBarBg.sortingOrder = _soilRenderer.sortingOrder + 4;

        var fillGO = new GameObject("GrowthBarFill");
        fillGO.transform.SetParent(transform, false);
        fillGO.transform.localPosition = new Vector3(-BarWidth * 0.5f, BarYOffset, 0f);
        fillGO.transform.localScale = new Vector3(0f, BarHeight * 0.85f, 1f);
        _growthBarFill = fillGO.AddComponent<SpriteRenderer>();
        _growthBarFill.sprite = GetWhitePixel();
        _growthBarFill.color = new Color(0.55f, 0.85f, 0.35f, 1f);
        _growthBarFill.sortingOrder = _soilRenderer.sortingOrder + 5;
    }

    void RefreshGrowthBar()
    {
        if (_growthBarBg == null || _growthBarFill == null) return;

        bool visible = _crop != null && !IsReady && _crop.growthDays > 0;
        _growthBarBg.gameObject.SetActive(visible);
        _growthBarFill.gameObject.SetActive(visible);
        if (!visible) return;

        float progress = Mathf.Clamp01((float)_daysPlanted / _crop.growthDays);
        float fillWidth = BarWidth * progress;

        var fillT = _growthBarFill.transform;
        fillT.localScale = new Vector3(fillWidth, BarHeight * 0.85f, 1f);
        // Left-anchored growth: position the centered sprite so its left edge is fixed.
        fillT.localPosition = new Vector3(-BarWidth * 0.5f + fillWidth * 0.5f, BarYOffset, 0f);

        _growthBarFill.color = Color.Lerp(
            new Color(0.55f, 0.85f, 0.35f, 1f),  // verde
            new Color(0.95f, 0.75f, 0.20f, 1f),  // dourado
            progress);
    }

    void BuildReadyMarker()
    {
        _readyMarkerRoot = new GameObject("ReadyMarker");
        _readyMarkerRoot.transform.SetParent(transform, false);
        _readyMarkerRoot.transform.localPosition = new Vector3(0.33f, 0.56f, 0f);

        var backGO = new GameObject("Back");
        backGO.transform.SetParent(_readyMarkerRoot.transform, false);
        backGO.transform.localScale = new Vector3(0.24f, 0.30f, 1f);
        var back = backGO.AddComponent<SpriteRenderer>();
        back.sprite = GetWhitePixel();
        back.color = new Color(1f, 0.78f, 0.18f, 0.92f);
        back.sortingOrder = _soilRenderer.sortingOrder + 10;

        var lineGO = new GameObject("Line");
        lineGO.transform.SetParent(_readyMarkerRoot.transform, false);
        lineGO.transform.localPosition = new Vector3(0f, 0.04f, 0f);
        lineGO.transform.localScale = new Vector3(0.045f, 0.16f, 1f);
        var line = lineGO.AddComponent<SpriteRenderer>();
        line.sprite = GetWhitePixel();
        line.color = new Color(0.16f, 0.08f, 0.02f, 1f);
        line.sortingOrder = _soilRenderer.sortingOrder + 11;

        var dotGO = new GameObject("Dot");
        dotGO.transform.SetParent(_readyMarkerRoot.transform, false);
        dotGO.transform.localPosition = new Vector3(0f, -0.10f, 0f);
        dotGO.transform.localScale = new Vector3(0.055f, 0.055f, 1f);
        var dot = dotGO.AddComponent<SpriteRenderer>();
        dot.sprite = GetWhitePixel();
        dot.color = new Color(0.16f, 0.08f, 0.02f, 1f);
        dot.sortingOrder = _soilRenderer.sortingOrder + 11;

        _readyMarkerRoot.SetActive(false);
    }

    void RefreshReadyMarker()
    {
        if (_readyMarkerRoot != null)
            _readyMarkerRoot.SetActive(IsReady);
    }

    void ShowActionFeedback(Color color)
    {
        if (!isActiveAndEnabled) return;
        if (_actionFeedbackRoutine != null)
            StopCoroutine(_actionFeedbackRoutine);
        if (_actionFeedbackRoot != null)
            Destroy(_actionFeedbackRoot);
        _actionFeedbackRoutine = StartCoroutine(AnimateActionFeedback(color));
    }

    IEnumerator AnimateActionFeedback(Color color)
    {
        const int count = 7;
        var pieces = new SpriteRenderer[count];
        var starts = new Vector3[count];
        var ends = new Vector3[count];

        _actionFeedbackRoot = new GameObject("ActionFeedback");
        _actionFeedbackRoot.transform.SetParent(transform, false);
        _actionFeedbackRoot.transform.localPosition = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            float angle = (i / (float)count) * Mathf.PI * 2f;
            starts[i] = new Vector3(Mathf.Cos(angle) * 0.12f, Mathf.Sin(angle) * 0.04f, 0f);
            ends[i] = new Vector3(Mathf.Cos(angle) * 0.30f, 0.40f + Mathf.Abs(Mathf.Sin(angle)) * 0.12f, 0f);

            var go = new GameObject($"Spark_{i}");
            go.transform.SetParent(_actionFeedbackRoot.transform, false);
            go.transform.localPosition = starts[i];
            go.transform.localScale = Vector3.one * (0.055f + i % 3 * 0.015f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = GetWhitePixel();
            sr.color = color;
            sr.sortingOrder = _soilRenderer.sortingOrder + 12;
            pieces[i] = sr;
        }

        float duration = 0.48f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float eased = 1f - Mathf.Pow(1f - k, 2f);
            for (int i = 0; i < pieces.Length; i++)
            {
                if (pieces[i] == null) continue;
                pieces[i].transform.localPosition = Vector3.Lerp(starts[i], ends[i], eased);
                pieces[i].transform.localScale = Vector3.one * Mathf.Lerp(0.075f, 0.025f, k);
                var c = color;
                c.a = 1f - k;
                pieces[i].color = c;
            }
            yield return null;
        }

        Destroy(_actionFeedbackRoot);
        _actionFeedbackRoot = null;
        _actionFeedbackRoutine = null;
    }

    static Sprite GetWhitePixel()
    {
        if (_whitePixel != null) return _whitePixel;
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        tex.name = "CropSlot_WhitePixelTexture";
        tex.hideFlags = HideFlags.HideAndDontSave;
        _whitePixel = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f), 1f);
        _whitePixel.name = "CropSlot_WhitePixel";
        return _whitePixel;
    }

    public void Interact()
    {
        if (ToolbarController.Instance == null) return;

        bool plantingChanged = false;

        switch (ToolbarController.Instance.Selected)
        {
            case ToolType.Mandioca:
            case ToolType.Cacau:
            case ToolType.Acai:
                CropData crop = CropSystem.Instance?.GetCropData(ToolbarController.Instance.Selected);
                if (crop != null && TryPlant(crop))
                {
                    plantingChanged = true;
                    TutorialController.Instance?.NotifyPlanted();
                }
                break;
            case ToolType.Water:
                if (TryWater())
                    TutorialController.Instance?.NotifyWatered();
                break;
            case ToolType.Harvest:
                CropData harvested = TryHarvest();
                if (harvested != null)
                {
                    InventorySystem.Instance?.AddCrop(harvested.cropName);
                    plantingChanged = true;
                    TutorialController.Instance?.NotifyHarvested();
                }
                break;
        }

        if (plantingChanged)
        {
            HUDController.Instance?.RefreshBalance(GameManager.Instance.Balance);
            HUDController.Instance?.RefreshProportionalityCue();
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
