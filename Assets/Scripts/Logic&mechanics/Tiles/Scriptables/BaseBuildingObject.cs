using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Buildable", menuName = "LevelBuilding/Create Buildable")]
public class BaseBuildingObject : BaseScriptableObject
{
    [SerializeField] private TileBase tileBase;
    [SerializeField] private AnimatedTile animatedTile;
    [SerializeField] private RuleTile ruleTile;

    public TileBase TileBase
    {
        get
        {
            return tileBase;
        }
    }

    public AnimatedTile AnimatedTile
    {
        get
        {
            return animatedTile;
        }
    }

    public RuleTile RuleTile
    {
        get
        {
            return ruleTile;
        }
    }
}