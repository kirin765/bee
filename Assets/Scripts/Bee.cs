using System.Collections;
using UnityEngine;

public class Bee : MonoBehaviour
{

    [SerializeField]
    private int hp = 1;
    [SerializeField]
    private int beeXp = 1;
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private Xp xp;
    [SerializeField]
    private GameOver gameOver;
    private Coroutine fallCo;
    private Rigidbody2D rb;
    [SerializeField]
    private Hearts hearts;
    public Xp XpRef { get => xp; set => xp = value; }
    public GameOver GameOverRef { get => gameOver; set => gameOver = value; }
    public Hearts HeartsRef { get => hearts; set => hearts = value; }
    private bool isAlive = true;
    float halfHeight;
    float deadLine;
    private Collider2D col;

    void Awake()
    {
        halfHeight = Camera.main.orthographicSize;
        deadLine = -halfHeight - 0.5f;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector2 pos = rb.position;

        if(isAlive && (pos.y < deadLine))
        {
            isAlive = false;
            int heart_cnt = hearts != null ? hearts.LossHeart() : 0;
            if (heart_cnt == 0)
            {
                if (gameOver != null) gameOver.TurnOn();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        pos.y -= Time.deltaTime * speed;


        rb.MovePosition(pos);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // arrow handling moved to Arrow.TakeDamage -> Bee.TakeDamage
    }

        // Returns true if the bee died from this damage
        public bool TakeDamage(int damage)
        {
            if (!isAlive) return false;
            hp -= damage;
            if (hp <= 0)
            {
                isAlive = false;
                col.enabled = false;
                if (xp != null) xp.AddXp(beeXp);
                if (fallCo == null) StartCoroutine(Falling());
                return true;
            }
            return false;
        }

    IEnumerator Falling()
    {
        float duration = 3.0f;
        float passedTime = 0f;
        Vector3 baseScale = transform.localScale;
        while(passedTime < duration)
        {
            passedTime += Time.deltaTime;
            transform.localScale = baseScale * Mathf.Lerp(1.0f, 0f, Mathf.Clamp01(passedTime/duration));
            transform.Rotate(0f,0f,720f * Time.deltaTime);
            
            yield return null;
        }
        Destroy(gameObject);
    }
}
