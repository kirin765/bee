using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float cullViewportPadding = 0.1f;

    private Rigidbody2D rb;
    private Camera cam;

    public float damage = 1f;
    public int pierceRemaining = 0; // 0 means no pierce

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bee")) return;

        Bee bee = collision.GetComponent<Bee>();
        if (bee != null) bee.TakeDamage(damage);

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
        }
        else
        {
            transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
        }

        if (cam == null) cam = Camera.main;
        if (cam == null) return;
        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        if (vp.z < 0f ||
            vp.x < -cullViewportPadding || vp.x > 1f + cullViewportPadding ||
            vp.y < -cullViewportPadding || vp.y > 1f + cullViewportPadding)
        {
            Destroy(gameObject);
        }
    }
}
