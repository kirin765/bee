using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private Rigidbody2D rb;

    public int damage = 1;
    public int pierceRemaining = 0; // 0 means no pierce

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bee")) return;

        Bee bee = collision.GetComponent<Bee>();
        if (bee != null)
        {
            bee.TakeDamage(damage);
        }

        if (pierceRemaining > 0)
        {
            pierceRemaining--;
            return;
        }

        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
            return;
        }

        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
    }
}
