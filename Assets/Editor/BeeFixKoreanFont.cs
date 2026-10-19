using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

/// <summary>
/// Rebuilds the broken dynamic Korean font asset (NanumGothic SDF) whose atlas
/// texture / material sub-assets were lost, then registers it as a fallback of
/// the default TMP font asset so Korean text renders.
///
/// Runs automatically on editor load (only when the atlas is missing), and can
/// also be triggered from the menu: Tools > Bee > Fix Korean Font.
/// </summary>
public static class BeeFixKoreanFont
{
    private const string TtfPath = "Assets/Fonts/NanumGothic.ttf";
    private const string FontAssetPath = "Assets/Fonts/NanumGothic SDF.asset";
    private const string DefaultFontPath =
        "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

    [InitializeOnLoadMethod]
    private static void AutoFix()
    {
        // Delay so we don't touch the asset database while it is still importing.
        EditorApplication.delayCall += () =>
        {
            if (NeedsFix())
                FixKoreanFont();
        };
    }

    private static bool NeedsFix()
    {
        TMP_FontAsset fa = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (fa == null) return false;
        if (fa.atlasTextures == null || fa.atlasTextures.Length == 0) return true;
        for (int i = 0; i < fa.atlasTextures.Length; i++)
            if (fa.atlasTextures[i] == null) return true;
        return fa.material == null;
    }

    [MenuItem("Tools/Bee/Fix Korean Font")]
    public static void FixKoreanFont()
    {
        Font font = AssetDatabase.LoadAssetAtPath<Font>(TtfPath);
        if (font == null)
        {
            Debug.LogError($"[BeeFix] Could not find {TtfPath}");
            return;
        }

        try
        {
            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
                font,
                90,                 // sampling point size
                9,                  // atlas padding
                GlyphRenderMode.SDFAA,
                1024,               // atlas width
                1024,               // atlas height
                AtlasPopulationMode.Dynamic,
                true);              // enable multi-atlas support

            if (fontAsset == null)
            {
                Debug.LogError("[BeeFix] TMP_FontAsset.CreateFontAsset returned null.");
                return;
            }

            fontAsset.name = "NanumGothic SDF";

            // Overwrite the existing (broken) asset in place; the existing .meta /
            // GUID is preserved so references to it keep working.
            AssetDatabase.CreateAsset(fontAsset, FontAssetPath);

            if (fontAsset.material != null)
            {
                fontAsset.material.name = "NanumGothic SDF Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            if (fontAsset.atlasTextures != null)
            {
                for (int i = 0; i < fontAsset.atlasTextures.Length; i++)
                {
                    Texture2D atlas = fontAsset.atlasTextures[i];
                    if (atlas == null) continue;
                    atlas.name = "NanumGothic SDF Atlas";
                    AssetDatabase.AddObjectToAsset(atlas, fontAsset);
                }
            }

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(FontAssetPath, ImportAssetOptions.ForceUpdate);

            // Ensure the rebuilt font is a fallback of the default font asset.
            TMP_FontAsset defaultFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(DefaultFontPath);
            if (defaultFont != null)
            {
                if (defaultFont.fallbackFontAssetTable == null)
                    defaultFont.fallbackFontAssetTable = new List<TMP_FontAsset>();

                if (!defaultFont.fallbackFontAssetTable.Contains(fontAsset))
                {
                    defaultFont.fallbackFontAssetTable.Add(fontAsset);
                    EditorUtility.SetDirty(defaultFont);
                    AssetDatabase.SaveAssets();
                }
            }

            Debug.Log("[BeeFix] Rebuilt NanumGothic SDF font asset and wired it as a fallback.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[BeeFix] Failed to rebuild Korean font asset: " + e);
        }
    }
}
