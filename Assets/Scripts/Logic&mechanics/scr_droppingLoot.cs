using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Random = UnityEngine.Random;

public class scr_droppingLoot : MonoBehaviour
{
    [SerializeField] private GameObject lootObject;
    [SerializeField] private int countOfLoot;
    
    [SerializeField] private float yForceMin;
    [SerializeField] private float yForceMax;
    [SerializeField] private float xForceMin;
    [SerializeField] private float xForceMax;
    
    private scr_GameManager GameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager = scr_GameManager.instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        for (int i = 0; i < countOfLoot; i++)
        { 
            GameObject loot = PrefabUtility.InstantiatePrefab(lootObject, SceneManager.GetSceneByName(GameManager.nameScene)) as GameObject;
            
            loot.transform.position = transform.position;
            loot.GetComponent<scr_persistedObject>().nameScene = GameManager.nameScene;
            
            loot.GetComponent<Rigidbody2D>().velocity = 
                new Vector3(Random.Range(xForceMin, xForceMax),Random.Range(yForceMin, yForceMax));
            
        }
    }
}
