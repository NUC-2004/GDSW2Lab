using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float acceleration = 15f;
    public float deceleration = 20f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public float lowJumpMultiplier = 5f;
    public float fallMultiplier = 4f;
    public float maxFallSpeed = -20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private bool isGrounded;
    private bool isDead = false;
    private bool isBig = false;
    private Vector3 spawnPoint;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        spawnPoint = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer
        );

        anim.SetBool("isWalking", moveInput != 0);
        anim.SetBool("isGrounded", isGrounded);

        if (moveInput > 0.1f) sr.flipX = false;
        else if (moveInput < -0.1f) sr.flipX = true;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (rb.linearVelocity.y < maxFallSpeed)
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x, maxFallSpeed
            );

        if (transform.position.y < -10f) Respawn();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = Mathf.Abs(targetSpeed) > 0.1f
            ? acceleration : deceleration;
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x + movement,
            rb.linearVelocity.y
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            isGrounded = true;
    }

    public void GrowBig()
    {
        if (isBig) return;
        isBig = true;
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    }

    public void TakeDamage()
    {
        if (isBig)
        {
            isBig = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
            return;
        }

        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        sr.color = Color.red;
        Invoke("Respawn", 1f);
    }

    void Respawn()
    {
        isDead = false;
        isBig = false;
        rb.isKinematic = false;
        sr.color = Color.white;
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.position = spawnPoint;
        rb.linearVelocity = Vector2.zero;
    }
}