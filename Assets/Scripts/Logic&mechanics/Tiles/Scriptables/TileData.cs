using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    public float movementSpeed = 1f;
    public bool canJump = true;

    public bool canBurn = false;
    public float spreadChance;
    public float spreadInterval;
    public float burnTime;
}
