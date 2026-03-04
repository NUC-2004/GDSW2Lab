using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class QuestionBlock : MonoBehaviour
{
    private Tilemap tilemap;
    public TileBase emptyBlockTile;
    public GameObject coinPrefab;
    public GameObject mushroomPrefab;
    public bool hasMushroom = false;

    // Track which tiles have been hit
    private HashSet<Vector3Int> hitTiles = new HashSet<Vector3Int>();

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                Vector3Int cellPos = tilemap.WorldToCell(contact.point);
                cellPos.y += 1;

                // Check if tile exists AND hasn't been hit before
                if (tilemap.GetTile(cellPos) != null &&
                    !hitTiles.Contains(cellPos))
                {
                    hitTiles.Add(cellPos);

                    Vector3 spawnPos = tilemap.CellToWorld(cellPos)
                        + new Vector3(0.5f, 1.5f, 0);

                    if (hasMushroom && mushroomPrefab != null)
                        Instantiate(mushroomPrefab, spawnPos, Quaternion.identity);
                    else if (coinPrefab != null)
                        Instantiate(coinPrefab, spawnPos, Quaternion.identity);

                    if (emptyBlockTile != null)
                        tilemap.SetTile(cellPos, emptyBlockTile);
                    else
                        tilemap.SetTile(cellPos, null);
                }
            }
        }
    }
}