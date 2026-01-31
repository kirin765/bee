using UnityEditor;
using UnityEngine;

public static class LevelDefinitionCreator
{
    [MenuItem("Bee/Create Sample Level Definition")]
    public static void CreateSample()
    {
        const string assetPath = "Assets/LevelDefinition_Sample.asset";

        LevelDefinition level = AssetDatabase.LoadAssetAtPath<LevelDefinition>(assetPath);
        if (level == null)
        {
            level = ScriptableObject.CreateInstance<LevelDefinition>();
            AssetDatabase.CreateAsset(level, assetPath);
        }

        level.waves.Clear();

        level.waves.Add(new WaveDefinition
        {
            name = "Wave 1 - Tutorial",
            duration = 40f,
            spawnMode = SpawnMode.Continuous,
            spawnInterval = 1.4f,
            spawnCount = 6,
            baseHp = 1,
            baseSpeed = 1.0f,
            baseXp = 1,
            bossHpMultiplier = 8f
        });

        level.waves.Add(new WaveDefinition
        {
            name = "Wave 2 - Random",
            duration = 45f,
            spawnMode = SpawnMode.Continuous,
            spawnInterval = 1.2f,
            spawnCount = 8,
            baseHp = 1,
            baseSpeed = 1.1f,
            baseXp = 1,
            bossHpMultiplier = 8f
        });

        level.waves.Add(new WaveDefinition
        {
            name = "Wave 3 - Random",
            duration = 50f,
            spawnMode = SpawnMode.Continuous,
            spawnInterval = 1.0f,
            spawnCount = 10,
            baseHp = 2,
            baseSpeed = 1.15f,
            baseXp = 2,
            bossHpMultiplier = 8f
        });

        level.waves.Add(new WaveDefinition
        {
            name = "Wave 4 - Burst",
            duration = 30f,
            spawnMode = SpawnMode.Burst,
            spawnInterval = 0.8f,
            spawnCount = 8,
            baseHp = 1,
            baseSpeed = 1.0f,
            baseXp = 1,
            bossHpMultiplier = 8f
        });

        level.waves.Add(new WaveDefinition
        {
            name = "Wave 5 - Boss",
            duration = 40f,
            spawnMode = SpawnMode.Boss,
            spawnInterval = 1.0f,
            spawnCount = 1,
            baseHp = 2,
            baseSpeed = 0.9f,
            baseXp = 5,
            bossHpMultiplier = 8f,
            bossSpeedMultiplier = 0.8f,
            bossScale = 1.5f
        });

        level.useDifficultyCurve = true;
        level.difficultyCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(120f, 1.4f),
            new Keyframe(240f, 1.8f)
        );

        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = level;
        EditorGUIUtility.PingObject(level);
    }
}
