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
            nightWarningPanel.SetActive(true);
        }

        if (_elapsed >= GameManager.DayDurationSeconds)
        {
            _paused = true;
            nightWarningPanel.SetActive(false);
            GameManager.Instance.GoToMarket();
        }
    }

    void UpdateSkyColor()
    {
        // Lerp from clear (alpha 0) to dusk tint (alpha 0.6) over the day
        float alpha = Mathf.Lerp(0f, 0.6f, NormalizedTime);
        skyOverlay.color = new Color(0.1f, 0.05f, 0.2f, alpha);
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
