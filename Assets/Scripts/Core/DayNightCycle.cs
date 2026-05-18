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

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (_paused) return;

        _elapsed += Time.deltaTime;
        UpdateSkyColor();

        if (!_warningShown && TimeRemaining <= warningThreshold)
        {
            _warningShown = true;
            if (nightWarningPanel != null)
                nightWarningPanel.SetActive(true);
        }

        if (_elapsed >= GameManager.DayDurationSeconds)
        {
            _paused = true;
            if (nightWarningPanel != null)
                nightWarningPanel.SetActive(false);
            GameManager.Instance.GoToMarket();
        }
    }

    void UpdateSkyColor()
    {
        if (skyOverlay == null) return;

        Color clear = new Color(1f, 1f, 1f, 0f);
        Color golden = new Color(1.00f, 0.52f, 0.12f, 0.10f);
        Color dusk = new Color(0.07f, 0.04f, 0.16f, 0.34f);
        float t = Mathf.Clamp01(NormalizedTime);

        if (t <= 0.58f)
        {
            skyOverlay.color = clear;
            return;
        }

        skyOverlay.color = t <= 0.78f
            ? Color.Lerp(clear, golden, Mathf.InverseLerp(0.58f, 0.78f, t))
            : Color.Lerp(golden, dusk, Mathf.InverseLerp(0.78f, 1f, t));
    }

    public void Pause() => _paused = true;
    public void Resume() => _paused = false;

    public void ResetDay()
    {
        _elapsed = 0f;
        _warningShown = false;
        _paused = false;
        if (nightWarningPanel != null)
            nightWarningPanel.SetActive(false);
        UpdateSkyColor();
    }
}
