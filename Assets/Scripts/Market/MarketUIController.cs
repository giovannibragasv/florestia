using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketUIController : MonoBehaviour
{
    [Header("Crop Selection")]
    [SerializeField] TMP_Dropdown cropDropdown;
    [SerializeField] TMP_Text stockLabel;

    [Header("Pricing")]
    [SerializeField] Slider priceSlider;
    [SerializeField] TMP_Text costLabel;
    [SerializeField] TMP_Text priceLabel;
    [SerializeField] TMP_Text marginLabel;

    [Header("Buyer")]
    [SerializeField] BuyerSelector buyerSelector;
    [SerializeField] TMP_Text buyerDialogueLine;
    [SerializeField] Image buyerPortrait;

    [Header("Action")]
    [SerializeField] Button sellButton;
    [SerializeField] Button endDayButton;

    string _selectedCrop;
    BuyerData _selectedBuyer;

    void Start()
    {
        if (cropDropdown == null) return;

        cropDropdown.onValueChanged.AddListener(_ => OnCropChanged());
        priceSlider.onValueChanged.AddListener(_ => RefreshPriceDisplay());
        sellButton.onClick.AddListener(OnSellClicked);
        endDayButton.onClick.AddListener(OnEndDayClicked);

        OnCropChanged();
    }

    void OnCropChanged()
    {
        _selectedCrop = cropDropdown.options[cropDropdown.value].text;
        float asking = PricingSystem.Instance.GetAskingPrice(_selectedCrop);
        priceSlider.value = asking;
        RefreshStockLabel();
        RefreshPriceDisplay();
    }

    void RefreshStockLabel()
    {
        int qty = InventorySystem.Instance.GetCount(_selectedCrop);
        stockLabel.text = $"Estoque: {qty}";
    }

    void RefreshPriceDisplay()
    {
        float asking = priceSlider.value;
        PricingSystem.Instance.SetAskingPrice(_selectedCrop, asking);

        float cost = PricingSystem.Instance.GetSeedCost(_selectedCrop);
        float margin = PricingSystem.Instance.GetMarginValue(_selectedCrop);
        float pct = PricingSystem.Instance.GetMarginPercent(_selectedCrop);

        costLabel.text = $"Custo: R${cost:F2}";
        priceLabel.text = $"Seu preço: R${asking:F2}";
        marginLabel.text = margin >= 0
            ? $"Margem: R${margin:F2} (+{pct:F0}%)"
            : $"Margem: R${margin:F2} ({pct:F0}%)";
    }

    public void OnBuyerSelected(BuyerData buyer)
    {
        _selectedBuyer = buyer;
        buyerPortrait.sprite = buyer.portrait;
        buyerDialogueLine.text = "";
    }

    void OnSellClicked()
    {
        if (_selectedBuyer == null || InventorySystem.Instance.GetCount(_selectedCrop) == 0) return;

        bool sold = BuyerSystem.Instance.TrySell(
            _selectedBuyer, _selectedCrop, 1,
            PricingSystem.Instance.GetAskingPrice(_selectedCrop));

        buyerDialogueLine.text = sold ? _selectedBuyer.acceptLine : _selectedBuyer.rejectLine;
        HUDController.Instance?.RefreshBalance(GameManager.Instance.Balance);
        RefreshStockLabel();
    }

    void OnEndDayClicked()
    {
        CropSystem.Instance.OnDayEnd();
        StaminaSystem.Instance.ResetForNewDay();
        DayNightCycle.Instance?.ResetDay();
        GameManager.Instance.AdvanceDay();
    }
}
