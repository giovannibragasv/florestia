using TMPro;
using UnityEngine;

public static class FlorestiaFontUtility
{
    const string PreferredFont = "Fonts & Materials/Stardew_Valley SDF";
    const string FallbackFont = "Fonts & Materials/LiberationSans SDF";

    public static TMP_FontAsset LoadPreferredTMPFont()
    {
        return Resources.Load<TMP_FontAsset>(PreferredFont)
               ?? Resources.Load<TMP_FontAsset>(FallbackFont);
    }
}
