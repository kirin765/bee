using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    public Arrow arrowPrefab;
    
    [SerializeField]
    public Bee beePrefab;

    [SerializeField]
    private float beeCooltime = 3.0f;
    private float passedTime = 0f;
    private float left_limit_x;
    private float right_limit_x;
    [SerializeField]
    public Xp xp;
    [SerializeField]
    public GameOver gameOver;
    [SerializeField]
    public Hearts hearts;

    void Start()
    {
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        left_limit_x = -halfHeight * cam.aspect;
        right_limit_x = halfHeight * cam.aspect;
    }

    public void MakeArrow(Vector3 pos)
    {
        Instantiate(arrowPrefab, pos, Quaternion.identity);
    }

    public void MakeBee(Vector3 pos)
    {
        Bee bee = Instantiate(beePrefab, pos, beePrefab.transform.rotation);
        bee.xp = xp;
        bee.hearts = hearts;
        bee.gameOver = gameOver;
    }

    void Update()
    {
        passedTime += Time.deltaTime;
        if(passedTime > beeCooltime)
        {
            passedTime = 0f;

            MakeBee(new Vector3(Random.Range(left_limit_x, right_limit_x), 15.0f, 0f));
        }
    }
}
