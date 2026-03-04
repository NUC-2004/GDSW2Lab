using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float runSpeed = 14f;
    public float jumpForce = 12f;
    public float slideFriction = 0.95f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private Vector3 spawnPoint;
    private SpriteRenderer sr;
    private bool canMove = true;

    private float moveInput;
    private bool isSliding = false;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spawnPoint = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isSliding)
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

            if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isGrounded)
            {
                isSliding = true;
            }
        }

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
            isSliding = false;
        }

        if (transform.position.y < -10f)
        {
            transform.position = spawnPoint;
            rb.velocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (!isSliding)
        {
            if (canMove)
            {
                rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x * slideFriction, rb.velocity.y);

            if (Mathf.Abs(rb.velocity.x) < 0.5f)
            {
                isSliding = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
            
                if (moveInput != 0) 
                {
                    canMove = false; 
                }
            }
        }
        if (!canMove && Input.GetAxisRaw("Horizontal") == 0)
        {
            canMove = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }
}