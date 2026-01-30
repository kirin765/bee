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

    void Start()
    {
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        left_limit_x = -halfHeight * cam.aspect;
        right_limit_x = halfHeight * cam.aspect;
        top_limit_y = halfHeight;
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
        if(passedTime > beeCooltime)
        {
            passedTime = 0f;

            float spawnY = top_limit_y + beeSpawnYOffset;
            MakeBee(new Vector3(Random.Range(left_limit_x, right_limit_x), spawnY, 0f));
        }
    }
}
