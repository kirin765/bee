using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Bee beePrefab;

    [Header("Spawn Bounds")]
    [SerializeField] private float beeSpawnYOffset = 1.0f;

    [Header("Refs")]
    [SerializeField] private Xp xp;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Hearts hearts;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnInterval = 4.0f;
    [SerializeField] private float maxSpawnInterval = 8.0f;
    [SerializeField] private int minSpawnCount = 1;
    [SerializeField] private int maxSpawnCount = 2;

    [Header("Base Stats")]
    [SerializeField] private float baseHp = 1f;
    [SerializeField] private float baseSpeed = 1.0f;
    [SerializeField] private float baseXp = 1f;

    [Header("Scaling")]
    [SerializeField] private float statIncreaseInterval = 20.0f;
    [SerializeField] private float statIncreaseMultiplier = 1.3f;

    [Header("Boss Bee")]
    [SerializeField] private float bossSpawnInterval = 60.0f;
    [SerializeField] private float bossSpeedMultiplier = 0.5f;
    [SerializeField] private float bossHpMultiplier = 10.0f;
    [SerializeField] private float bossScale = 2.0f;
    [SerializeField] private float bossSpawnIntervalOverride = 5.0f;
    [SerializeField] private int speedFreezeLevel = 15;
    [SerializeField] private int spawnIntervalScaleLevel = 15;
    [SerializeField] private float spawnIntervalScaleInterval = 30.0f;
    [SerializeField] private float spawnIntervalScaleMultiplier = 0.8f;

    [Header("Random Lane")]
    [SerializeField] private int randomLaneCount = 5;
    [SerializeField] private float randomLaneJitter = 0.25f;

    private float leftLimitX;
    private float rightLimitX;
    private float topLimitY;

    private float spawnTimer = 0f;
    private float nextSpawnInterval = 2f;
    private float statTimer = 0f;
    private float bossTimer = 0f;
    private float statMultiplier = 1f;
    private int bossAliveCount = 0;
    private bool speedMultiplierFrozen = false;
    private float speedMultiplierCap = 1f;
    private float spawnIntervalScaleTimer = 0f;
    private float spawnIntervalMultiplier = 1f;

    private int[] laneOrder;
    private int laneCursor = 0;

    private void Start()
    {
        InitCameraBounds();
        InitLaneOrder();
        RollNextSpawnInterval();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        spawnTimer += dt;
        statTimer += dt;
        if (bossAliveCount == 0)
        {
            bossTimer += dt;
        }
        else
        {
            bossTimer = 0f;
        }

        if (statIncreaseInterval > 0f && statTimer >= statIncreaseInterval)
        {
            int steps = Mathf.FloorToInt(statTimer / statIncreaseInterval);
            statTimer -= steps * statIncreaseInterval;
            statMultiplier *= Mathf.Pow(statIncreaseMultiplier, steps);
        }

        int level = GetPlayerLevel();
        if (!speedMultiplierFrozen && level > speedFreezeLevel)
        {
            speedMultiplierFrozen = true;
            speedMultiplierCap = statMultiplier;
        }

        if (level > spawnIntervalScaleLevel && spawnIntervalScaleInterval > 0f)
        {
            spawnIntervalScaleTimer += dt;
            if (spawnIntervalScaleTimer >= spawnIntervalScaleInterval)
            {
                int steps = Mathf.FloorToInt(spawnIntervalScaleTimer / spawnIntervalScaleInterval);
                spawnIntervalScaleTimer -= steps * spawnIntervalScaleInterval;
                spawnIntervalMultiplier *= Mathf.Pow(spawnIntervalScaleMultiplier, steps);
            }
        }

        if (bossAliveCount == 0 && bossSpawnInterval > 0f && bossTimer >= bossSpawnInterval)
        {
            bossTimer = 0f;
            SpawnBoss();
        }

        if (bossAliveCount > 0)
        {
            nextSpawnInterval = bossSpawnIntervalOverride;
        }

        if (spawnTimer >= nextSpawnInterval)
        {
            spawnTimer = 0f;
            int minCount = Mathf.Max(1, minSpawnCount);
            int maxCount = Mathf.Max(minCount, maxSpawnCount);
            int count = Random.Range(minCount, maxCount + 1);
            for (int i = 0; i < count; i++) SpawnNormal();
            RollNextSpawnInterval();
        }
    }

    public void MakeArrow(Vector3 pos)
    {
        if (arrowPrefab == null) return;
        Instantiate(arrowPrefab, pos, Quaternion.identity);
    }

    public void MakeArrow(Vector3 pos, float damage, int pierce, float xOffset)
    {
        if (arrowPrefab == null) return;
        Vector3 spawnPos = pos + new Vector3(xOffset, 0f, 0f);
        Arrow a = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        a.damage = damage;
        a.pierceRemaining = Mathf.Max(0, pierce - 1);
    }

    public void MakeArrow(Vector3 pos, float damage, int pierce, Vector2 offset, float angleDeg)
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

    private void InitCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        float halfHeight = cam.orthographicSize;
        leftLimitX = -halfHeight * cam.aspect;
        rightLimitX = halfHeight * cam.aspect;
        topLimitY = halfHeight;
    }

    private void SpawnNormal()
    {
        float spawnY = topLimitY + beeSpawnYOffset;
        float x = GetLaneRandomX();
        float hp = baseHp * statMultiplier;
        float xpVal = baseXp * statMultiplier;
        float speedMultiplier = speedMultiplierFrozen ? speedMultiplierCap : statMultiplier;
        float speed = baseSpeed * speedMultiplier;
        MakeBeeWithStats(new Vector3(x, spawnY, 0f), hp, xpVal, speed, 1f);
    }

    private void SpawnBoss()
    {
        float spawnY = topLimitY + beeSpawnYOffset;
        float x = GetLaneRandomX();
        float hp = baseHp * statMultiplier * bossHpMultiplier;
        float xpVal = baseXp * statMultiplier;
        float speedMultiplier = speedMultiplierFrozen ? speedMultiplierCap : statMultiplier;
        float speed = baseSpeed * speedMultiplier * bossSpeedMultiplier;
        Bee boss = MakeBeeWithStats(new Vector3(x, spawnY, 0f), hp, xpVal, speed, bossScale);
        if (boss != null)
        {
            boss.IsBoss = true;
            boss.OnDeath += HandleBeeDeath;
            bossAliveCount++;
        }
    }

    private void RollNextSpawnInterval()
    {
        if (bossAliveCount > 0)
        {
            nextSpawnInterval = bossSpawnIntervalOverride;
            return;
        }
        float min = Mathf.Max(0.1f, minSpawnInterval);
        float max = Mathf.Max(min, maxSpawnInterval);
        float interval = Random.Range(min, max);
        interval *= spawnIntervalMultiplier;
        nextSpawnInterval = Mathf.Max(0.1f, interval);
    }

    private Bee MakeBeeWithStats(Vector3 pos, float hp, float xpVal, float speed, float scale)
    {
        if (beePrefab == null) return null;
        Bee bee = Instantiate(beePrefab, pos, beePrefab.transform.rotation);
        bee.XpRef = xp;
        bee.HeartsRef = hearts;
        bee.GameOverRef = gameOver;

        bee.Configure(hp, xpVal, speed, scale);
        return bee;
    }

    private void HandleBeeDeath(Bee bee)
    {
        if (bee == null || !bee.IsBoss) return;
        bee.OnDeath -= HandleBeeDeath;
        bossAliveCount = Mathf.Max(0, bossAliveCount - 1);
        bossTimer = 0f;
        RollNextSpawnInterval();
    }

    private int GetPlayerLevel()
    {
        return xp != null ? xp.Level : 0;
    }

    private void InitLaneOrder()
    {
        int lanes = Mathf.Max(2, randomLaneCount);
        laneOrder = new int[lanes];
        for (int i = 0; i < lanes; i++) laneOrder[i] = i;
        ShuffleLanes();
        laneCursor = 0;
    }

    private void ShuffleLanes()
    {
        if (laneOrder == null) return;
        for (int i = laneOrder.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (laneOrder[i], laneOrder[j]) = (laneOrder[j], laneOrder[i]);
        }
    }

    private float GetLaneRandomX()
    {
        int lanes = Mathf.Max(2, randomLaneCount);
        if (laneOrder == null || laneOrder.Length != lanes)
        {
            InitLaneOrder();
        }

        if (laneCursor >= laneOrder.Length)
        {
            ShuffleLanes();
            laneCursor = 0;
        }

        int lane = laneOrder[laneCursor++];
        float t = (laneOrder.Length == 1) ? 0.5f : (float)lane / (laneOrder.Length - 1);
        float x = Mathf.Lerp(leftLimitX, rightLimitX, t);
        float laneWidth = (rightLimitX - leftLimitX) / laneOrder.Length;
        float jitter = Mathf.Clamp(randomLaneJitter, 0f, 1f) * laneWidth;
        return x + Random.Range(-jitter, jitter);
    }
}
