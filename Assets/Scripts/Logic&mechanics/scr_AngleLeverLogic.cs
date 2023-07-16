using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class scr_AngleLeverLogic : MonoBehaviour
{
    InputManager input;
    private Animator animlever;
    private AudioSource soundLeverActive;

    private bool onPlaceLever=false;

    [SerializeField][Range(0, 180)] private int degreeRotation;
    [SerializeField]private float speedRotatingObjects;
    [SerializeField]private float timeReturnToMiddleState;

    public GameObject[] objectsConnected;


    IEnumerator returnToMiddleState(float time){
        // После прошествия времени угловой рычаг вернётся в нейтральное положение
        yield return new WaitForSeconds(time);
        animlever.SetBool("Middle", true);

        
    }

    private void Awake()
    {
        soundLeverActive = GetComponent<AudioSource>();
        animlever = GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {

        input = InputManager.instance;
        //input.playerInput.actions["TempSkillSlot"].performed += pressRightAction;
        input.playerInput.actions["Interaction"].performed += pressLeftAction;

        

        for(int i=0;i<objectsConnected.Length;i++){
            if(objectsConnected[i].TryGetComponent(out scr_RotateModule scriptRotateModule)){
                scriptRotateModule.speedRotating = speedRotatingObjects;
                
                // scriptRotateModule
                // if(!scriptRotateModule.M){
                //     scriptRotateModule.objectsConnected = new GameObject[]{gameObject};
                //     scriptRotateModule.stateConnectedObjects = new bool[]{animlever.GetBool("Active")};
                // }
            }else{
                print("На объекте "+objectsConnected[i].name+" нет скрипта scr_RotateModule");
            }   
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {

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

    private void pressRightAction(InputAction.CallbackContext context)
    {
        if (onPlaceLever&&animlever.GetBool("Middle"))
        {
            StartCoroutine(returnToMiddleState(timeReturnToMiddleState));

            animlever.SetBool("Middle", false);
            animlever.SetBool("Right", true);

            soundLeverActive.Play();

            turnObjectsConnected(degreeRotation,-1);
        }
    }

    private void pressLeftAction(InputAction.CallbackContext context)
    {
        if (onPlaceLever&&animlever.GetBool("Middle"))
        {
            StartCoroutine(returnToMiddleState(timeReturnToMiddleState));
            animlever.SetBool("Middle", false);
            animlever.SetBool("Right", false);
            soundLeverActive.Play();

            turnObjectsConnected(degreeRotation,1);
        }
    }

    private void turnObjectsConnected(int degree,float rotationDirection){
        if(objectsConnected!=null){
            for(int i=0;i<objectsConnected.Length;i++){
                objectsConnected[i].GetComponent<scr_RotateModule>().ChangeDegrees(degree,rotationDirection);
            }
        }else{
            print(name + "Ни к чему не подключён");
        }

    }


    private void OnDestroy()
    {
        //input.playerInput.actions["TempSkillSlot"].performed -= pressRightAction;
        input.playerInput.actions["Interaction"].performed -= pressLeftAction;
    }
}
