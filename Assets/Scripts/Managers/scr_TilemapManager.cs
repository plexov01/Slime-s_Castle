using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class scr_TilemapManager : MonoBehaviour
{
    public static scr_TilemapManager instance = null;

    [SerializeField] private TileData[] tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles = new Dictionary<TileBase, TileData>();

    private List<KeyValuePair<string, Tilemap>> tilemaps = new List<KeyValuePair<string, Tilemap>>();

    [System.Serializable]
    public struct Maps
    {
        public string name;
        public Tilemap map;
    }

    public List<Maps> currentTilemaps = new List<Maps>();

    [SerializeField] private Fire fire;
    public List<Vector3Int> activeFires = new List<Vector3Int>();

    private void Awake()
    {
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    /*public void AddTilemap(Tilemap tilemap)
    {
        string nameScene = tilemap.gameObject.scene.name;

        if (tilemap != null)
        {
            tilemaps.Add(new KeyValuePair<string, Tilemap>(nameScene, tilemap));
            print("added tilemap; key = " + nameScene + " tilemap = " + tilemap + " count = " + tilemaps.Count);

            var item = new Maps
            {
                name = nameScene,
                map = tilemap
            };

            currentTilemaps.Add(item);
        }
    }
    
    public void RemoveTilemap(Tilemap tilemap)
    {
        string nameScene = tilemap.gameObject.scene.name;

        tilemaps.Remove(new KeyValuePair<string, Tilemap>(nameScene, tilemap));

        var item = new Maps
        {
            name = nameScene,
            map = tilemap
        };

        currentTilemaps.Remove(item);
        print("removed tilemap; key = " + nameScene + " count = " + tilemaps.Count);

    }*/

    public void AddTilemap(string nameScene)
    {
        bool containsTilemap = false;

        foreach (KeyValuePair<string, Tilemap> kvp in tilemaps)
        {
            if (kvp.Key == nameScene)
            {
                containsTilemap = true;
                break;
            }
        }

        if (!containsTilemap)
        {
            Tilemap[] activeTilemaps = FindObjectsOfType<Tilemap>();

            for (int i = 0; i < activeTilemaps.Length; i++)
            {
                if (activeTilemaps[i].name == "Setting" && activeTilemaps[i].gameObject.scene.name == nameScene)
                {
                    Tilemap tilemap = activeTilemaps[i];

                    if (tilemap != null)
                    {
                        tilemaps.Add(new KeyValuePair<string, Tilemap>(nameScene, tilemap));
                        print("added tilemap; key = " + nameScene + " tilemap = " + tilemap + " count = " + tilemaps.Count);

                        var item = new Maps
                        {
                            name = nameScene,
                            map = tilemap
                        };

                        currentTilemaps.Add(item);

                        break;
                    }
                }
            }
        }
    }

    public void RemoveTilemap(string nameScene)
    {
        foreach (KeyValuePair<string, Tilemap> kvp in tilemaps)
        {
            if (kvp.Key == nameScene)
            {
                int i = tilemaps.IndexOf(kvp);
                tilemaps.RemoveAt(i);
                currentTilemaps.RemoveAt(i);
                print("removed tilemap; key = " + nameScene + " count = " + tilemaps.Count);
                break;
            }
        }
    }

    public float GetTileMovementSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition;
        TileBase tile;

        foreach (KeyValuePair<string, Tilemap> kvp in tilemaps)
        {
            if (kvp.Value != null)
            {
                gridPosition = kvp.Value.WorldToCell(worldPosition);
                tile = kvp.Value.GetTile(gridPosition);

                if (tile != null && dataFromTiles.ContainsKey(tile))
                {
                    return dataFromTiles[tile].movementSpeed;
                }
            }
        }

        return 1f;
    }

    public bool GetTileCanJump(Vector2 worldPosition)
    {
        Vector3Int gridPosition;
        TileBase tile;

        foreach (KeyValuePair<string, Tilemap> kvp in tilemaps)
        {
            if (kvp.Value != null)
            {
                gridPosition = kvp.Value.WorldToCell(worldPosition);
                tile = kvp.Value.GetTile(gridPosition);

                if (tile != null && dataFromTiles.ContainsKey(tile))
                {
                    return dataFromTiles[tile].canJump;
                }
            }
        }

        return true;
    }

    public TileData GetTileData(Vector3Int tilePosition, Tilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(tilePosition);

        if (tile != null && dataFromTiles.ContainsKey(tile))
        {
            return dataFromTiles[tile];
        }

        return null;
    }

    public void FinishedBurning(Vector3Int position, Tilemap tilemap)
    {
        print("Finished burning. Position: " + position);
        tilemap.SetTile(position, null);
        activeFires.Remove(position);
    }

    public void TryToSpread(Vector3Int position, Tilemap tilemap)
    {
        for (int x = position.x - 1; x <= position.x + 1; x++)
        {
            for (int y = position.y - 1; y <= position.y + 1; y++)
            {
                TryToBurnTile(new Vector3Int(x, y, 0));
            }
        }

        void TryToBurnTile(Vector3Int tilePosition)
        {
            if (activeFires.Contains(tilePosition))
            {
                return;
            }

            TileData data = GetTileData(tilePosition, tilemap);

            if (data != null && data.canBurn)
            {
                if (Random.Range(0f, 100f) <= data.spreadChance)
                {
                    SetTileOnFire(tilePosition, tilemap);
                }
            }
        }
    }

    public void SetTileOnFire(Vector3 position)
    {
        foreach (KeyValuePair<string, Tilemap> kvp in tilemaps)
        {
            Tilemap tilemap = kvp.Value;
            Vector3Int tilePosition = tilemap.WorldToCell(position);
            TileBase tile = tilemap.GetTile(tilePosition);
            TileData data = GetTileData(tilePosition, tilemap);
            //print("distance between player and tile: " + Vector2.Distance(position, kvp.Value.GetCellCenterWorld(kvp.Value.WorldToCell(position))));
            //print("tile: " + tile + ". Position: " + tilePosition);

            if (data != null && data.canBurn && tile != null)
            {
                if (activeFires.Contains(tilePosition))
                {
                    return;
                }

                print("Set on fire. Position:" + tilePosition + ". Tile: " + tile + ". Scene: " + kvp.Key);
                Fire newFire = Instantiate(fire);
                newFire.transform.parent = tilemap.transform;
                newFire.transform.position = tilemap.GetCellCenterWorld(tilePosition);
                newFire.StartBurning(tilePosition, data, this, tilemap);
                activeFires.Add(tilePosition);
                break;
            }
        }
    }

    private void SetTileOnFire(Vector3Int tilePosition, Tilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(tilePosition);
        TileData data = GetTileData(tilePosition, tilemap);

        if (data != null && tile != null)
        {
            if (activeFires.Contains(tilePosition))
            {
                return;
            }

            print("Set on fire. Position:" + tilePosition + ". Tile: " + tile + ". Scene: " + tilemap.gameObject.scene.name);
            Fire newFire = Instantiate(fire);
            newFire.transform.parent = tilemap.transform;
            newFire.transform.position = tilemap.GetCellCenterWorld(tilePosition);
            newFire.StartBurning(tilePosition, data, this, tilemap);
            activeFires.Add(tilePosition);
        }
    }
}
