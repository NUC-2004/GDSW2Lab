using UnityEngine;

public class Coin : MonoBehaviour
{
    public float bounceForce = 8f;
    private Rigidbody2D rb;
    private bool collected = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Bounce up when spawned
        rb.linearVelocity = new Vector2(0, bounceForce);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (collected) return;
        if (col.CompareTag("Player"))
        {
            collected = true;
            // Add to score later
            Destroy(gameObject);
        }
    }
}