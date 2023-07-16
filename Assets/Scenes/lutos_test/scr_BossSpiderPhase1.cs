using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpiderBossPhase1;

public class scr_BossSpiderPhase1 : MonoBehaviour, ITrigger
{
    public Animator animator;

    [SerializeField]private int maxHealth = 3;
    [SerializeField]private int currentHealth = 3;
    [SerializeField]private string currentState;
    
    [SerializeField]private bool bossWasTriggered;
    [SerializeField]private bool isFighting;
    
    private StateMachine stateMachine = new StateMachine();
    private Rigidbody2D rb;
    
    // private scr_cnpt_FormBehavior formBehavior;
    [Header("Appearing settings")] 
    [SerializeField] private AnimationCurve appearingSpeedCurve;
    [SerializeField] private float appearingTime;
    [SerializeField] private Vector3 appearingOffset = new Vector3(2.5f, 1.5f,0);

    [Header("Moving settings")] 
    [SerializeField] private Transform waitingPoint;
    [SerializeField] private float movingSpeed;
    private bool isMovingToWaitingPoint;
    
    //DEBUG====
    [Header("DEBUG")]
    [SerializeField] private bool hitIt = false;
    [SerializeField] private bool stunIt = false;
    ///===

    private void Awake()
    {
        stateMachine.Init(new WaitingForStart(stateMachine),animator,this,gameObject);
        rb = GetComponent<Rigidbody2D>();
        scr_EventSystem.instance.playerDeath.AddListener(RestartFight);
    }

    private void Update()
    {
        stateMachine.currentState.UpdateLogic();

        //DEBUG====
        currentState = stateMachine.currentState.ToString();
        if(hitIt)
            Hit();
        if (stunIt)
            Stun();
        ///===
    }

    public bool MovingToWaitingPoint()
    {
        Vector3 direction = (waitingPoint.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * (movingSpeed * Time.deltaTime));

        return Vector3.Distance(rb.position, waitingPoint.position) > 0.1f;
    }

    public void Appear(bool isChasingFromTheLeft)
    {
        //transform.position = scr_Player.instance.transform.position + new Vector3(2.5f * Mathf.Pow(-1,Convert.ToInt32(isChasingFromTheLeft)) ,1.5f,0);
        
        StartCoroutine(IAppear(isChasingFromTheLeft));
        
        // Vector3 direction = (waitingPoint.position - transform.position).normalized;
        // rb.MovePosition(transform.position + direction * (movingSpeed * Time.deltaTime));
    }

    

    private IEnumerator IAppear(bool isChasingFromTheLeft)
    {
        //if (isDashing) yield break;

        //isAppearing = true;

        //rb.velocity = new Vector2(rb.velocity.x, 0);


        var strikePoint = Vector3.zero;
        var newOffset = new Vector3(appearingOffset.x * Mathf.Pow(-1, Convert.ToInt32(isChasingFromTheLeft)), appearingOffset.y, appearingOffset.z);
        var elapsedTime = 0f;
        while (elapsedTime < appearingTime)
        {
            var progress = elapsedTime / appearingTime;
            //var velocityMultiplier =  appearingSpeedCurve.Evaluate(progress); // direction * dashSpeed *
            

            //ApplyVelocity(direction, velocityMultiplier);

            //var offset = new Vector3(2.5f * Mathf.Pow(-1, Convert.ToInt32(isChasingFromTheLeft)), 1.5f, 0);

            //Vector3 offset = Vector3.zero;

            if (progress < 0.3f)
            {
                strikePoint = scr_Player.instance.transform.position;
            }
            else
            {
                newOffset *= (1 - Time.deltaTime);
            }

            var target = strikePoint + newOffset;
            
            //var newPosition = target * velocityMultiplier;
            
            rb.MovePosition(target);
            
            //rb.velocity = new Vector2(velocityMultiplier, 0);

            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void Hit()
    {
        stateMachine.currentState.Hit();
        //DEBUG====
        hitIt = false;
        ///===
    }
    
    //DEBUG====
    public void Stun()
    {
        stateMachine.ChangeState(new Stunned(stateMachine));
    }
    ///===

    public void TakeDamage()
    {
        currentHealth -= 1;
        //check if not 0
    }

    public void Trigger()
    {
        if(isFighting)
            return;
        
        isFighting = true;
        
        if (!bossWasTriggered)
        {
            stateMachine.ChangeState(new StartingFight(stateMachine));
            bossWasTriggered = true;
        }
        else
            stateMachine.ChangeState(new Waiting(stateMachine));

    }

    private void RestartFight()
    {
        isFighting = false;
        currentHealth = maxHealth;
        transform.position = waitingPoint.position;
        
        stateMachine.ChangeState(new WaitingForStart(stateMachine));
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log($"triggering by {col.gameObject.name}");
        
        if (col.gameObject.TryGetComponent<TestLightScript>(out var lightScript))
            stateMachine.ChangeState(new Stunned(stateMachine));
    }

    private void OnDestroy()
    {
        scr_EventSystem.instance.playerDeath.RemoveListener(RestartFight);
    }

    
}
