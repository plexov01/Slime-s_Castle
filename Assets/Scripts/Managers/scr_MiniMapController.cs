using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class scr_MiniMapController : MonoBehaviour
{
    public static scr_MiniMapController instance = null;

    public GameObject player;
    [SerializeField] private GameObject Camera;
    [SerializeField] private GameObject pnl_MiniMap;
    [SerializeField] private Tilemap allMiniMap;
    [SerializeField] private Tilemap openMiniMap;

    public bool showMiniMap;
    public bool playerExistOnScene;

    [SerializeField] private float visibilityRadius;

    [SerializeField] private Vector3Int playerPositionInt;
    [SerializeField] private Vector3Int playerPositionIntInCell;
    [SerializeField] private Vector2Int widthHeightViewport;

    [SerializeField] private Tile whiteTile;

    scr_GameManager GameManager;

    // private Vector3 mas;
    

    private void Awake() {
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
    // Start is called before the first frame update
    void Start()
    {   
        GameManager = scr_GameManager.instance;
        if(GameManager.player!=null){
            player = GameManager.player;
            playerExistOnScene = true;
        }else{
            playerExistOnScene = false;
        }
        pnl_MiniMap.SetActive(false);

        
        // Debug.Log(Camera.GetComponent<Camera>().size);
        // Vector3Int vecInt=new Vector3Int((int)player.transform.position.x,(int)player.transform.position.y,(int)player.transform.position.z);
        // BoundsInt area= new BoundsInt(allMiniMap.WorldToCell(vecInt),new Vector3Int(50,50,1));
        
        // Tilemap tilemap = allMiniMap;
        // TileBase[] tileArray = tilemap.GetTilesBlock(area);

        // // for (int index = 0; index < tileArray.Length; index++)
        // // {
        // //     if(tileArray[index]!=null){
        // //         Debug.Log(tileArray[index].x);
        // //         // mas.add(allMiniMap.CellToWorld(tileArray[index].transform));
        // //     }
            
        // // }

        // for (int x = 0; x < area.size.x; x++)
        // {
            
        //     for(int y = 0; y<area.size.y; y++){
        //         Vector3Int area2 = allMiniMap.WorldToCell(vecInt);
        //         area2.x+=x;
        //         area2.y+=y;
        //         if(allMiniMap.GetTile(area2)!=null){
        //             allMiniMap.SetTile(area2,null);

        //         }
        //     }
        //     // if
        //     // if(tileArray[index]!=null){
        //     //     Debug.Log(tileArray[index].x);
        //     //     // mas.add(allMiniMap.CellToWorld(tileArray[index].transform));
        //     // }
            
        // } 

        // Debug.Log(allMiniMap.cellBounds);
        // Debug.Log(allMiniMap.localBounds);

    }

    // Update is called once per frame
    void Update()
    {
        pnl_MiniMap.SetActive(showMiniMap);
        if(!showMiniMap) return;
        if (!playerExistOnScene)
        {
            showMiniMap = false;
            return;
        }

        

        
        playerPositionInt = Vector3Int.FloorToInt(player.transform.position);
        playerPositionInt.x -= widthHeightViewport.x/8;
        playerPositionInt.y -= widthHeightViewport.y/8;

        playerPositionIntInCell = allMiniMap.WorldToCell(playerPositionInt);




        for (int x = playerPositionIntInCell.x; x < playerPositionIntInCell.x+widthHeightViewport.x; x++)
        {
            
            for(int y = playerPositionIntInCell.y; y<playerPositionIntInCell.y+widthHeightViewport.y; y++){

                Vector3Int tilePosition = new Vector3Int(x,y,0);
                if(allMiniMap.HasTile(tilePosition)&&!openMiniMap.HasTile(tilePosition)){


                    // if(Vector3.Distance(tilePosition,playerPositionIntInCell)<visibilityRadius){
                        // Нужно использовать позицию игрока playerPositionInt без сдвига относительно widthHeightViewport
                    // }

                    openMiniMap.SetTile(tilePosition,whiteTile);

                }
            }
        }
            
        // Debug.Log(allMiniMap.WorldToCell(player.transform.position));
        
        // if(showMiniMap){}
        Camera.transform.position = new Vector3(player.transform.position.x,player.transform.position.y,Camera.transform.position.z);
            
        
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        if(playerExistOnScene){
            Gizmos.DrawWireCube(player.transform.position, new Vector3(widthHeightViewport.x/4, widthHeightViewport.y/4, 0));
            Gizmos.DrawWireSphere(player.transform.position, visibilityRadius/4);
        }
        
        
    }




}
