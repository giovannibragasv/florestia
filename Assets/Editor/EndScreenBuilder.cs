using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class EndScreenBuilder
{
    [MenuItem("Florestia/Build End Screen UI")]
    static void Build()
    {
        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(canvasGO, "Build End Screen");

        Transform ct = canvasGO.transform;

        RectTransform MakeRect(string name, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            Undo.RegisterCreatedObjectUndo(go, "Build End Screen");
            return rt;
        }

        TMP_Text MakeLabel(string name, Transform parent, string text,
            Vector2 pos, Vector2 size, int fontSize = 28)
        {
            var rt = MakeRect(name, parent,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.black;
            return tmp;
        }

        // Background
        var bg = MakeRect("Background", ct, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        bg.gameObject.AddComponent<Image>().color = new Color(0.13f, 0.13f, 0.13f);

        // Title
        MakeLabel("TitleLabel", ct, "Fim do Ciclo", new Vector2(0, 240), new Vector2(400, 55), 38);

        // Result labels
        var finalBalance = MakeLabel("FinalBalanceLabel", ct, "Saldo final: R$0,00",
            new Vector2(0, 170), new Vector2(400, 45));
        finalBalance.color = Color.white;

        var outcome = MakeLabel("OutcomeLabel", ct, "—",
            new Vector2(0, 120), new Vector2(400, 45), 32);

        var educational = MakeLabel("EducationalLabel", ct, "",
            new Vector2(0, 80), new Vector2(520, 40), 18);
        educational.color = new Color(0.9f, 0.86f, 0.68f);

        var bestCrop = MakeLabel("BestCropLabel", ct, "",
            new Vector2(0, 40), new Vector2(400, 40), 22);
        bestCrop.color = new Color(0.8f, 0.8f, 0.8f);

        // Chart container
        var chartRT = MakeRect("ChartContainer", ct,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -60), new Vector2(500, 160));
        chartRT.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.08f);

        // Chart axis label
        MakeLabel("ChartLabel", ct, "Saldo por dia",
            new Vector2(0, -150), new Vector2(300, 30), 16).color = new Color(0.6f, 0.6f, 0.6f);

        // Bar prefab (create as asset)
        var barGO = new GameObject("BarPrefab");
        barGO.AddComponent<RectTransform>();
        barGO.AddComponent<Image>().color = new Color(0.2f, 0.75f, 0.4f);
        string barPath = "Assets/Prefabs/UI/ChartBar.prefab";
        var barPrefab = PrefabUtility.SaveAsPrefabAsset(barGO, barPath);
        Object.DestroyImmediate(barGO);

        // Play again button
        var btnRT = MakeRect("PlayAgainButton", ct,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -230), new Vector2(200, 55));
        btnRT.gameObject.AddComponent<Image>().color = new Color(0.2f, 0.6f, 0.3f);
        var playAgainBtn = btnRT.gameObject.AddComponent<Button>();
        MakeLabel("PlayAgainLabel", btnRT, "Jogar novamente",
            Vector2.zero, new Vector2(200, 55), 20).color = Color.white;

        // Wire EndScreenController (which persists from FarmScene)
        // Find the _EndScreen GO if it already exists in scene
        var endScreenGO = GameObject.Find("_EndScreen");
        if (endScreenGO != null)
        {
            var esc = endScreenGO.GetComponent<EndScreenController>();
            if (esc != null)
            {
                SerializedObject so = new SerializedObject(esc);
                so.FindProperty("finalBalanceLabel").objectReferenceValue = finalBalance;
                so.FindProperty("outcomeLabel").objectReferenceValue      = outcome;
                so.FindProperty("educationalLabel").objectReferenceValue  = educational;
                so.FindProperty("bestCropLabel").objectReferenceValue     = bestCrop;
                so.FindProperty("chartContainer").objectReferenceValue    = chartRT;
                so.FindProperty("barPrefab").objectReferenceValue         = barPrefab;
                so.FindProperty("playAgainButton").objectReferenceValue   = playAgainBtn;
                so.ApplyModifiedPropertiesWithoutUndo();
                Debug.Log("EndScreenController wired successfully.");
            }
        }
        else
        {
            Debug.LogWarning("_EndScreen not found in scene. Add EndScreenController manually and assign the UI fields.");
        }

        Debug.Log("End Screen UI built. ChartBar prefab saved to Assets/Prefabs/UI/ChartBar.prefab");
    }
}
