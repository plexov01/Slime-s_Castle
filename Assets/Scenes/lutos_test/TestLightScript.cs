using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TestLightScript : MonoBehaviour
{
    private Light2D light;
    private CircleCollider2D collider2D;

    [SerializeField] GameObject LightCircle;
    
    [Header("Light change parameters (per frame)")]
    [SerializeField] float intensityChange = 0.005f;
    [SerializeField] Vector3 circleScaleChange;
    
    [SerializeField] Vector3 maxCircleScale;
    // [SerializeField] float LightCircle;
    // [SerializeField] float LightCircle;
    // [SerializeField] float LightCircle;

    private void Awake()
    {
        collider2D = GetComponent<CircleCollider2D>();
        light = GetComponent<Light2D>();
    }

    void FixedUpdate()
    {
        //light.pointLightOuterRadius -= 0.001f; 
        //light.pointLightInnerRadius -= 0.001f; 
        light.intensity -= intensityChange;

        LightCircle.transform.localScale += circleScaleChange;
        collider2D.radius += circleScaleChange.x/2;
        
        //collider2D.radius -= 0.001f;

        // if (light.intensity <= 0)
        // {
        //     Destroy(gameObject);
        // }
        if (LightCircle.transform.localScale.magnitude >= maxCircleScale.magnitude)
        {
            Destroy(gameObject);
        }
    }
}
