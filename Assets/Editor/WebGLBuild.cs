// Headless WebGL build helper for Apps in Toss (.ait) submission.
// Korean UI font fallback is applied before building so every TMP text
// (LiberationSans etc.) renders Hangul via a NanumGothic SDF fallback.
// Usage (batch mode):
//   Unity.exe -batchmode -nographics -quit -projectPath <proj> \
//     -executeMethod WebGLBuild.Build -logFile <log>
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuild
{
    private const string KoreanTtfPath = "Assets/Fonts/NanumGothic.ttf";
    private const string KoreanFontAssetPath = "Assets/Fonts/NanumGothic SDF.asset";

    public static void Build()
    {
        SetupKoreanFontFallback();

        string[] scenes = Array.ConvertAll(
            EditorBuildSettings.scenes,
            s => s.path);
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "Builds/WebGL",
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
        {
            throw new Exception("WebGL build failed: " + summary.result
                + " (" + summary.totalErrors + " errors)");
        }
        Debug.Log("[WebGLBuild] OK output=" + summary.outputPath
            + " size=" + summary.totalSize);
    }

    private static void SetupKoreanFontFallback()
    {
        if (!AssetDatabase.LoadAssetAtPath<Font>(KoreanTtfPath))
        {
            Debug.LogWarning("[WebGLBuild] NanumGothic.ttf not found — Korean fallback skipped");
            return;
        }
        TMP_FontAsset korean = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(KoreanFontAssetPath);
        if (korean == null)
        {
            // TMP 4 (uGUI-integrated, Unity 6 upgrade path) needs a TMP Settings
            // asset before CreateFontAsset; TMP 3 projects already have one.
            if (TMP_Settings.instance == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/TextMesh Pro"))
                    AssetDatabase.CreateFolder("Assets", "TextMesh Pro");
                if (!AssetDatabase.IsValidFolder("Assets/TextMesh Pro/Resources"))
                    AssetDatabase.CreateFolder("Assets/TextMesh Pro", "Resources");
                var settings = ScriptableObject.CreateInstance<TMP_Settings>();
                AssetDatabase.CreateAsset(settings,
                    "Assets/TextMesh Pro/Resources/TMP Settings.asset");
                AssetDatabase.SaveAssets();
                Debug.Log("[WebGLBuild] created TMP Settings asset");
            }
            Font font = AssetDatabase.LoadAssetAtPath<Font>(KoreanTtfPath);
            // Universal overload (TMP 3.x/4.x): defaults SDFAA dynamic atlas.
            korean = TMP_FontAsset.CreateFontAsset(font);
            AssetDatabase.CreateAsset(korean, KoreanFontAssetPath);
            AssetDatabase.SaveAssets();
        }
        int updated = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:TMP_FontAsset"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path == KoreanFontAssetPath) continue;
            TMP_FontAsset fa = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (fa == null) continue;
            List<TMP_FontAsset> table = fa.fallbackFontAssetTable ?? new List<TMP_FontAsset>();
            if (!table.Contains(korean))
            {
                table.Add(korean);
                fa.fallbackFontAssetTable = table;
                EditorUtility.SetDirty(fa);
                updated++;
            }
        }
        // Global fallback on TMP Settings: covers text using the default or a
        // broken/migrated font reference (e.g. uGUI-TMP after 2022 -> 6000 upgrade).
        if (TMP_Settings.instance != null)
        {
            var globalFallbacks = TMP_Settings.fallbackFontAssets
                ?? new List<TMP_FontAsset>();
            if (!globalFallbacks.Contains(korean))
            {
                globalFallbacks.Add(korean);
                TMP_Settings.fallbackFontAssets = globalFallbacks;
                EditorUtility.SetDirty(TMP_Settings.instance);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("[WebGLBuild] Korean fallback on " + updated + " TMP font assets");
    }
}