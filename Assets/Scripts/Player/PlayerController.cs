using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float interactionRange = 1.0f;
    [SerializeField] KeyCode interactKey = KeyCode.E;
    [SerializeField] SpriteRenderer tileHighlight;
    [SerializeField] Sprite[] walkDown;
    [SerializeField] Sprite[] walkUp;
    [SerializeField] Sprite[] walkSide;
    [SerializeField] Vector2 mapMin = new Vector2(-3.5f, -2.45f);
    [SerializeField] Vector2 mapMax = new Vector2(7.5f, 7.45f);
    [SerializeField] float forwardSelectDistance = 1.45f;
    [SerializeField] float lateralSelectTolerance = 0.75f;
    [SerializeField] float backwardSelectTolerance = 0.3f;

    // 0=up  1=right  2=down  3=left  (Stardew convention)
    public int FacingDirection { get; private set; } = 2;

    static readonly Vector2[] FacingOffset =
        { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    Rigidbody2D _rb;
    SpriteRenderer _sr;
    CropSlot[] _cachedSlots;
    Vector2 _input;
    float _animTimer;
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

        if (Input.GetKeyDown(interactKey))
            TryInteract();
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
        }
        else
        {
            _animTimer += Time.deltaTime;
            if (_animTimer >= 0.2f)
            {
                _animTimer = 0f;
                _animFrame = (_animFrame + 1) % 2;
            }
        }

        Sprite[] frames = FacingDirection == 0 ? walkUp : FacingDirection == 2 ? walkDown : walkSide;
        if (frames != null && frames.Length > _animFrame && frames[_animFrame] != null)
            _sr.sprite = frames[_animFrame];
    }

    void UpdateTileHighlight()
    {
        if (tileHighlight == null) return;

        CropSlot found = FindSlotInFacingDirection();
        tileHighlight.gameObject.SetActive(found != null);
        if (found != null)
        {
            tileHighlight.transform.position = new Vector3(
                found.transform.position.x,
                found.transform.position.y, 0f);
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 7f);
            float alpha = Mathf.Lerp(0.65f, 0.9f, pulse);
            float scale = Mathf.Lerp(1.08f, 1.18f, pulse);
            Color c = tileHighlight.color;
            c.a = alpha;
            tileHighlight.color = c;
            tileHighlight.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    void TryInteract()
    {
        if (ToolbarController.Instance == null) return;

        FindSlotInFacingDirection()?.Interact();
    }

    CropSlot FindSlotInFacingDirection()
    {
        RefreshSlotCacheIfNeeded();

        Vector2 origin = transform.position;
        Vector2 forward = FacingOffset[FacingDirection];
        Vector2 target = origin + forward * interactionRange;
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

            float distanceToTarget = Vector2.Distance(slot.transform.position, target);
            float behindPenalty = forwardDistance < 0f ? 1f : 0f;
            float score = distanceToTarget + lateralDistance * 0.5f + behindPenalty;
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
        _cachedSlots = FindObjectsByType<CropSlot>();
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
