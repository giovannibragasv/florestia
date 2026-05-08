using UnityEngine;
using UnityEngine.UI;

public class BuyerSelector : MonoBehaviour
{
    [SerializeField] MarketUIController marketUI;
    [SerializeField] BuyerData[] buyers;
    [SerializeField] Button[] buyerButtons;
    [SerializeField] Color normalColor = new Color(0.25f, 0.20f, 0.16f, 1f);
    [SerializeField] Color selectedColor = new Color(0.72f, 0.52f, 0.24f, 1f);

    void Start()
    {
        for (int i = 0; i < buyerButtons.Length && i < buyers.Length; i++)
        {
            int idx = i;
            buyerButtons[idx].onClick.AddListener(() => SelectBuyer(idx));
        }

        if (buyers != null && buyers.Length > 0)
            SelectBuyer(0);
    }

    void SelectBuyer(int index)
    {
        if (buyers == null || buyerButtons == null ||
            index < 0 || index >= buyers.Length || buyers[index] == null)
            return;

        marketUI.OnBuyerSelected(buyers[index]);

        for (int i = 0; i < buyerButtons.Length; i++)
        {
            if (buyerButtons[i] == null ||
                buyerButtons[i].targetGraphic is not Image image)
                continue;

            image.color = i == index ? selectedColor : normalColor;
        }
    }
}
