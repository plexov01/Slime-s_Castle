using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_laserReceiver : MonoBehaviour
{
    private float laserReceivementTime = -1f;
    [SerializeField] private float timeRate = 0.1f;
    [SerializeField] private bool isOn;

    [SerializeField] private Material requiredLaserMaterial;

    [SerializeField] public GameObject[] objectsConnected;

    

    //what is that?
    public int indexOfObject;

    //SpriteRenderer spriteRenderer;
    Animator animator;
    [Header("Объекты, которые будут показаны камерой после активации")]
    public GameObject[] broadcastObject;
    
    private scr_ActivateCameras scriptActivateCameras;

    //Для того, чтобы присвоить ID только тогда, когда оно определилось
    IEnumerator AddAfterWaitObjectID(int i, scr_ActivateCameras scriptActivateCameras)
    {
        yield return new WaitWhile(() => scriptActivateCameras.currentCamerasID =="");
        this.scriptActivateCameras.listCamerasID.Insert(i,scriptActivateCameras.currentCamerasID);
    }
    
    private void Awake()
    {
        // spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();        
    }

    private void Start()
    {
        if (gameObject.TryGetComponent(out scr_ActivateCameras scriptActivateCamerasLocal))
        {
            scriptActivateCameras = scriptActivateCamerasLocal;
        }
        else
        {
            Debug.Log("На leverLogic нет scr_ActivateCameras");
        }
        
        for (int i = 0; i < broadcastObject.Length; i++)
        {
            if (broadcastObject[i].TryGetComponent(out scr_ActivateCameras scriptActivateCameras))
            {
                StartCoroutine(AddAfterWaitObjectID(i, scriptActivateCameras));
            }
            else
            {
                Debug.Log("На объекте "+broadcastObject[i].name+" нет скрипта scr_ActivateCameras");
            }
        }
        for (int i = 0; i < objectsConnected.Length; i++)
        {
            if (objectsConnected[i].TryGetComponent(out DoorLogic scriptDoor))
            {
                if (!scriptDoor.M)
                {
                    scriptDoor.objectsConnected = new GameObject[] { gameObject };
                    scriptDoor.stateConnectedObjects = new bool[] { isOn };
                }
            }
            else
            {
                print("На объекте " + objectsConnected[i].name + " нет скрипта DoorLogic");
            }
        }

    }

    private void Update()
    {
        //timeLeftSinceLastLaserReceivement -= Time.deltaTime;
        if (laserReceivementTime + timeRate <= Time.time)
        {
            //off
            if (isOn)
            {
                ChangeState();
            }
        }
        else
        {
            //on
            if (!isOn)
            {
                ChangeState();
            }
        }
    }

    public void Enable(Material laserMaterial)
    {
        if (laserMaterial == requiredLaserMaterial)
        {
            laserReceivementTime = Time.time;
        }
    }

    public void ChangeState()
    {
        isOn = !isOn;
        turnObjectsConnected();
        animator.SetBool("isOn",isOn);

        if (isOn)
        {
            scriptActivateCameras.BroadcastAllCameras();
        }

    }

    private void turnObjectsConnected()
    {
        if (objectsConnected != null)
        {
            for (int i = 0; i < objectsConnected.Length; i++)
            {
                objectsConnected[i].GetComponent<DoorLogic>().stateConnectedObjects[indexOfObject] = isOn;
                objectsConnected[i].GetComponent<DoorLogic>().CheckStateOnDoor();
        
            }
        }
        else
        {
            print(name + "Ни к чему не подключён");
        }

    }
}
