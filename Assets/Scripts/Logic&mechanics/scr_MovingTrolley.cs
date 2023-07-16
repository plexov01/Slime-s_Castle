using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class scr_MovingTrolley : MonoBehaviour
{
    
    
    
    [SerializeField][Range(-10,10)]private float speedTrolleyMovingHorizontalVector;
    [SerializeField][Range(0,20)]private float velocityTrolleyFalling;
    [SerializeField]private float distanceFromFloor;
    
    [SerializeField]private Vector2 speedVectorTrolley;
    [SerializeField]private Vector2 speedCalcVector;
    
    

    [Header("States:")]

    [SerializeField]private bool movingOnRails;
    [SerializeField]private bool falling;
    [SerializeField]private bool playerInside;

    [SerializeField] private LayerMask railLayerMask;
    

    InputManager input;
    private GameObject player;
    private Rigidbody2D rb;
    [SerializeField]private bool playerOnPlace;
    


    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.instance;
        input.playerInput.actions["Interaction"].performed += PressAction;

        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if(!playerInside)return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up,distanceFromFloor*2f,railLayerMask);
        //Отрисовка нормалей от точки
        Debug.DrawRay(hit.point, hit.normal * hit.distance, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        
        speedCalcVector.x = speedTrolleyMovingHorizontalVector;
        // Расчёт кратчайшего расстояния до рельс
        float shortDistanceToRails = hit.distance * Vector3.Dot(Vector3.up, hit.normal);
        
        if (hit.collider!=null)
        {
            // Если перед этим падал - обнуляем скорость
            if (falling)
            {
                rb.velocity=new Vector2(rb.velocity.x,0);
            }
            falling = false;
            // Проверка расстояния до земли
            
            if (shortDistanceToRails > distanceFromFloor)
            {
                movingOnRails = true;
            }
        }
        else
        {
            falling = true;
            movingOnRails = false;
        }


        if (movingOnRails)
        {
            speedVectorTrolley = speedCalcVector - Vector3.Dot(speedCalcVector,hit.normal)*hit.normal;
            speedVectorTrolley *= Mathf.Abs(speedTrolleyMovingHorizontalVector) / speedVectorTrolley.magnitude;
            
            if (shortDistanceToRails*1.01>distanceFromFloor)
            {
                speedVectorTrolley.y = Mathf.Lerp(speedVectorTrolley.y,-velocityTrolleyFalling,0.02f);
            }

            if (shortDistanceToRails*0.99<distanceFromFloor)
            {
                speedVectorTrolley.y = Mathf.Lerp(speedVectorTrolley.y,velocityTrolleyFalling,0.02f);
            }
                                 
        }

        if (falling)
        {

            if (speedVectorTrolley.y>-velocityTrolleyFalling)
            {
                speedVectorTrolley.y -= velocityTrolleyFalling * Time.deltaTime;
            }
            
        }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, hit.normal), 0.1f*Mathf.Abs(speedTrolleyMovingHorizontalVector));
        
        rb.velocity = speedVectorTrolley;

    }

    private void PressAction(InputAction.CallbackContext context)
    {
        if(playerInside){
            playerInside = false;
            scr_CameraManager.instance.SwitchCameraState(Cameras.main);
            player.GetComponent<Rigidbody2D>().isKinematic = false;
            player.transform.parent = null;

            player.transform.rotation = Quaternion.Euler(0,0,0);
            

            rb.velocity = Vector2.zero;
            return;
        }

        if (playerOnPlace)
        {
            playerInside = true;
            //Переключение на необходимую камеру(настройки камеры устраняют визуальное дёрганье объекта при быстром движении)
            scr_CameraManager.instance.SwitchCameraState(Cameras.trolley);
            player.transform.parent = gameObject.transform;
            player.transform.position = transform.position;
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);

            player.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles);
            
            

            
            // animlever.SetBool("Active", !animlever.GetBool("Active"));
            // soundLeverActive.Play();

            TurnTrolley();
            return;
        }
        
        
        
    }

    private void TurnTrolley(){
        Debug.Log("Вагонетка запущена");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            playerOnPlace=true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            playerOnPlace=false;
            player = null;
        }
    }


    private void OnDestroy()
    {
        input.playerInput.actions["Interaction"].performed -= PressAction;
    }
}
