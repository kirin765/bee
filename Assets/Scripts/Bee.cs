using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bee : MonoBehaviour
{
    [SerializeField] private int hp = 1;
    [SerializeField] private int beeXp = 1;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Xp xp;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Hearts hearts;
    [SerializeField] private string beeLayerName = "Bee";
    
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject totalHpBar;

    private int maxHp;

    private Coroutine fallCoroutine;
    private Rigidbody2D rb;
    private Collider2D col;

    private bool isAlive = true;
    private float deathLineY;
    private static bool beeCollisionIgnored = false;

    private const float DeathLineOffset = 0.5f;
    private const float FallDuration = 3.0f;

    public Xp XpRef { get => xp; set => xp = value; }
    public GameOver GameOverRef { get => gameOver; set => gameOver = value; }
    public Hearts HeartsRef { get => hearts; set => hearts = value; }

    public void Configure(int newHp, int newXp, float newSpeed, float scale = 1f)
    {
        hp = Mathf.Max(1, newHp);
        beeXp = Mathf.Max(1, newXp);
        speed = Mathf.Max(0.1f, newSpeed);
        if (scale > 0f) transform.localScale = transform.localScale * scale;
    }

    private void Awake()
    {
        if (!beeCollisionIgnored)
        {
            int layer = LayerMask.NameToLayer(beeLayerName);
            if (layer >= 0) Physics2D.IgnoreLayerCollision(layer, layer, true);
            beeCollisionIgnored = true;
        }

        int beeLayer = LayerMask.NameToLayer(beeLayerName);
        if (beeLayer >= 0 && gameObject.layer != beeLayer)
        {
            gameObject.layer = beeLayer;
        }

        float halfHeight = Camera.main.orthographicSize;
        deathLineY = -halfHeight - DeathLineOffset;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (rb != null) rb.freezeRotation = true;
        if(hpBar != null)
        {
            hpBar.fillAmount = 1f;
        }
        maxHp = hp;
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 pos = rb.position;

        if (isAlive && pos.y < deathLineY)
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
        if(hpBar != null)
        {
            hpBar.fillAmount = Mathf.Clamp01((float)hp / (float)maxHp);
        }
        if (hp <= 0)
        {
            if(totalHpBar != null){
                totalHpBar.SetActive(false);
            }
            isAlive = false;
            col.enabled = false;
            if (xp != null) xp.AddXp(beeXp);
            if (fallCoroutine == null) fallCoroutine = StartCoroutine(Falling());
            
            return true;
        }

        return false;
    }

    private IEnumerator Falling()
    {
        float passedTime = 0f;
        Vector3 baseScale = transform.localScale;

        while (passedTime < FallDuration)
        {
            passedTime += Time.deltaTime;
            transform.localScale = baseScale * Mathf.Lerp(1.0f, 0f, Mathf.Clamp01(passedTime / FallDuration));
            transform.Rotate(0f, 0f, 720f * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
