using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Arrow arrowPrefab;

    [SerializeField]
    private Bee beePrefab;

    [SerializeField]
    private float beeCooltime = 3.0f;
    private float passedTime = 0f;
    private float left_limit_x;
    private float right_limit_x;
    private float top_limit_y;
    [SerializeField] private float beeSpawnYOffset = 1.0f;
    [SerializeField]
    private Xp xp;
    [SerializeField]
    private GameOver gameOver;
    [SerializeField]
    private Hearts hearts;

    [Header("Wave Runtime")]
    [SerializeField] private SpawnPattern spawnPattern = SpawnPattern.Random;
    [SerializeField] private SpawnMode spawnMode = SpawnMode.Continuous;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private float waveFrequency = 1.3f;
    [SerializeField, Range(0.1f, 1f)] private float waveAmplitude = 0.9f;
    [SerializeField] private int baseHp = 1;
    [SerializeField] private float baseSpeed = 1.0f;
    [SerializeField] private int baseXp = 1;
    [SerializeField, Range(0f, 1f)] private float eliteChance = 0.1f;
    [SerializeField] private float eliteHpMultiplier = 2.0f;
    [SerializeField] private float eliteSpeedMultiplier = 1.1f;
    [SerializeField] private float eliteScale = 1.15f;
    [SerializeField] private float bossHpMultiplier = 6.0f;
    [SerializeField] private float bossSpeedMultiplier = 0.8f;
    [SerializeField] private float bossScale = 1.5f;

    private bool zigLeft = true;
    private float waveTime = 0f;
    private bool bossSpawned = false;
    private float difficultyMultiplier = 1f;

    void Start()
    {
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        left_limit_x = -halfHeight * cam.aspect;
        right_limit_x = halfHeight * cam.aspect;
        top_limit_y = halfHeight;
    }

    public void SetDifficultyMultiplier(float value)
    {
        difficultyMultiplier = Mathf.Max(0.1f, value);
    }

    public void ApplyWave(WaveDefinition wave)
    {
        if (wave == null) return;
        spawnPattern = wave.pattern;
        spawnMode = wave.spawnMode;
        spawnInterval = wave.spawnInterval;
        spawnCount = Mathf.Max(1, wave.spawnCount);
        waveFrequency = Mathf.Max(0.1f, wave.waveFrequency);
        waveAmplitude = Mathf.Clamp(wave.waveAmplitude, 0.1f, 1f);
        baseHp = Mathf.Max(1, wave.baseHp);
        baseSpeed = Mathf.Max(0.1f, wave.baseSpeed);
        baseXp = Mathf.Max(1, wave.baseXp);
        eliteChance = Mathf.Clamp01(wave.eliteChance);
        eliteHpMultiplier = Mathf.Max(1f, wave.eliteHpMultiplier);
        eliteSpeedMultiplier = Mathf.Max(0.1f, wave.eliteSpeedMultiplier);
        eliteScale = Mathf.Max(0.1f, wave.eliteScale);
        bossHpMultiplier = Mathf.Max(1f, wave.bossHpMultiplier);
        bossSpeedMultiplier = Mathf.Max(0.1f, wave.bossSpeedMultiplier);
        bossScale = Mathf.Max(0.1f, wave.bossScale);

        passedTime = 0f;
        waveTime = 0f;
        bossSpawned = false;

        if (spawnMode == SpawnMode.Burst)
        {
            for (int i = 0; i < spawnCount; i++) SpawnOne(false);
        }
        else if (spawnMode == SpawnMode.Boss)
        {
            SpawnOne(true);
            bossSpawned = true;
        }
    }

    public void MakeArrow(Vector3 pos)
    {
        if (arrowPrefab != null)
            Instantiate(arrowPrefab, pos, Quaternion.identity);
    }

    public void MakeArrow(Vector3 pos, int damage, int pierce, float xOffset)
    {
        if (arrowPrefab == null) return;
        Vector3 spawnPos = pos + new Vector3(xOffset, 0f, 0f);
        Arrow a = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        a.damage = damage;
        a.pierceRemaining = Mathf.Max(0, pierce - 1); // pierceRemaining means how many extra hits it can make
    }

    public void MakeArrow(Vector3 pos, int damage, int pierce, Vector2 offset, float angleDeg)
    {
        if (arrowPrefab == null) return;
        Vector3 spawnPos = pos + new Vector3(offset.x, offset.y, 0f);
        Quaternion rot = Quaternion.Euler(0f, 0f, angleDeg);
        Arrow a = Instantiate(arrowPrefab, spawnPos, rot);
        a.damage = damage;
        a.pierceRemaining = Mathf.Max(0, pierce - 1);
    }

    public void MakeBee(Vector3 pos)
    {
        if (beePrefab == null) return;
        Bee bee = Instantiate(beePrefab, pos, beePrefab.transform.rotation);
        bee.XpRef = xp;
        bee.HeartsRef = hearts;
        bee.GameOverRef = gameOver;
    }

    void Update()
    {
        passedTime += Time.deltaTime;
        waveTime += Time.deltaTime;

        if (spawnMode == SpawnMode.Continuous)
        {
            float interval = spawnInterval > 0f ? spawnInterval : beeCooltime;
            if (passedTime > interval)
            {
                passedTime = 0f;
                SpawnOne(false);
            }
        }
        else if (spawnMode == SpawnMode.Boss && !bossSpawned)
        {
            SpawnOne(true);
            bossSpawned = true;
        }
    }

    private void SpawnOne(bool forceBoss)
    {
        float spawnY = top_limit_y + beeSpawnYOffset;
        float x = GetSpawnX();
        Bee bee = MakeBeeWithStats(new Vector3(x, spawnY, 0f), forceBoss);
        if (bee != null && spawnPattern == SpawnPattern.Wave)
        {
            // Wave pattern advances by spawn to avoid clustering at same phase
            waveTime += 0.3f;
        }
    }

    private float GetSpawnX()
    {
        switch (spawnPattern)
        {
            case SpawnPattern.ZigZag:
                float zx = zigLeft ? left_limit_x : right_limit_x;
                zigLeft = !zigLeft;
                return zx + Random.Range(-0.2f, 0.2f);
            case SpawnPattern.Wave:
                float halfWidth = right_limit_x;
                float amp = Mathf.Max(0.1f, halfWidth * waveAmplitude);
                return Mathf.Sin(waveTime * waveFrequency) * amp;
            case SpawnPattern.Random:
            default:
                return Random.Range(left_limit_x, right_limit_x);
        }
    }

    private Bee MakeBeeWithStats(Vector3 pos, bool forceBoss)
    {
        if (beePrefab == null) return null;
        Bee bee = Instantiate(beePrefab, pos, beePrefab.transform.rotation);
        bee.XpRef = xp;
        bee.HeartsRef = hearts;
        bee.GameOverRef = gameOver;

        bool isElite = !forceBoss && Random.value < eliteChance;
        float hpMult = forceBoss ? bossHpMultiplier : (isElite ? eliteHpMultiplier : 1f);
        float speedMult = forceBoss ? bossSpeedMultiplier : (isElite ? eliteSpeedMultiplier : 1f);
        float scale = forceBoss ? bossScale : (isElite ? eliteScale : 1f);

        int hp = Mathf.RoundToInt(baseHp * difficultyMultiplier * hpMult);
        int xpVal = Mathf.RoundToInt(baseXp * difficultyMultiplier);
        float speed = baseSpeed * speedMult;

        bee.Configure(hp, xpVal, speed, scale);
        return bee;
    }
}
