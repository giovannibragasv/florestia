using UnityEngine;

public class BuyerSystem : MonoBehaviour
{
    public static BuyerSystem Instance { get; private set; }

    [SerializeField] BuyerData[] buyers; // assign Atravessador, Feirante, Comprador in Inspector

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public BuyerData[] GetAllBuyers() => buyers;

    // Returns true and processes sale if buyer accepts the asking price
    public bool TrySell(BuyerData buyer, string cropName, int quantity, float askingPricePerUnit)
    {
        if (buyer.GetMaxPrice(cropName) < askingPricePerUnit) return false;
        if (!InventorySystem.Instance.TryRemoveCrop(cropName, quantity)) return false;

        float revenue = askingPricePerUnit * quantity;
        GameManager.Instance.AddRevenue(revenue);
        GameManager.Instance.RecordSale(cropName, quantity, askingPricePerUnit, buyer.buyerName);
        EndScreenController.Instance?.RecordSale(cropName, revenue, quantity, askingPricePerUnit);
        return true;
    }
}
