using System.Collections;
using UnityEngine;

public class DoorLogic : MonoBehaviour
{

    private Animator animDoor;
    private AudioSource soundActive;
    private Collider2D colliderDoor;

    [Header("1:M")]
    public bool M=false;

    [SerializeField]private bool stateIsInverted;

    [SerializeField]public GameObject[] objectsConnected;

    [SerializeField]private bool[] stateForActiveDoor;
    public bool[] stateConnectedObjects;
    
    private scr_ActivateCameras scriptActivateCameras;
    
    //Для того, чтобы присвоить ID только тогда, когда оно определилось
    IEnumerator AddAfterWaitObjectID(scr_ActivateCameras scriptActivateCameras)
    {
        yield return new WaitWhile(() => scriptActivateCameras.currentCamerasID =="");
        // this.scriptActivateCameras.listCamerasID.Insert(i,scriptActivateCameras.currentCamerasID);
        scriptActivateCameras.listCamerasID.Add(this.scriptActivateCameras.currentCamerasID);
        for (int j = 0; j < objectsConnected.Length; j++)
        {
            string objectId = objectsConnected[j].GetComponent<scr_ActivateCameras>().currentCamerasID;
            if (scriptActivateCameras.currentCamerasID!=objectId)
            {
                scriptActivateCameras.listCamerasID.Add(objectId);
            }
        }
    }
    
    void Start()
    {
        animDoor=GetComponent<Animator>();
        soundActive=GetComponent<AudioSource>();
        colliderDoor=GetComponent<Collider2D>();
        
        if (gameObject.TryGetComponent(out scr_ActivateCameras scriptActivateCamerasLocal))
        {
            scriptActivateCameras = scriptActivateCamerasLocal;
        }
        else
        {
            Debug.Log("На leverLogic нет scr_ActivateCameras");
        }

        if(M)
        {
            if(stateForActiveDoor.Length<objectsConnected.Length)
            {
                stateForActiveDoor=new bool[objectsConnected.Length];
                for(int i=0;i<objectsConnected.Length;i++)stateForActiveDoor[i]=true;
            }
            stateConnectedObjects=new bool[objectsConnected.Length];
            for(int i=0;i<objectsConnected.Length;i++)
            {
                if(objectsConnected[i].TryGetComponent(out scr_ActivateCameras scriptActivateCameras))
                {
                    StartCoroutine(AddAfterWaitObjectID(scriptActivateCameras));
                }
                else
                {
                    Debug.Log("На объекте "+objectsConnected[i].name+" нет скрипта scr_ActivateCameras");
                }
                
                if(objectsConnected[i].TryGetComponent(out leverLogic scriptObjectLever))
                {
                    if(scriptObjectLever.indexOfObject==0)
                    {
                        scriptObjectLever.indexOfObject=i;
                    }
                    else
                    {
                        print("Связанный объект "+objectsConnected[i].name+" уже с кем-то связан");
                    }
                        

                    scriptObjectLever.objectsConnected = new GameObject[]{gameObject};


                    stateConnectedObjects[i] = 
                        objectsConnected[i].GetComponent<Animator>().GetBool("Active");
                }

                if(objectsConnected[i].TryGetComponent(out pockPlateLogic scriptObjectPockPlate))
                {
                    if(scriptObjectPockPlate.indexOfObject==0)
                    {
                        scriptObjectPockPlate.indexOfObject=i;
                    }
                    else
                    {
                        print("Связанный объект "+objectsConnected[i].name+" уже с кем-то связан");
                    }
                        
                    if(M)
                    {
                        scriptObjectPockPlate.objectsConnected = new GameObject[]{gameObject};
                        if (scriptObjectPockPlate.timesActive.Length == 0) 
                        {
                            scriptObjectPockPlate.timesActive = new float[1];
                        }
                    }
                    
                    stateConnectedObjects[i] 
                        = objectsConnected[i].GetComponent<Animator>().GetBool("Active");
                }
                    
            }
        }
        else
        {
            if(objectsConnected.Length == 0)objectsConnected = new  GameObject[1];
            if(stateForActiveDoor.Length == 0)stateForActiveDoor = new bool[]{true};
            stateConnectedObjects = new bool[objectsConnected.Length];
        }

        CheckStateOnDoor();
    }

    public void CheckStateOnDoor()
    {
        bool coincude=true;
        
        for(int i=0;i<objectsConnected.Length;i++)
        {
            if(stateForActiveDoor[i]!=stateConnectedObjects[i])
            {
                coincude=false;
                break;
            }
        }
        
        if(coincude!=animDoor.GetBool("Active"))
        {
            soundActive.Play();
        }

        if (!stateIsInverted)
        {
            if (coincude)
            {
                animDoor.SetBool("Active", true);
                colliderDoor.enabled = false;

            }
            else
            {
                animDoor.SetBool("Active", false);
                colliderDoor.enabled = true;
            }
        }
        else
        {
            if (coincude)
            {
                animDoor.SetBool("Active", false);
                colliderDoor.enabled = true;
            }
            else
            {
                animDoor.SetBool("Active", true);
                colliderDoor.enabled = false;
            }
        }

    }
}
