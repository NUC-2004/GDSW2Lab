using UnityEngine;

public class GoombaController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;

    [Header("Detection")]
    public Transform wallCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.1f;

    [Header("Activation")]
    public float activationDistance = 15f;
    private Transform player;
    private bool isActivated = false;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;
    private bool movingRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead) return;

        // Only activate when player is nearby
        if (!isActivated)
        {
            if (player != null && Vector2.Distance(
                transform.position, player.position) < activationDistance)
                isActivated = true;
            else
                return;
        }

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
                col.transform.position.y > transform.position.y + 0.1f;

            if (stompedFromAbove)
            {
                GetStomped();
                Rigidbody2D playerRb =
                    col.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                    playerRb.linearVelocity = new Vector2(
                        playerRb.linearVelocity.x, 8f
                    );
            }
            else
            {
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
        if (anim != null)
            anim.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.5f);
    }
}