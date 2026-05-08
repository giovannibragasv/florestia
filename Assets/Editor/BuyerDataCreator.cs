using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuyerDataCreator
{
    const string BuyerDir = "Assets/Data/Buyers";

    [MenuItem("Florestia/Create Buyer Data Assets")]
    static void Create()
    {
        EnsureBuyerFolder();

        BuyerData[] buyers =
        {
            EnsureBuyer(
                "Atravessador.asset",
                "Atravessador",
                "Assets/Sprites/Buyers/buyer_atravessador.png",
                5f, 12f, 20f,
                "Feito! Vendo logo.",
                "Tá caro demais, amigo."),
            EnsureBuyer(
                "Feirante.asset",
                "Feirante Local",
                "Assets/Sprites/Buyers/buyer_feirante.png",
                7f, 15f, 26f,
                "Trato feito! Boa mercadoria.",
                "Não tenho esse dinheiro não."),
            EnsureBuyer(
                "CompradorDireto.asset",
                "Comprador Direto",
                "Assets/Sprites/Buyers/buyer_comprador.png",
                9f, 18f, 30f,
                "Pode deixar, pago bem por qualidade.",
                "Prefiro buscar em outro lugar.")
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        WireSceneBuyers(buyers);
        Debug.Log("BuyerData assets created and wired.");
    }

    static void EnsureBuyerFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");
        if (!AssetDatabase.IsValidFolder(BuyerDir))
            AssetDatabase.CreateFolder("Assets/Data", "Buyers");
    }

    static BuyerData EnsureBuyer(
        string fileName,
        string buyerName,
        string portraitPath,
        float maxMandioca,
        float maxCacau,
        float maxAcai,
        string acceptLine,
        string rejectLine)
    {
        string path = $"{BuyerDir}/{fileName}";
        BuyerData buyer = AssetDatabase.LoadAssetAtPath<BuyerData>(path);
        if (buyer == null)
        {
            buyer = ScriptableObject.CreateInstance<BuyerData>();
            AssetDatabase.CreateAsset(buyer, path);
        }

        buyer.buyerName = buyerName;
        buyer.portrait = AssetDatabase.LoadAssetAtPath<Sprite>(portraitPath);
        buyer.maxPriceMandioca = maxMandioca;
        buyer.maxPriceCacau = maxCacau;
        buyer.maxPriceAcai = maxAcai;
        buyer.acceptLine = acceptLine;
        buyer.rejectLine = rejectLine;
        EditorUtility.SetDirty(buyer);
        return buyer;
    }

    static void WireSceneBuyers(BuyerData[] buyers)
    {
        bool changed = false;

        BuyerSystem buyerSystem = Object.FindFirstObjectByType<BuyerSystem>();
        if (buyerSystem != null)
        {
            AssignBuyers(buyerSystem, buyers);
            changed = true;
        }

        BuyerSelector buyerSelector = Object.FindFirstObjectByType<BuyerSelector>();
        if (buyerSelector != null)
        {
            AssignBuyers(buyerSelector, buyers);
            changed = true;
        }

        if (changed)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    static void AssignBuyers(Object target, BuyerData[] buyers)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty("buyers");
        prop.arraySize = buyers.Length;
        for (int i = 0; i < buyers.Length; i++)
            prop.GetArrayElementAtIndex(i).objectReferenceValue = buyers[i];
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }
}
