using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum Cameras
{
    main,
    pipe,
    trolley
}

public class scr_CameraManager : MonoBehaviour
{
    public static scr_CameraManager instance = null;

    private bool mainCameraIsActive = true;

    public Animator animator;

    public CinemachineVirtualCamera mainVcam;
    public CinemachineVirtualCamera pipeCrawlingVcam;
    public CinemachineVirtualCamera TrolleyVcam;
    // public CinemachineVirtualCamera miniMapVcam;
    public Camera mainCamera;
    public GameObject mainVcamGameObject;
    
    private int IDCounter = 0;
    

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
        scr_EventSystem.instance.playerAwake.AddListener(SetFollowAtPlayer);
    }

    public void SwitchCameraState(Cameras camera)
    {
        if (camera==Cameras.main)
        {
            animator.Play("Main");
        }
        
        if (camera==Cameras.pipe)
        {
            animator.Play("PipeCrawling");
        }
        
        if (camera==Cameras.trolley)
        {
            animator.Play("insideTrolley");
        }
        
        // if (mainCameraIsActive)
        // {
        //     animator.Play("PipeCrawling");
        // }
        // else
        // {
        //     animator.Play("Main");
        // }
        // mainCameraIsActive = !mainCameraIsActive;
        
        
    }

    public void SetFollowAtPlayer(Transform playerTransform)
    {
        mainVcam.Follow = playerTransform;
        pipeCrawlingVcam.Follow = playerTransform;
        TrolleyVcam.Follow = playerTransform;

    }

    public int GetNewCameraID()
    {
        IDCounter++;
        return IDCounter;
    }

}
