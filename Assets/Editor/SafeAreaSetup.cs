using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SafeAreaSetup
{
    public static void Install()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        GameObject hud = GameObject.Find("HUD");
        if (hud == null) { Debug.LogError("[SafeAreaSetup] HUD not found"); EditorApplication.Exit(1); }
        if (hud.GetComponent<SafeAreaFitter>() == null)
            hud.AddComponent<SafeAreaFitter>();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[SafeAreaSetup] SafeAreaFitter installed on HUD");
        EditorApplication.Exit(0);
    }
}
