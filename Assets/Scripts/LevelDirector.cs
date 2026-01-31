using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [SerializeField] private LevelDefinition level;
    [SerializeField] private Spawner spawner;
    [SerializeField] private bool loopWaves = true;

    private int waveIndex = -1;
    private float waveEndTime = 0f;
    private float elapsed = 0f;

    private void Start()
    {
        if (spawner == null) spawner = FindFirstObjectByType<Spawner>();
        StartNextWave();
    }

    private void Update()
    {
        if (level == null || spawner == null) return;
        if (level.waves == null || level.waves.Count == 0) return;

        elapsed += Time.deltaTime;
        spawner.SetDifficultyMultiplier(level.EvaluateDifficulty(elapsed));

        if (waveIndex < 0 || waveIndex >= level.waves.Count) return;

        if (Time.time >= waveEndTime)
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        if (level == null || spawner == null) return;
        if (level.waves == null || level.waves.Count == 0) return;

        if (loopWaves)
            waveIndex = (waveIndex + 1) % level.waves.Count;
        else
            waveIndex = Mathf.Clamp(waveIndex + 1, 0, level.waves.Count - 1);
        WaveDefinition wave = level.waves[waveIndex];

        spawner.ApplyWave(wave);
        waveEndTime = Time.time + Mathf.Max(1f, wave.duration);
    }
}
