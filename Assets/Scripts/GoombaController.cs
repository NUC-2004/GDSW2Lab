using UnityEngine;

public class GoombaController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;

    [Header("Detection")]
    public Transform wallCheck;
    public Transform edgeCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.1f;

    private Rigidbody2D rb;
    private bool isDead = false;
    private bool movingRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead) return;

        // Patrol left and right
        rb.linearVelocity = new Vector2(
            movingRight ? moveSpeed : -moveSpeed,
            rb.linearVelocity.y
        );

        // Check for wall
        bool hitWall = Physics2D.OverlapCircle(
            wallCheck.position, checkRadius, groundLayer
        );



        if (hitWall)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDead) return;

        if (col.gameObject.CompareTag("Player"))
        {
            bool stompedFromAbove =
                col.transform.position.y > transform.position.y + 0.2f;

            if (stompedFromAbove)
            {
                GetStomped();
                // Bounce player up after stomp
                Rigidbody2D playerRb =
                    col.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                    playerRb.linearVelocity = new Vector2(
                        playerRb.linearVelocity.x, 8f
                    );
            }
            else
            {
                // Player takes damage
                col.gameObject.SendMessage("TakeDamage",
                    SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void GetStomped()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.3f);
    }
}