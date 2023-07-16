using UnityEngine;

public class scr_GameManager : MonoBehaviour
{
    public static scr_GameManager instance = null;

    public GameObject startPosition;
    public GameObject player;

    public string nameScene;

    scr_SaveController SaveController;
    scr_SaveObjectManager SaveObjectManager;
    scr_TilemapManager tilemapManager;

    public SaveGame currentSaveGame;
    public SettingsData currentSettingsData;

    private void Awake()
    {
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
    private void Start() 
    {
        SaveController = scr_SaveController.instance;
        SaveObjectManager = scr_SaveObjectManager.instance;
        tilemapManager = scr_TilemapManager.instance;
    }

    public void SetStartPosition()
    {
        startPosition=GameObject.Find("StartPosition");
        if(currentSaveGame.newGame)
        {
            player.transform.position = startPosition.transform.position;
            currentSaveGame.position = startPosition.transform.position;
            SaveController.SetSaveGame(currentSaveGame.numberOfSave,currentSaveGame);
        }
        else
        {
            player.transform.position = currentSaveGame.position;
        }

    }

    public void NewSceneLoaded(string nameScene)
    {
        //print("scene " + nameScene + " loaded");
        this.nameScene = nameScene;
        SaveObjectManager.loadDroppedObjects(nameScene);
        tilemapManager.AddTilemap(nameScene);
    }

    public void SceneUnloaded(string nameScene)
    {
        //print("scene " + nameScene + " unloaded");
        tilemapManager.RemoveTilemap(nameScene);
    }

}
