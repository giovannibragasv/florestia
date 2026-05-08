using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public static StaminaSystem Instance { get; private set; }

    public const int MaxStamina = 20;
    public int Current { get; private set; } = MaxStamina;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool TrySpend(int amount)
    {
        if (Current < amount) return false;
        Current -= amount;
        HUDController.Instance?.RefreshStamina(Current, MaxStamina);
        return true;
    }

    public void ResetForNewDay()
    {
        Current = MaxStamina;
        HUDController.Instance?.RefreshStamina(Current, MaxStamina);
    }
}
