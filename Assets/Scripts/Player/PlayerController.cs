using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float interactionRange = 1.0f;
    [SerializeField] KeyCode interactKey = KeyCode.E;
    [SerializeField] SpriteRenderer tileHighlight;
    [SerializeField] Sprite idleDown;
    [SerializeField] Sprite idleUp;
    [SerializeField] Sprite[] walkDown;
    [SerializeField] Sprite[] walkUp;
    [SerializeField] Sprite[] walkSide;
    [SerializeField] Vector2 mapMin = new Vector2(-3.5f, -2.45f);
    [SerializeField] Vector2 mapMax = new Vector2(7.5f, 7.45f);
    [SerializeField] float forwardSelectDistance = 1.45f;
    [SerializeField] float lateralSelectTolerance = 0.62f;
    [SerializeField] float backwardSelectTolerance = 0.05f;
    [SerializeField] float waterRepeatInterval = 0.16f;

    // 0=up  1=right  2=down  3=left  (Stardew convention)
    public int FacingDirection { get; private set; } = 2;

    static readonly Vector2[] FacingOffset =
        { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    Rigidbody2D _rb;
    SpriteRenderer _sr;
    CropSlot[] _cachedSlots;
    SpriteRenderer[] _highlightEdges;
    static Sprite _highlightPixel;
    CropSlot _hoveredSlot;
    Vector2 _input;
    float _animTimer;
    float _nextWaterUseTime;
    int _animFrame;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        UpdateFacing();
        UpdateWalkAnimation();
        UpdateTileHighlight();

        bool pressedUse = Input.GetKeyDown(interactKey) || Input.GetKeyDown(KeyCode.Space);
        bool holdingWater = ToolbarController.Instance != null
            && ToolbarController.Instance.Selected == ToolType.Water
            && Input.GetKey(interactKey)
            && Time.time >= _nextWaterUseTime;

        if (pressedUse || holdingWater)
        {
            TryInteract();
            if (ToolbarController.Instance != null &&
                ToolbarController.Instance.Selected == ToolType.Water)
                _nextWaterUseTime = Time.time + waterRepeatInterval;
        }

        if (Input.GetMouseButtonDown(0))
            TryInteractWithClickedSlot();
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = _input * moveSpeed;
        Vector2 pos = _rb.position;
        Vector2 clamped = new Vector2(
            Mathf.Clamp(pos.x, mapMin.x, mapMax.x),
            Mathf.Clamp(pos.y, mapMin.y, mapMax.y));
        if (clamped != pos)
        {
            _rb.position = clamped;
            _rb.linearVelocity = Vector2.zero;
        }
    }

    void LateUpdate()
    {
        if (Camera.main == null) return;
        if (Camera.main.GetComponent<CameraConfiner>() != null) return;

        Vector3 target = transform.position;
        target.z = -10f;
        Camera.main.transform.position =
            Vector3.Lerp(Camera.main.transform.position, target, Time.deltaTime * 6f);
    }

    void UpdateFacing()
    {
        if (_input == Vector2.zero) return;
        if (Mathf.Abs(_input.x) >= Mathf.Abs(_input.y))
        {
            FacingDirection = _input.x > 0 ? 1 : 3;
            _sr.flipX = _input.x < 0;
        }
        else
        {
            FacingDirection = _input.y > 0 ? 0 : 2;
            _sr.flipX = false;
        }
    }

    void UpdateWalkAnimation()
    {
        if (_input == Vector2.zero)
        {
            _animTimer = 0f;
            _animFrame = 0;

            Sprite idleFrame = GetIdleFrame();
            if (idleFrame != null)
                _sr.sprite = idleFrame;
            return;
        }

        Sprite[] frames = FacingDirection == 0 ? walkUp : FacingDirection == 2 ? walkDown : walkSide;
        int frameCount = CountUsableFrames(frames);
        if (_animFrame >= frameCount)
            _animFrame = 0;

        if (frameCount <= 1)
        {
            _animTimer = 0f;
            _animFrame = 0;
        }
        else
        {
            _animTimer += Time.deltaTime;
            if (_animTimer >= 0.2f)
            {
                _animTimer = 0f;
                _animFrame = (_animFrame + 1) % frameCount;
            }
        }

        Sprite frame = GetUsableFrame(frames, _animFrame);
        if (frame != null)
            _sr.sprite = frame;
    }

    Sprite GetIdleFrame()
    {
        if (FacingDirection == 0)
            return idleUp != null ? idleUp : GetUsableFrame(walkUp, 0);
        if (FacingDirection == 2)
            return idleDown != null ? idleDown : GetUsableFrame(walkDown, 0);
        return GetUsableFrame(walkSide, 0);
    }

    int CountUsableFrames(Sprite[] frames)
    {
        if (frames == null) return 0;

        int count = 0;
        foreach (Sprite frame in frames)
        {
            if (frame != null)
                count++;
        }
        return count;
    }

    Sprite GetUsableFrame(Sprite[] frames, int frameIndex)
    {
        if (frames == null) return null;

        int current = 0;
        foreach (Sprite frame in frames)
        {
            if (frame == null) continue;
            if (current == frameIndex)
                return frame;
            current++;
        }
        return null;
    }

    void UpdateTileHighlight()
    {
        if (tileHighlight == null) return;
        EnsureTileHighlightStyle();

        _hoveredSlot = FindSlotUnderMouse();
        CropSlot found = _hoveredSlot;
        tileHighlight.gameObject.SetActive(found != null);
        if (found != null)
        {
            tileHighlight.transform.position = new Vector3(
                found.transform.position.x,
                found.transform.position.y, 0f);
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 5f);
            float alpha = Mathf.Lerp(0.55f, 0.82f, pulse);
            foreach (var edge in _highlightEdges)
            {
                if (edge == null) continue;
                Color c = edge.color;
                c.a = alpha;
                edge.color = c;
            }
        }
    }

    void EnsureTileHighlightStyle()
    {
        if (_highlightEdges != null && _highlightEdges.Length == 4) return;

        tileHighlight.sprite = null;
        tileHighlight.color = Color.clear;
        tileHighlight.transform.localScale = Vector3.one;

        _highlightEdges = new SpriteRenderer[4];
        Vector2[] positions =
        {
            new Vector2(0f, 0.46f),
            new Vector2(0f, -0.46f),
            new Vector2(-0.46f, 0f),
            new Vector2(0.46f, 0f)
        };
        Vector2[] scales =
        {
            new Vector2(0.92f, 0.06f),
            new Vector2(0.92f, 0.06f),
            new Vector2(0.06f, 0.92f),
            new Vector2(0.06f, 0.92f)
        };

        for (int i = 0; i < 4; i++)
        {
            var go = new GameObject($"HighlightEdge_{i}");
            go.transform.SetParent(tileHighlight.transform, false);
            go.transform.localPosition = positions[i];
            go.transform.localScale = new Vector3(scales[i].x, scales[i].y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = GetHighlightPixel();
            sr.color = new Color(1f, 0.82f, 0.18f, 0.7f);
            sr.sortingOrder = 8;
            _highlightEdges[i] = sr;
        }
    }

    bool TryInteract()
    {
        if (ToolbarController.Instance == null) return false;

        CropSlot slot = _hoveredSlot != null ? _hoveredSlot : FindSlotUnderMouse();
        if (slot == null)
            slot = FindSlotInFacingDirection();
        if (slot == null) return false;
        slot.Interact();
        return true;
    }

    bool TryInteractWithClickedSlot()
    {
        if (ToolbarController.Instance == null || Camera.main == null) return false;
        if (IsPointerOverBlockingUi())
            return false;

        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new Vector2(world.x, world.y);
        CropSlot slot = FindSlotAtWorldPoint(point);
        if (slot == null) return false;
        slot.Interact();
        return true;
    }

    CropSlot FindSlotUnderMouse()
    {
        if (Camera.main == null || IsPointerOverBlockingUi()) return null;

        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new Vector2(world.x, world.y);
        return FindSlotAtWorldPoint(point);
    }

    CropSlot FindSlotAtWorldPoint(Vector2 point)
    {
        var hits = Physics2D.OverlapPointAll(point);
        foreach (var hit in hits)
        {
            if (hit == null) continue;
            CropSlot slot = hit.GetComponent<CropSlot>() ?? hit.GetComponentInParent<CropSlot>();
            if (slot != null) return slot;
        }

        RefreshSlotCacheIfNeeded();
        CropSlot best = null;
        float bestDist = 0.72f;
        foreach (var candidate in _cachedSlots)
        {
            if (candidate == null) continue;
            float dist = Vector2.Distance(point, candidate.transform.position);
            if (dist >= bestDist) continue;
            bestDist = dist;
            best = candidate;
        }
        return best;
    }

    CropSlot FindSlotInFacingDirection()
    {
        RefreshSlotCacheIfNeeded();

        Vector2 origin = transform.position;
        Vector2 forward = FacingOffset[FacingDirection];
        float targetForwardDistance = Mathf.Clamp(interactionRange, 0.75f, forwardSelectDistance);
        CropSlot best = null;
        float bestScore = float.MaxValue;

        foreach (var slot in _cachedSlots)
        {
            if (slot == null) continue;

            Vector2 delta = (Vector2)slot.transform.position - origin;
            float forwardDistance = Vector2.Dot(delta, forward);
            if (forwardDistance < -backwardSelectTolerance ||
                forwardDistance > forwardSelectDistance)
                continue;

            float lateralDistance = Mathf.Abs(
                forward.x * delta.y - forward.y * delta.x);
            if (lateralDistance > lateralSelectTolerance)
                continue;

            float forwardError = Mathf.Abs(forwardDistance - targetForwardDistance);
            float behindPenalty = forwardDistance < 0f ? 3f : 0f;
            float score = forwardError + lateralDistance * 1.35f + behindPenalty;
            if (score < bestScore)
            {
                bestScore = score;
                best = slot;
            }
        }

        return best;
    }

    void RefreshSlotCacheIfNeeded()
    {
        if (_cachedSlots != null && _cachedSlots.Length > 0) return;

        CropSlot[] all = FindObjectsByType<CropSlot>();
        Transform farmGrid = GameObject.Find("FarmGrid")?.transform;
        if (farmGrid == null)
        {
            _cachedSlots = all;
            return;
        }

        var filtered = new System.Collections.Generic.List<CropSlot>();
        foreach (var slot in all)
        {
            if (slot == null || !slot.gameObject.activeInHierarchy) continue;
            if (slot.transform.IsChildOf(farmGrid))
                filtered.Add(slot);
        }
        _cachedSlots = filtered.Count > 0 ? filtered.ToArray() : all;
    }

    static bool IsPointerOverBlockingUi()
    {
        if (EventSystem.current == null) return false;

        var data = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        foreach (var result in results)
        {
            var selectable = result.gameObject.GetComponentInParent<Selectable>();
            if (selectable != null) return true;
        }
        return false;
    }

    static Sprite GetHighlightPixel()
    {
        if (_highlightPixel != null) return _highlightPixel;
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        tex.name = "TileHighlight_WhitePixelTexture";
        tex.hideFlags = HideFlags.HideAndDontSave;
        _highlightPixel = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f), 1f);
        _highlightPixel.name = "TileHighlight_WhitePixel";
        return _highlightPixel;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.yellow;
        Vector2 forward = FacingOffset[FacingDirection];
        Vector2 center = (Vector2)transform.position + forward * (forwardSelectDistance * 0.5f);
        Vector2 size = Mathf.Abs(forward.x) > 0f
            ? new Vector2(forwardSelectDistance, lateralSelectTolerance * 2f)
            : new Vector2(lateralSelectTolerance * 2f, forwardSelectDistance);
        Gizmos.DrawWireCube(center, size);
    }
}
