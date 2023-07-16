using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_parallaxEffect : MonoBehaviour
{
    [SerializeField] private Transform followingTarget;
    [SerializeField] private GameObject followingTargetObject;
    [Header("0 - stand, 1 - completely move after player")]
    [SerializeField, Range(0f, 1f)] private float parallaxHorizontalStrength;
    [SerializeField, Range(0f, 1f)] private float parallaxVerticalStrength;
    // [SerializeField] private bool disableVerticalParallax;

    private Vector3 targetPreviousPosition;
    private GameObject camera;

    private SpriteRenderer _spriteRenderer;

    private scr_CameraManager CameraManager;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        CameraManager = scr_CameraManager.instance;

        camera = CameraManager.mainVcamGameObject;

        if (!followingTarget)
        {
            followingTarget = camera.transform;
        }
        

        targetPreviousPosition = followingTarget.position;


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = followingTarget.position - targetPreviousPosition;

        transform.position += new Vector3(delta.x * parallaxHorizontalStrength, delta.y * parallaxVerticalStrength, 0);
        targetPreviousPosition = followingTarget.position;
        

    }

    private void OnDrawGizmosSelected()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawWireCube(transform.position,new Vector3(_spriteRenderer.size.x*transform.localScale.x*(1/(1-parallaxHorizontalStrength)),_spriteRenderer.size.y*transform.localScale.y*(1/(1-parallaxVerticalStrength))));

    }
}
