using UnityEngine;
using UnityEngine.UI;

public class BuyerSelector : MonoBehaviour
{
    [SerializeField] MarketUIController marketUI;
    [SerializeField] BuyerData[] buyers;
    [SerializeField] Button[] buyerButtons;

    void Start()
    {
        for (int i = 0; i < buyerButtons.Length && i < buyers.Length; i++)
        {
            int idx = i;
            buyerButtons[idx].onClick.AddListener(() => marketUI.OnBuyerSelected(buyers[idx]));
        }
    }
}
