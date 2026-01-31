using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnMode { Continuous, Burst, Boss }

[Serializable]
public class WaveDefinition
{
    public string name;
    public float duration = 15f;
    public SpawnMode spawnMode = SpawnMode.Continuous;
    public float spawnInterval = 1.2f;
    public int spawnCount = 5;

    public int baseHp = 1;
    public float baseSpeed = 1.0f;
    public int baseXp = 1;

    public float bossHpMultiplier = 6.0f;
    public float bossSpeedMultiplier = 0.8f;
    public float bossScale = 1.5f;
}

[CreateAssetMenu(menuName = "Bee/Level Definition", fileName = "LevelDefinition")]
public class LevelDefinition : ScriptableObject
{
    public List<WaveDefinition> waves = new List<WaveDefinition>();

    public bool useDifficultyCurve = true;
    public AnimationCurve difficultyCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(120f, 1.4f),
        new Keyframe(240f, 1.8f)
    );

    public float EvaluateDifficulty(float elapsed)
    {
        if (!useDifficultyCurve || difficultyCurve == null) return 1f;
        return Mathf.Max(0.1f, difficultyCurve.Evaluate(elapsed));
    }
}
