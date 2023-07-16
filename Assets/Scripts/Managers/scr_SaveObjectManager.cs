using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

using System.IO;

public enum DroppedObjects
{
    coin,
    garbage
    
}
public class scr_SaveObjectManager : MonoBehaviour
{
    public static scr_SaveObjectManager instance = null;

    public List<PersistedObject> persistedObjects;

    public List<PersistedObject> persistedDroppedObjects;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject coinObject;
    [SerializeField] private GameObject mushroomObject;
    [SerializeField] private GameObject garbageObject;
    
    private GameObject coinObjectClone;
    private GameObject mushroomObjectClone;
    private GameObject garbageObjectClone;

    [SerializeField] private int countPersistedDroppedObjects = 0;
    
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
        // Для теста
        // GameObject test = PrefabUtility.InstantiatePrefab(garbageObject, SceneManager.GetSceneByName("scn_test1")) as GameObject;
        // test.GetComponent<scr_persistedObject>().nameScene = "scn_test1";
        //
        // test = PrefabUtility.InstantiatePrefab(garbageObject, SceneManager.GetSceneByName("scn_test1")) as GameObject;
        // test.GetComponent<scr_persistedObject>().nameScene = "scn_test1";



    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void loadDroppedObjects(string nameScene)
    {
        for (int i = 0; i < persistedDroppedObjects.Count; i++)
        {
            if (persistedDroppedObjects[i].nameScene == nameScene)
            {
                if (persistedDroppedObjects[i].recoverable)
                {
                    switch (persistedDroppedObjects[i].typeDroppedObject)
                    {
                        case DroppedObjects.coin:
                            coinObjectClone = PrefabUtility.InstantiatePrefab(coinObject, SceneManager.GetSceneByName(nameScene)) as GameObject;
                            coinObjectClone.GetComponent<scr_persistedObject>().stringId = persistedDroppedObjects[i].stringId;
                            break;
                        case DroppedObjects.garbage:
                            garbageObjectClone = PrefabUtility.InstantiatePrefab(garbageObject, SceneManager.GetSceneByName(nameScene)) as GameObject;
                            garbageObjectClone.GetComponent<scr_persistedObject>().stringId = persistedDroppedObjects[i].stringId;
                            break;
                        
                    }
                    
                }
                
                
            }

        }
        // saveAllPersistedObjectsInFile();

    }

    public int getCountDroppedObject()
    {
        countPersistedDroppedObjects++;
        return countPersistedDroppedObjects;
        
    }
    
    public bool ExistsAllPersistedObjectsFile()
    {
        string path = Application.streamingAssetsPath + "/PersistedObjects/"+"AllPersistedObjects"+".json";
        return File.Exists(path);
    }


    public AllPersistedObjects GetAllPersistedObjectsFile()
    {
        if(ExistsAllPersistedObjectsFile())
        {
            string path = Application.streamingAssetsPath + "/PersistedObjects/"+"AllPersistedObjects"+".json";
            AllPersistedObjects allPersistedObjects = JsonUtility.FromJson<AllPersistedObjects>(File.ReadAllText(path));
            return allPersistedObjects;
        }
        else
        {
            Debug.Log("AllPersistedObjects не существует");
            return new AllPersistedObjects();
        }
 
    }

    public void getDataFromAllPersistedObjects()
    {
        AllPersistedObjects allPersistedObjects = GetAllPersistedObjectsFile();
        persistedObjects = allPersistedObjects.persistedObjects;
        persistedDroppedObjects = allPersistedObjects.persistedDroppedObjects;
    }

    public void saveAllPersistedObjectsInFile()
    {
        AllPersistedObjects allPersistedObjects = new AllPersistedObjects();
        allPersistedObjects.persistedObjects = persistedObjects;
        allPersistedObjects.persistedDroppedObjects = persistedDroppedObjects;
        
        string data = JsonUtility.ToJson(allPersistedObjects);
        string path = Application.streamingAssetsPath + "/PersistedObjects/"+"AllPersistedObjects"+".json";
        System.IO.File.WriteAllText(path, data);
        Debug.Log( "AllPersistedObjects создан");  
    }

}

[System.Serializable]
public class AllPersistedObjects
{
    public List<PersistedObject> persistedObjects;
    public List<PersistedObject> persistedDroppedObjects;
}
