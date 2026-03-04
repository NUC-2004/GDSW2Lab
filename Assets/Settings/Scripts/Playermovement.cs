using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private Vector3 spawnPoint;
    private SpriteRenderer sr;

    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spawnPoint = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        anim.SetBool("isWalking", moveInput != 0);
        anim.SetBool("isGrounded", isGrounded);

        if (moveInput > 0.1f)
            sr.flipX = false;
        else if (moveInput < -0.1f)
            sr.flipX = true;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        if (transform.position.y < -10f)
        {
            transform.position = spawnPoint;
            rb.velocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }
}