using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fire : MonoBehaviour
{
    private Vector3Int position;
    //private TileData data;
    private scr_TilemapManager tilemapManager;
    private Tilemap tilemap;

    [SerializeField] private float burnTimeCounter;
    [SerializeField] private float spreadIntervalCounter;

    public void StartBurning(Vector3Int position, TileData data, scr_TilemapManager tilemapManager, Tilemap tilemap)
    {
        this.position = position;
        //this.data = data;
        this.tilemapManager = tilemapManager;
        this.tilemap = tilemap;

        burnTimeCounter = data.burnTime;
        spreadIntervalCounter = data.spreadInterval;
    }

    void Update()
    {
        burnTimeCounter -= Time.deltaTime;

        if (burnTimeCounter <= 0)
        {
            tilemapManager.FinishedBurning(position, tilemap);
            Destroy(gameObject);
        }

        spreadIntervalCounter -= Time.deltaTime;

        if (spreadIntervalCounter <= 0)
        {
            spreadIntervalCounter = 0;
            tilemapManager.TryToSpread(position, tilemap);
        }
    }

    private void OnDestroy()
    {
        tilemapManager.activeFires.Remove(position);
    }
}
