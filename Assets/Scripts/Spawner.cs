using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Bee beePrefab;

    [Header("Legacy Timing")]
    [SerializeField] private float beeCooltime = 3.0f;

    [Header("Spawn Bounds")]
    [SerializeField] private float beeSpawnYOffset = 1.0f;

    [Header("Refs")]
    [SerializeField] private Xp xp;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Hearts hearts;

    [Header("Wave Runtime")]
    [SerializeField] private SpawnMode spawnMode = SpawnMode.Continuous;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private int baseHp = 1;
    [SerializeField] private float baseSpeed = 1.0f;
    [SerializeField] private int baseXp = 1;

    [Header("Random Lane")]
    [SerializeField] private int randomLaneCount = 5;
    [SerializeField] private float randomLaneJitter = 0.25f;

    private float leftLimitX;
    private float rightLimitX;
    private float topLimitY;

    private float passedTime = 0f;
    private float difficultyMultiplier = 1f;

    private int[] laneOrder;
    private int laneCursor = 0;

    private void Start()
    {
        InitCameraBounds();
        InitLaneOrder();
    }

    private void Update()
    {
        passedTime += Time.deltaTime;

        if (spawnMode == SpawnMode.Continuous)
        {
            float interval = spawnInterval > 0f ? spawnInterval : beeCooltime;
            if (passedTime > interval)
            {
                passedTime = 0f;
                SpawnOne();
            }
        }
    }

    public void SetDifficultyMultiplier(float value)
    {
        difficultyMultiplier = Mathf.Max(0.1f, value);
    }

    public void ApplyWave(WaveDefinition wave)
    {
        if (wave == null) return;

        spawnMode = wave.spawnMode;
        spawnInterval = wave.spawnInterval;
        spawnCount = Mathf.Max(1, wave.spawnCount);
        baseHp = Mathf.Max(1, wave.baseHp);
        baseSpeed = Mathf.Max(0.1f, wave.baseSpeed);
        baseXp = Mathf.Max(1, wave.baseXp);
        passedTime = 0f;

        if (spawnMode == SpawnMode.Burst)
        {
            for (int i = 0; i < spawnCount; i++) SpawnOne();
        }
    }

    public void MakeArrow(Vector3 pos)
    {
        if (arrowPrefab == null) return;
        Instantiate(arrowPrefab, pos, Quaternion.identity);
    }

    public void MakeArrow(Vector3 pos, int damage, int pierce, float xOffset)
    {
        if (arrowPrefab == null) return;
        Vector3 spawnPos = pos + new Vector3(xOffset, 0f, 0f);
        Arrow a = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        a.damage = damage;
        a.pierceRemaining = Mathf.Max(0, pierce - 1);
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

    private void InitCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        float halfHeight = cam.orthographicSize;
        leftLimitX = -halfHeight * cam.aspect;
        rightLimitX = halfHeight * cam.aspect;
        topLimitY = halfHeight;
    }

    private void SpawnOne()
    {
        float spawnY = topLimitY + beeSpawnYOffset;
        float x = GetLaneRandomX();
        MakeBeeWithStats(new Vector3(x, spawnY, 0f));
    }

    private Bee MakeBeeWithStats(Vector3 pos)
    {
        if (beePrefab == null) return null;
        Bee bee = Instantiate(beePrefab, pos, beePrefab.transform.rotation);
        bee.XpRef = xp;
        bee.HeartsRef = hearts;
        bee.GameOverRef = gameOver;

        int hp = Mathf.RoundToInt(baseHp * difficultyMultiplier);
        int xpVal = Mathf.RoundToInt(baseXp * difficultyMultiplier);
        float speed = baseSpeed;

        bee.Configure(hp, xpVal, speed, 1f);
        return bee;
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
