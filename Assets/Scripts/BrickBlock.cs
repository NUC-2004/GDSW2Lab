using UnityEngine;
using UnityEngine.Tilemaps;

public class BrickBlock : MonoBehaviour
{
    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        // Check if player is below the tilemap
        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                // Convert hit position to tile coordinates
                Vector3Int cellPos = tilemap.WorldToCell(contact.point);
                // Check one tile above contact point
                cellPos.y += 1;

                if (tilemap.GetTile(cellPos) != null)
                {
                    tilemap.SetTile(cellPos, null);

                    // Bounce player down slightly
                    Rigidbody2D playerRb =
                        col.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                        playerRb.linearVelocity = new Vector2(
                            playerRb.linearVelocity.x, -2f
                        );
                }
            }
        }
    }
}