using UnityEngine;

[CreateAssetMenu(fileName = "BuyerData", menuName = "Florestia/Buyer Data")]
public class BuyerData : ScriptableObject
{
    public string buyerName;
    public Sprite portrait;

    // Hidden max prices — player sees only rejection/acceptance, not these values
    public float maxPriceMandioca;
    public float maxPriceCacau;
    public float maxPriceAcai;

    [TextArea] public string rejectLine;
    [TextArea] public string acceptLine;

    public float GetMaxPrice(string cropName) => cropName switch
    {
        "Mandioca" => maxPriceMandioca,
        "Cacau"    => maxPriceCacau,
        "Acai"     => maxPriceAcai,
        _          => 0f
    };
}
