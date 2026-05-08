using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        DayNightCycle.Instance?.Pause();
        GameManager.Instance.GoToMarket();
    }
}
