using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class scr_BossFirefly_AngleLever : MonoBehaviour
{
    InputManager input;
    private Animator animlever;
    private AudioSource soundLeverActive;

    private bool onPlaceLever = false;

    [SerializeField] [Range(0, 180)] private int degreeRotation;
    [SerializeField] private float speedRotatingObjects;
    [SerializeField] private float timeReturnToMiddleState;
    public int turnNumber = 0;

    public GameObject objectConnected;

    IEnumerator returnToMiddleState(float time)
    {
        // После прошествия времени угловой рычаг вернётся в нейтральное положение
        yield return new WaitForSeconds(time);
        animlever.SetBool("Middle", true);
    }

    void Start()
    {

        input = InputManager.instance;
        input.playerInput.actions["Interaction"].performed += pressRightAction;

        soundLeverActive = GetComponent<AudioSource>();
        animlever = GetComponent<Animator>();
        animlever.SetBool("Middle", true);

        if (objectConnected.TryGetComponent(out scr_RotateModule scriptRotateModule))
        {
            scriptRotateModule.speedRotating = speedRotatingObjects;
        }
        else
        {
            print("На объекте " + objectConnected.name + " нет скрипта scr_RotateModule");
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            onPlaceLever = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            onPlaceLever = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            onPlaceLever = false;
        }
    }

    private void pressRightAction(InputAction.CallbackContext context)
    {
        if (onPlaceLever && animlever.GetBool("Middle"))
        {
            StartCoroutine(returnToMiddleState(timeReturnToMiddleState));

            animlever.SetBool("Middle", false);
            animlever.SetBool("Right", true);

            soundLeverActive.Play();

            turnNumber++;

            if (turnNumber >= 1 && turnNumber <= 2 || turnNumber >= 7 && turnNumber <= 8)
            {
                turnObjectsConnected(degreeRotation, -1);
            }
            else
            {
                turnObjectsConnected(degreeRotation, 1);
            }

            if (turnNumber == 8)
            {
                turnNumber = 0;
            }

        }
    }

    private void turnObjectsConnected(int degree, float rotationDirection)
    {
        if (objectConnected != null)
        {
            objectConnected.GetComponent<scr_RotateModule>().ChangeDegrees(degree, rotationDirection);
        }
        else
        {
            print(name + "Ни к чему не подключён");
        }

    }


    private void OnDestroy()
    {
        input.playerInput.actions["Interaction"].performed -= pressRightAction;
    }
}
