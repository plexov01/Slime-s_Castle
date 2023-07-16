using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class TilemapSaveHandler : MonoBehaviour
{
    private Tilemap tilemap;

    private Dictionary<TileBase, BaseBuildingObject> tileBaseToBuildingObject = new Dictionary<TileBase, BaseBuildingObject>();
    private Dictionary<string, TileBase> guidToTileBase = new Dictionary<string, TileBase>();

    private Dictionary<AnimatedTile, BaseBuildingObject> animatedTileToBuildingObject = new Dictionary<AnimatedTile, BaseBuildingObject>();
    private Dictionary<string, AnimatedTile> guidToAnimatedTile = new Dictionary<string, AnimatedTile>();

    private Dictionary<RuleTile, BaseBuildingObject> ruleTileToBuildingObject = new Dictionary<RuleTile, BaseBuildingObject>();
    private Dictionary<string, RuleTile> guidToRuleTile = new Dictionary<string, RuleTile>();

    private string filename;
    private string tilemapKey;

    [SerializeField] private string filenameForEditor = "saveGame0_TilemapData.json";

    private bool mapIsloaded = false;

    private void Start()
    {
        InitTilemapAndTileReferences();
        StartCoroutine(LoadMap());
    }

    private void InitTilemapAndTileReferences()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapKey = tilemap.name + "_" + tilemap.gameObject.scene.name;

        if (tilemap == null)
        {
            print("Tilemap not found on this object");
            return;
        }

        BaseBuildingObject[] buildables = Resources.LoadAll<BaseBuildingObject>("Scriptables/Buildables");

        foreach (BaseBuildingObject buildable in buildables)
        {
            if (buildable.TileBase != null)
            {
                if (!tileBaseToBuildingObject.ContainsKey(buildable.TileBase))
                {
                    //print("Tilebase: " + buildable.TileBase + " name: " + buildable.TileBase.name);
                    tileBaseToBuildingObject.Add(buildable.TileBase, buildable);
                    guidToTileBase.Add(buildable.Id, buildable.TileBase);
                }
                else
                {
                    Debug.LogError("TileBase " + buildable.TileBase.name + " is already in use by " + tileBaseToBuildingObject[buildable.TileBase].Id);
                    continue;
                }
            }
            else if (buildable.AnimatedTile != null)
            {
                if (!animatedTileToBuildingObject.ContainsKey(buildable.AnimatedTile))
                {
                    //print("AnimatedTile: " + buildable.AnimatedTile + " name: " + buildable.AnimatedTile.name);
                    animatedTileToBuildingObject.Add(buildable.AnimatedTile, buildable);
                    guidToAnimatedTile.Add(buildable.Id, buildable.AnimatedTile);
                }
                else
                {
                    Debug.LogError("AnimatedTile " + buildable.AnimatedTile.name + " is already in use by " + animatedTileToBuildingObject[buildable.AnimatedTile].Id);
                    continue;
                }
            }
            else if (buildable.RuleTile != null)
            {
                if (!ruleTileToBuildingObject.ContainsKey(buildable.RuleTile))
                {
                    //print("RuleTile: " + buildable.RuleTile + " name: " + buildable.RuleTile.name);
                    ruleTileToBuildingObject.Add(buildable.RuleTile, buildable);
                    guidToRuleTile.Add(buildable.Id, buildable.RuleTile);
                }
                else
                {
                    Debug.LogError("RuleTile " + buildable.RuleTile.name + " is already in use by " + ruleTileToBuildingObject[buildable.RuleTile].Id);
                    continue;
                }
            }
            else
            {
                Debug.LogError("All fields for " + buildable.name + " are null");
            }
        }
    }

    private void SaveMap(string filename)
    {
        if (!mapIsloaded)
        {
            return;
        }

        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(filename);

        TilemapData mapData = new TilemapData();
        mapData.key = tilemapKey;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile != null && tileBaseToBuildingObject.ContainsKey(tile))
            {
                string guid = tileBaseToBuildingObject[tile].Id;
                TileInfo tileInfo = new TileInfo(pos, guid);
                mapData.tiles.Add(tileInfo);
            }
            else
            {
                AnimatedTile animatedTile = tilemap.GetTile<AnimatedTile>(pos);

                if (animatedTile != null && animatedTileToBuildingObject.ContainsKey(animatedTile))
                {
                    string guid = animatedTileToBuildingObject[animatedTile].Id;
                    TileInfo tileInfo = new TileInfo(pos, guid);
                    mapData.tiles.Add(tileInfo);
                }
                else
                {
                    RuleTile ruleTile = tilemap.GetTile<RuleTile>(pos);

                    if (ruleTile != null && ruleTileToBuildingObject.ContainsKey(ruleTile))
                    {
                        string guid = ruleTileToBuildingObject[ruleTile].Id;
                        TileInfo tileInfo = new TileInfo(pos, guid);
                        mapData.tiles.Add(tileInfo);
                    }
                }
            }
        }

        bool tilemapSaveExists = false;
        foreach (var tilemapData in data)
        {
            if (tilemapData.key == tilemapKey)
            {
                tilemapData.tiles = mapData.tiles;
                tilemapSaveExists = true;
                break;
            }
        }

        if (!tilemapSaveExists)
        {
            data.Add(mapData);
        }

        FileHandler.SaveToJSON<TilemapData>(data, filename);
        print("tilemap " + tilemap.name + " from scene " + tilemap.gameObject.scene.name + " saved");
    }

    public void SaveMapFromEditor()
    {
        if (tilemap == null)
        {
            InitTilemapAndTileReferences();
        }

        filename = filenameForEditor;

        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(filename);

        TilemapData mapData = new TilemapData();
        mapData.key = tilemapKey;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile != null && tileBaseToBuildingObject.ContainsKey(tile))
            {
                string guid = tileBaseToBuildingObject[tile].Id;
                TileInfo tileInfo = new TileInfo(pos, guid);
                mapData.tiles.Add(tileInfo);
            }
            else
            {
                AnimatedTile animatedTile = tilemap.GetTile<AnimatedTile>(pos);

                if (animatedTile != null && animatedTileToBuildingObject.ContainsKey(animatedTile))
                {
                    string guid = animatedTileToBuildingObject[animatedTile].Id;
                    TileInfo tileInfo = new TileInfo(pos, guid);
                    mapData.tiles.Add(tileInfo);
                }
                else
                {
                    RuleTile ruleTile = tilemap.GetTile<RuleTile>(pos);

                    if (ruleTile != null && ruleTileToBuildingObject.ContainsKey(ruleTile))
                    {
                        string guid = ruleTileToBuildingObject[ruleTile].Id;
                        TileInfo tileInfo = new TileInfo(pos, guid);
                        mapData.tiles.Add(tileInfo);
                    }
                }
            }
        }

        bool tilemapSaveExists = false;
        foreach (var tilemapData in data)
        {
            if (tilemapData.key == tilemapKey)
            {
                tilemapData.tiles = mapData.tiles;
                tilemapSaveExists = true;
                break;
            }
        }

        if (!tilemapSaveExists)
        {
            data.Add(mapData);
        }

        FileHandler.SaveToJSON<TilemapData>(data, filename);
        print("tilemap " + tilemap.name + " from scene " + tilemap.gameObject.scene.name + " saved");
    }

    private IEnumerator LoadMap()
    {
        yield return new WaitUntil(() => scr_GameManager.instance.currentSaveGame.nameOfSave != "");
        filename = scr_GameManager.instance.currentSaveGame.nameOfSave + "_TilemapData.json";
        print("filename: " + filename);
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(filename);

        if (data.Count == 0)
        {
            print("First time loading tilemap " + tilemap.name + " on scene " + gameObject.scene.name + " on save file " + scr_GameManager.instance.currentSaveGame.nameOfSave + ". No saved tilemap to load.");
            mapIsloaded = true;
            yield break;
        }
        
        foreach (var mapData in data)
        {
            if (mapData.key == tilemapKey)
            {
                tilemap.ClearAllTiles();

                if (mapData.tiles != null && mapData.tiles.Count > 0)
                {
                    foreach (TileInfo tile in mapData.tiles)
                    {
                        if (guidToTileBase.ContainsKey(tile.guidForBuildable))
                        {
                            tilemap.SetTile(tile.position, guidToTileBase[tile.guidForBuildable]);
                        }
                        else if (guidToAnimatedTile.ContainsKey(tile.guidForBuildable))
                        {
                            tilemap.SetTile(tile.position, guidToAnimatedTile[tile.guidForBuildable]);
                        }
                        else if (guidToRuleTile.ContainsKey(tile.guidForBuildable))
                        {
                            tilemap.SetTile(tile.position, guidToRuleTile[tile.guidForBuildable]);
                        }
                        else
                        {
                            Debug.LogError("Refernce " + tile.guidForBuildable + " could not be found.");
                        }
                    }
                }

                mapIsloaded = true;
            }
            else
            {
                continue;
            }
        }

        if (mapIsloaded)
        {
            print("Tilemap " + tilemap.name + " on scene " + tilemap.gameObject.scene.name + " loaded");
        }
        else
        {
            print("First time loading tilemap " + tilemap.name + " on scene " + gameObject.scene.name + " on save file " + scr_GameManager.instance.currentSaveGame.nameOfSave + ". No saved tilemap to load.");
            mapIsloaded = true;
        }
    }

    public void LoadMapFromEditor()
    {
        if (tilemap == null)
        {
            InitTilemapAndTileReferences();
        }

        filename = filenameForEditor;
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(filename);
        mapIsloaded = false;

        if (data.Count == 0)
        {
            print("First time loading tilemap " + tilemap.name + " on scene " + gameObject.scene.name + " on save file " + filenameForEditor.Substring(0, 9) + ". No saved tilemap to load.");
            return;
        }

        foreach (var mapData in data)
        {
            if (mapData.key == tilemapKey)
            {
                tilemap.ClearAllTiles();

                if (mapData.tiles != null && mapData.tiles.Count > 0)
                {
                    foreach (TileInfo tile in mapData.tiles)
                    {
                        if (guidToTileBase.ContainsKey(tile.guidForBuildable))
                        {
                            tilemap.SetTile(tile.position, guidToTileBase[tile.guidForBuildable]);
                        }
                        else if (guidToAnimatedTile.ContainsKey(tile.guidForBuildable))
                        {
                            tilemap.SetTile(tile.position, guidToAnimatedTile[tile.guidForBuildable]);
                        }
                        else if (guidToRuleTile.ContainsKey(tile.guidForBuildable))
                        {
                            tilemap.SetTile(tile.position, guidToRuleTile[tile.guidForBuildable]);
                        }
                        else
                        {
                            Debug.LogError("Refernce " + tile.guidForBuildable + " could not be found.");
                        }
                    }
                }

                mapIsloaded = true;
            }
            else
            {
                continue;
            }
        }

        if (mapIsloaded)
        {
            print("Tilemap " + tilemap.name + " on scene " + tilemap.gameObject.scene.name + " loaded");
        }
        else
        {
            print("Wrong save file or tilemap wasn't saved before");
        }
    }

    private void OnDisable()
    {
        SaveMap(filename);
    }

    private void OnDestroy()
    {
        //OnSave();
        //tilemapManager.RemoveTilemap(tilemap);
    }


    [Serializable]
    public class TilemapData
    {
        public string key;
        public List<TileInfo> tiles = new List<TileInfo>();
    }

    [Serializable]
    public class TileInfo
    {
        public string guidForBuildable;
        public Vector3Int position;

        public TileInfo(Vector3Int pos, string guid)
        {
            position = pos;
            guidForBuildable = guid;
        }
    }
}
