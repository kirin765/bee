using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bee"))
            Destroy(gameObject);
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
