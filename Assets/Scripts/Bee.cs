using System.Collections;
using UnityEngine;

public class Bee : MonoBehaviour
{

    [SerializeField]
    public int beeXp = 1;
    [SerializeField]
    private float speed = 1.0f;
    public Xp xp;
    public GameOver gameOver;
    private Coroutine fallCo;
    private Rigidbody2D rb;
    public Hearts hearts;
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
            int heart_cnt = hearts.LossHeart();
            if (heart_cnt == 0)
            {
                gameOver.TurnOn();
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
        
        if(collision.CompareTag("Arrow"))
        {
            xp.AddXp(beeXp);
            col.enabled = false;
            isAlive=false;

            if(fallCo == null)
                StartCoroutine(Falling());
        }
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
