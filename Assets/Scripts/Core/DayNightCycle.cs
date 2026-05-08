using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [SerializeField] Image skyOverlay;
    [SerializeField] GameObject nightWarningPanel;
    [SerializeField] float warningThreshold = 60f; // seconds before day ends

    float _elapsed;
    bool _warningShown;
    bool _paused;

    public float TimeRemaining => Mathf.Max(0f, GameManager.DayDurationSeconds - _elapsed);
    public float NormalizedTime => _elapsed / GameManager.DayDurationSeconds;
    public bool IsNight => TimeRemaining <= warningThreshold;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (_paused) return;

        _elapsed += Time.deltaTime;
        UpdateSkyColor();

        if (!_warningShown && TimeRemaining <= warningThreshold)
        {
            _warningShown = true;
            nightWarningPanel?.SetActive(true);
        }

        if (_elapsed >= GameManager.DayDurationSeconds)
        {
            _paused = true;
            nightWarningPanel?.SetActive(false);
            GameManager.Instance.GoToMarket();
        }
    }

    void UpdateSkyColor()
    {
        if (skyOverlay == null) return;

        Color morning = new Color(0.53f, 0.81f, 0.98f, 0f);
        Color golden = new Color(0.98f, 0.72f, 0.3f, 0.25f);
        Color dusk = new Color(0.15f, 0.05f, 0.3f, 0.6f);
        float t = Mathf.Clamp01(NormalizedTime);

        skyOverlay.color = t <= 0.6f
            ? Color.Lerp(morning, golden, Mathf.InverseLerp(0f, 0.6f, t))
            : Color.Lerp(golden, dusk, Mathf.InverseLerp(0.6f, 1f, t));
    }

    public void Pause() => _paused = true;
    public void Resume() => _paused = false;

    public void ResetDay()
    {
        _elapsed = 0f;
        _warningShown = false;
        _paused = false;
        nightWarningPanel?.SetActive(false);
    }
}
