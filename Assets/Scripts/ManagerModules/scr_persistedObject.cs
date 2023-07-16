using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;


public class scr_persistedObject : MonoBehaviour
{
    private PersistedObject instance = new PersistedObject();
  
    private bool savedInManager;
    public string stringId;
    public string nameScene;
    public bool recoverable;
    
    [Header("true - if connected object")]
    [SerializeField] private bool connectedObject;
    [HideInInspector] public bool stateActive;
    // Объекты, которые не расположены изначально на сцене
    [Header("true - if dropped object")]
    [SerializeField] private bool droppedObject;
    public DroppedObjects typeDroppedObject;
    
    private scr_SaveObjectManager SaveObjectManager;
    private scr_GameManager GameManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        SaveObjectManager = scr_SaveObjectManager.instance;
        GameManager = scr_GameManager.instance;

        if (stringId == "")
        {
            if (droppedObject)
            {
                stringId = gameObject.name + SaveObjectManager.getCountDroppedObject();
            }
            else
            {
                stringId = gameObject.name;
            }
            
        }

        if (droppedObject)
        {
            for (int i = 0;i<SaveObjectManager.persistedDroppedObjects.Count;i++){
                if (SaveObjectManager.persistedDroppedObjects[i].stringId == stringId){
                    savedInManager = true; 
                    recoverable = SaveObjectManager.persistedDroppedObjects[i].recoverable; 
                    if (recoverable) 
                    {
                        transform.position = SaveObjectManager.persistedDroppedObjects[i].positionObject;
                        nameScene = GameManager.nameScene;
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                            
                }
                            
            }
                        
            if (!recoverable)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            for(int i = 0; i<SaveObjectManager.persistedObjects.Count;i++){
                if(SaveObjectManager.persistedObjects[i].stringId == stringId){
                    savedInManager = true;
                    recoverable = SaveObjectManager.persistedObjects[i].recoverable;
                    nameScene = GameManager.nameScene;;
                    stateActive = SaveObjectManager.persistedObjects[i].stateActive;

                    if (connectedObject)
                    {
                        gameObject.GetComponent<leverLogic>().turnStateOn(stateActive);
                    }
                    else
                    {
                        if(SaveObjectManager.persistedObjects[i].recoverable){
                            transform.position = SaveObjectManager.persistedObjects[i].positionObject;
                        }else{
                            Destroy(gameObject);
                        }
                    }
                }
            }
            
            

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnDestroy() {

        instance.stringId = stringId;
        instance.recoverable = recoverable;
        instance.positionObject = transform.position;
        instance.nameScene = nameScene;
        instance.stateActive = stateActive;
        instance.typeDroppedObject = typeDroppedObject;

        if (droppedObject)
        {
            if (savedInManager)
            {
                for (int i = 0;i<SaveObjectManager.persistedDroppedObjects.Count;i++) 
                {
                    if (SaveObjectManager.persistedDroppedObjects[i].stringId == stringId)
                    {
                        SaveObjectManager.persistedDroppedObjects[i] = instance;
                    }
                
                }
            }
            else
            {
                SaveObjectManager.persistedDroppedObjects.Add(instance);
            }
        }
        else
        {
            if(savedInManager)
            {
                for(int i = 0; i<SaveObjectManager.persistedObjects.Count;i++)
                {
                    if(SaveObjectManager.persistedObjects[i].stringId == stringId)
                    {
                        SaveObjectManager.persistedObjects[i] = instance;
                    }
                }
            }else
            {
                SaveObjectManager.persistedObjects.Add(instance);
            }
        }

        
        
    }
}

[System.Serializable]
public class PersistedObject
{

    public string stringId;
    public bool recoverable;
    public Vector3 positionObject;
    public string nameScene;
    public bool stateActive;
    public DroppedObjects typeDroppedObject;
    

}