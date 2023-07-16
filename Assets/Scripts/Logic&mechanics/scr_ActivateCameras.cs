using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class scr_ActivateCameras : MonoBehaviour
{
    [SerializeField] private bool canBroadcast = true;
    [SerializeField] private bool showOneTime;
    
    [SerializeField][Range(0,5f)] private float timeSwitchinfCamera;
    [SerializeField] private int indexBroadcastCamera;
    
    public string currentCamerasID;
    public List<string> listCamerasID=new List<string>(10);
    
    [SerializeField] private GameObject currentGameObjectLocalCamera;
    [SerializeField] private bool broadcasting;
    
    
    IEnumerator StartBroadcastCamera(int index)
    {
        broadcasting = true;
        TurnOnCamera(listCamerasID[index]);
        yield return null;
        yield return new WaitForSeconds(timeSwitchinfCamera);
        TurnOffCamera(listCamerasID[indexBroadcastCamera]);
        indexBroadcastCamera++;
        if (listCamerasID.Count>indexBroadcastCamera)
        {
            StartCoroutine(StartBroadcastCamera(indexBroadcastCamera));
        }
        else
        {
            broadcasting = false;
            indexBroadcastCamera = 0;
            if (showOneTime)
            {
                canBroadcast = false;
            }
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        if(currentGameObjectLocalCamera.TryGetComponent(out scr_LocalCamera scriptLocalCamera))
        {
            currentCamerasID = scriptLocalCamera.requiredTriggerID;
        }
        else
        {
            Debug.Log("На объекте "+currentGameObjectLocalCamera.name+" нет скрипта scr_LocalCamera");
        }  

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BroadcastAllCameras()
    {
        if (broadcasting||!canBroadcast)
        {
            return;
        }

        if (listCamerasID.Count>0)StartCoroutine(StartBroadcastCamera(indexBroadcastCamera));

    }

    private void TurnOnCamera(string objectName)
    {
        scr_EventSystem.instance.playerEnteredObjectTrigger.Invoke(objectName);
    }
    
    private void TurnOffCamera(string objectName)
    {
        scr_EventSystem.instance.playerLeftObjectTrigger.Invoke(objectName);
    }
    
    

}
