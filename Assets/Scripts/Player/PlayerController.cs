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

    // 0=up  1=right  2=down  3=left  (Stardew convention)
    public int FacingDirection { get; private set; } = 2;

    static readonly Vector2[] FacingOffset =
        { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    Rigidbody2D _rb;
    SpriteRenderer _sr;
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

    void FixedUpdate() => _rb.linearVelocity = _input * moveSpeed;

    void LateUpdate()
    {
        if (Camera.main == null) return;
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

        Vector2 targetPos = (Vector2)transform.position
            + FacingOffset[FacingDirection] * interactionRange;

        CropSlot found = FindSlotNear(targetPos);
        tileHighlight.gameObject.SetActive(found != null);
        if (found != null)
        {
            tileHighlight.transform.position = new Vector3(
                found.transform.position.x,
                found.transform.position.y, 0f);
            float alpha = 0.3f + 0.15f * Mathf.Sin(Time.time * 4f);
            Color c = tileHighlight.color;
            c.a = alpha;
            tileHighlight.color = c;
        }
    }

    void TryInteract()
    {
        if (ToolbarController.Instance == null) return;

        Vector2 targetPos = (Vector2)transform.position
            + FacingOffset[FacingDirection] * interactionRange;

        FindSlotNear(targetPos)?.Interact();
    }

    CropSlot FindSlotNear(Vector2 pos)
    {
        foreach (var hit in Physics2D.OverlapCircleAll(pos, 0.45f))
        {
            var slot = hit.GetComponent<CropSlot>();
            if (slot != null) return slot;
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.yellow;
        Vector2 targetPos = (Vector2)transform.position
            + FacingOffset[FacingDirection] * interactionRange;
        Gizmos.DrawWireSphere(targetPos, 0.45f);
    }
}
