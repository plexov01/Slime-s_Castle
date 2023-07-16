using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class leverLogic : MonoBehaviour
{
    InputManager input;

    private Animator animlever;
    private AudioSource soundLeverActive;
    [SerializeField]public GameObject[] objectsConnected;

    
    private bool onPlaceLever=false;

    private scr_persistedObject scriptPersistedObject;
    private scr_ActivateCameras scriptActivateCameras;

    public int indexOfObject;

    private void Awake()
    {
        soundLeverActive =GetComponent<AudioSource>();
        animlever=GetComponent<Animator>();
    }

    //Для того, чтобы присвоить ID только тогда, когда оно определилось
    IEnumerator AddAfterWaitObjectID(int i, scr_ActivateCameras scriptActivateCameras)
    {
        yield return new WaitWhile(() => scriptActivateCameras.currentCamerasID =="");
        this.scriptActivateCameras.listCamerasID.Insert(i,scriptActivateCameras.currentCamerasID);
    }


    void Start()
    {
        input = InputManager.instance;
        input.playerInput.actions["Interaction"].performed += pressAction;
        
        
        if (gameObject.TryGetComponent(out scr_persistedObject scriptPersistedObject))
        {
            this.scriptPersistedObject = scriptPersistedObject;
        }
        else
        {
            Debug.Log("На leverLogic нет scr_persistedObject");
        }
        
        if (gameObject.TryGetComponent(out scr_ActivateCameras scriptActivateCamerasLocal))
        {
            scriptActivateCameras = scriptActivateCamerasLocal;
        }
        else
        {
            Debug.Log("На leverLogic нет scr_ActivateCameras");
        }

        
        for(int i=0;i<objectsConnected.Length;i++)
        {
            if(!objectsConnected[i].TryGetComponent(out scr_ActivateCameras scriptActivateCameras))
            {
                Debug.Log("На объекте "+objectsConnected[i].name+" нет скрипта scr_ActivateCameras");
            }
            
            if(objectsConnected[i].TryGetComponent(out DoorLogic scriptDoor))
            {
                if(!scriptDoor.M)
                {
                    scriptDoor.objectsConnected = new GameObject[]{gameObject};
                    scriptDoor.stateConnectedObjects = new bool[]{animlever.GetBool("Active")};
                    StartCoroutine(AddAfterWaitObjectID(i, scriptActivateCameras));
                }
                // else
                // {
                //     this.scriptActivateCameras.listCamerasID.Add(scriptActivateCameras.currentCamerasID);
                //     for (int j = 0; j < scriptDoor.objectsConnected.Length; j++)
                //     {
                //         this.scriptActivateCameras.listCamerasID.Add(scriptDoor.objectsConnected[j].GetComponent<scr_ActivateCameras>().currentCamerasID);
                //     }
                //
                //     // this.scriptActivateCameras.listCamerasID.Remove(this.scriptActivateCameras.currentCamerasID);
                //     // this.scriptActivateCameras.listCamerasID.Insert(i,scriptActivateCameras.currentCamerasID);
                // }
                
            }
            else
            {
                Debug.Log("На объекте "+objectsConnected[i].name+" нет скрипта DoorLogic");
            }
            
            
        }
        
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            onPlaceLever=true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player"))
        {
            onPlaceLever=false;
        }
    }

    private void pressAction(InputAction.CallbackContext context)
    {
        if (onPlaceLever)
        {
            animlever.SetBool("Active", !animlever.GetBool("Active"));
            
            soundLeverActive.Play();

            turnObjectsConnected();
            
            scriptPersistedObject.stateActive = animlever.GetBool("Active");
            
            scriptActivateCameras.BroadcastAllCameras();


        }
    }

    private void turnObjectsConnected()
    {
        if(objectsConnected!=null)
        {
            for(int i=0;i<objectsConnected.Length;i++)
            {
                objectsConnected[i].GetComponent<DoorLogic>().stateConnectedObjects[indexOfObject] = 
                    animlever.GetBool("Active");
                objectsConnected[i].GetComponent<DoorLogic>().CheckStateOnDoor();

            }
        }
        else
        {
            Debug.Log(name + "Ни к чему не подключён");
        }

    }

    public void turnStateOn(bool state){
        animlever.SetBool("Active", state);
        turnObjectsConnected();
    }
    


    private void OnDestroy()
    {
        input.playerInput.actions["Interaction"].performed -= pressAction;
    }
}
