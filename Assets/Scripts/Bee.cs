using System.Collections;
using UnityEngine;

public class Bee : MonoBehaviour
{
    [SerializeField] private int hp = 1;
    [SerializeField] private int beeXp = 1;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Xp xp;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Hearts hearts;

    private Coroutine fallCo;
    private Rigidbody2D rb;
    private Collider2D col;

    private bool isAlive = true;
    private float halfHeight;
    private float deadLine;

    public Xp XpRef { get => xp; set => xp = value; }
    public GameOver GameOverRef { get => gameOver; set => gameOver = value; }
    public Hearts HeartsRef { get => hearts; set => hearts = value; }

    private void Awake()
    {
        halfHeight = Camera.main.orthographicSize;
        deadLine = -halfHeight - 0.5f;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 pos = rb.position;

        if (isAlive && pos.y < deadLine)
        {
            isAlive = false;
            if (hearts != null && hearts.IsInvincible)
            {
                Destroy(gameObject);
                return;
            }

            int heartIndex = hearts != null ? hearts.LossHeart() : 0;
            if (heartIndex == 0)
            {
                if (gameOver != null) gameOver.TurnOn();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        pos.y -= Time.fixedDeltaTime * speed;
        rb.MovePosition(pos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Arrow handling moved to Arrow.TakeDamage -> Bee.TakeDamage
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

    private IEnumerator Falling()
    {
        float duration = 3.0f;
        float passedTime = 0f;
        Vector3 baseScale = transform.localScale;

        while (passedTime < duration)
        {
            passedTime += Time.deltaTime;
            transform.localScale = baseScale * Mathf.Lerp(1.0f, 0f, Mathf.Clamp01(passedTime / duration));
            transform.Rotate(0f, 0f, 720f * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
