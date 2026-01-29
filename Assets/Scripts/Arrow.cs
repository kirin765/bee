using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    private Rigidbody2D rb;
    public int damage = 1;
    public int pierceRemaining = 0; // 0 means no pierce
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bee"))
        {
            Bee bee = collision.GetComponent<Bee>();
            if (bee != null)
            {
                bool died = bee.TakeDamage(damage);
                if (died)
                {
                    // award xp handled by Bee
                }
            }

            if (pierceRemaining > 0)
            {
                pierceRemaining--;
                // continue flying
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + Vector2.up * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

}
