using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public float moveSpeed = 2f;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public float checkRadius = 0.1f;
    private Rigidbody2D rb;
    private bool movingRight = true;
    private bool isActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Pop up from block
        rb.linearVelocity = new Vector2(0, 5f);
        Invoke("Activate", 0.4f);
    }

    void Activate()
    {
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        rb.linearVelocity = new Vector2(
            movingRight ? moveSpeed : -moveSpeed,
            rb.linearVelocity.y
        );

        bool hitWall = Physics2D.OverlapCircle(
            wallCheck.position, checkRadius, groundLayer
        );

        if (hitWall)
        {
            movingRight = !movingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.gameObject.GetComponent<PlayerController>()?.GrowBig();
            Destroy(gameObject);
        }
    }
}