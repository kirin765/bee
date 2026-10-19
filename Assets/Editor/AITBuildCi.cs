// Headless entry point for building the Apps in Toss Unity SDK (.ait) bundle.
// The SDK's Editor menu (AIT > Advanced > Build & Package) is async/UI driven, so
// CI/batch builds call AITConvertCore.DoExport directly through this wrapper.
//
// Usage (batch mode):
//   AIT_APP_NAME=bee AIT_DISPLAY_NAME="Bee Shooter" AIT_VERSION=1.0.0 \
//   AIT_ICON_URL=https://... AIT_CLEAN_BUILD=1 \
//   Unity -batchmode -nographics -quit -projectPath <proj> \
//     -executeMethod AITBuildCi.Build -logFile <log>
//
// Any AIT_* override below is optional and only used for diagnostics/tuning.
using System;
using AppsInToss;
using UnityEditor;
using UnityEngine;

public static class AITBuildCi
{
    public static void Build()
    {
        // Audio assets are Git-LFS-tracked. If Unity first imported an LFS
        // pointer it caches an empty placeholder (null clip) and never
        // re-imports on its own, so force the audio re-import before every build.
        BeeFixAudio.ReimportAudio();

        AITEditorScriptObject config = UnityUtil.GetEditorConf();
        config.appName = Env("AIT_APP_NAME", config.appName);
        config.displayName = Env("AIT_DISPLAY_NAME", config.displayName);
        config.iconUrl = Env("AIT_ICON_URL", config.iconUrl);
        config.version = Env("AIT_VERSION", config.version);

        // Diagnostics/tuning overrides (no-ops when the env var is unset).
        ApplyBool("AIT_NAME_FILES_AS_HASHES", v => config.nameFilesAsHashes = v);
        ApplyInt("AIT_DECOMPRESSION_FALLBACK", v => config.decompressionFallback = v);
        ApplyBool("AIT_WASM_STREAMING", v => config.wasmStreaming = v);
        ApplyInt("AIT_THREADS", v => config.threadsSupport = v);
        ApplyInt("AIT_DATA_CACHING", v => config.dataCaching = v);
        ApplyInt("AIT_MEMORY", v => config.memorySize = v);
        ApplyInt("AIT_DEVICE_PIXEL_RATIO", v => config.devicePixelRatio = v);

        ApplyBool("AIT_DEBUG_SYMBOLS_EXTERNAL", v => config.productionProfile.debugSymbolsExternal = v);
        ApplyInt("AIT_COMPRESSION_FORMAT", v => config.productionProfile.compressionFormat = v);
        ApplyBool("AIT_LZ4", v => config.productionProfile.enableLZ4Compression = v);
        ApplyInt("AIT_STRIPPING", v => config.productionProfile.managedStrippingLevel = v);

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();

        Debug.Log("[AITBuildCi] appName=" + config.appName
            + " displayName=" + config.displayName
            + " version=" + config.version
            + " iconUrl=" + config.iconUrl
            + " nameFilesAsHashes=" + config.nameFilesAsHashes
            + " decompressionFallback=" + config.decompressionFallback
            + " wasmStreaming=" + config.wasmStreaming
            + " threads=" + config.threadsSupport
            + " dataCaching=" + config.dataCaching
            + " memory=" + config.memorySize
            + " debugSymbolsExternal=" + config.productionProfile.debugSymbolsExternal
            + " compressionFormat=" + config.productionProfile.compressionFormat
            + " stripping=" + config.productionProfile.managedStrippingLevel);

        bool clean = Env("AIT_CLEAN_BUILD", "0") == "1";
        AITConvertCore.AITExportError result = AITConvertCore.DoExport(
            buildWebGL: true,
            doPackaging: true,
            cleanBuild: clean,
            profile: config.productionProfile,
            profileName: "Production");

        Debug.Log("[AITBuildCi] DoExport result = " + result);
        EditorApplication.Exit(result == AITConvertCore.AITExportError.SUCCEED ? 0 : 1);
    }

    private static string Env(string key, string fallback)
    {
        string value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrEmpty(value) ? fallback : value;
    }

    private static void ApplyBool(string key, Action<bool> set)
    {
        string value = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out bool parsed)) set(parsed);
    }

    private static void ApplyInt(string key, Action<int> set)
    {
        string value = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int parsed)) set(parsed);
    }
}
