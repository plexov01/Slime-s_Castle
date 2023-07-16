using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_RotateModule : MonoBehaviour
{
    // [SerializeField] private Vector3 rotation;
    [Header("Начальное положение задать здесь")]
    public Quaternion toDegrees;

    [Header("-")]
    public bool isRotating;

    public float rotationDirection;
    public float speedRotating;
 
    private void Start()
    {
        transform.rotation = toDegrees; 
    }

    private void Update()
    {

        RotateThis();
    

        // if (isRotating)
        // {
        //     Rotate();
        // }   
    }

    // public void Rotate()
    // {
    //     transform.Rotate(Time.deltaTime * rotation * rotationDirection);
    // }

    public void ChangeDegrees(int degree, float rotationDirection)
    {
        toDegrees = Quaternion.Euler(0, 0, toDegrees.eulerAngles.z + degree*rotationDirection);

        this.rotationDirection = rotationDirection;
    }

    public void RotateThis()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, toDegrees, speedRotating);
    }



}
