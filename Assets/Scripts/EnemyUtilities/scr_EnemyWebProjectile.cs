using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class scr_EnemyWebProjectile : MonoBehaviour
{
    [SerializeField] private Tile webTile;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            Tilemap tilemap = null;
            Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();

            foreach (var map in tilemaps)
            {
                if (map.name == "Setting" && map.gameObject.scene.name == gameObject.scene.name)
                {
                    tilemap = map;
                    break;
                }
            }

            Vector3Int tilePosition = tilemap.WorldToCell(transform.position);
            TileBase tile = tilemap.GetTile(tilePosition);

            if (tile == null)
            {
                tilemap.SetTile(tilePosition, webTile);
            }
            
            Destroy(gameObject);
        }
    }
}
