using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossSpiderPhase2 : MonoBehaviour, ITrigger
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float maxDistanceDelta;
    [SerializeField] private float maxDistanceToPlayer;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float superSpeed = 10f;
    [SerializeField] private float speedAcceleration = 0.1f;
    
    [SerializeField] private Transform startPosition;
    
    private int currentWaypointIndex = 0;

    private bool appearingAnimationPlayed = false;
    private bool bossWasTriggered = false;

    private Transform playerTransform;
    
    private void Awake()
    {
        scr_EventSystem.instance.playerDeath.AddListener(RestartFight);
        playerTransform = scr_Player.instance.transform;
    }

    private void FixedUpdate()
    {
        if (!bossWasTriggered)
            return;
        if (currentWaypointIndex >= waypoints.Count)
        {
            StartPhase2FinalAnimation();
            return;
        }

        MoveTowardsWaypoint();
        
    }

    private void MoveTowardsWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Count)
            return;
        if (waypoints[currentWaypointIndex] is null)
            return;

        if ((transform.position - waypoints[currentWaypointIndex].position).magnitude <= maxDistanceDelta)
        {
            currentWaypointIndex++;
            return;
        }

        var distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);
        float targetSpeed;
        if (distanceToPlayer >= maxDistanceToPlayer)
        {
            targetSpeed = superSpeed;
        }
        else
        {
            targetSpeed = speed;
        }
        
        speed = Mathf.Lerp(speed, targetSpeed, speedAcceleration * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, 
            waypoints[currentWaypointIndex].position, maxDistanceDelta * Time.deltaTime * targetSpeed);
    }
    
    private void RestartFight()
    {
        bossWasTriggered = false;
        currentWaypointIndex = 0;
        transform.position = startPosition.position;
    }

    private void StartPhase2FinalAnimation()
    {
        //animation?
        //destroy gameObject when animation was ended
        Destroy(gameObject);
    }

    private void StartPhase2()
    {
        if (appearingAnimationPlayed)
        {
            bossWasTriggered = true;
        }
        else
        {
            //add start fight animation
            appearingAnimationPlayed = true;
            bossWasTriggered = true;
        }
            
    }

    public void Trigger()
    {
        StartPhase2();
    }

    private void OnDestroy()
    {
        scr_EventSystem.instance.playerDeath.RemoveListener(RestartFight);
    }
}
