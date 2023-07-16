using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEditor;
using Random = UnityEngine.Random;

public class Scr_Chest : MonoBehaviour
{
    public string chestColour;
    public int numberOfCoins = 5;

    public Sprite openedStateSprite;
    public GameObject coinPrefab;

    public float yForceMin;
    public float yForceMax;
    public float xForceMin;
    public float xForceMax;

    private bool opened = false;

    InputManager input;
    private scr_GameManager GameManager;
    
    private bool playerCanInteract = false;

    private AudioSource audioSource;

    private void Awake()
    {
        input = InputManager.instance;
        input.playerInput.actions["Interaction"].performed += Interact;
        
        GameManager = scr_GameManager.instance;
                
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        
        
        
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerCanInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerCanInteract = false;
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {

        if (playerCanInteract)
        {
            if (!opened)
            {
                Open();
                opened = true;
            }

        }

    }

    private void OnDestroy()
    {
        input.playerInput.actions["Interaction"].performed -= Interact;
    }

    private void Open()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = openedStateSprite;
        audioSource.Play();

        gameObject.GetComponent<scr_persistedObject>().recoverable = false;

        for (int i = 0; i < numberOfCoins; i++)
        {
            // спавн на конкретной сцене
            GameObject coin = PrefabUtility.InstantiatePrefab(coinPrefab, SceneManager.GetSceneByName(GameManager.nameScene)) as GameObject;
            
            coin.transform.position = transform.position;
            coin.transform.rotation = transform.rotation;
            coin.GetComponent<scr_persistedObject>().nameScene = GameManager.nameScene;
            
            coin.GetComponent<Rigidbody2D>().velocity = 
                new Vector3(Random.Range(xForceMin, xForceMax),Random.Range(yForceMin, yForceMax));
        }
        
    }

}
