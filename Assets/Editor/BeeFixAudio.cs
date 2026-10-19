using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Forces a re-import of the audio assets. The BGM mp3 previously failed to
/// import (it was still a Git-LFS pointer when Unity first saw it), so Unity
/// cached an empty placeholder and will not re-import it on its own.
/// Run from the menu: Tools > Bee > Reimport Audio Assets.
/// </summary>
public static class BeeFixAudio
{
    private static readonly string[] AudioPaths =
    {
        "Assets/Audio/light-instrumental-melody-313329.mp3",
        "Assets/Audio/impactGeneric_light_004.ogg",
        "Assets/Audio/impactWood_medium_002.ogg",
    };

    [MenuItem("Tools/Bee/Reimport Audio Assets")]
    public static void ReimportAudio()
    {
        foreach (string path in AudioPaths)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[BeeFix] Missing audio file: {path}");
                continue;
            }

            AssetDatabase.ImportAsset(
                path,
                ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null)
                Debug.LogError($"[BeeFix] Still no AudioClip after reimport: {path}");
            else
                Debug.Log($"[BeeFix] Reimported {path} (length {clip.length:0.0}s)");
        }

        AssetDatabase.Refresh();
    }
}
