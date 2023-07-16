using System;
using UnityEngine;
using System.Collections;

public class scr_SlimeForm : scr_PlayerFormBase
{
    public float interactionRadius = 0.3f;

    public static bool isPipeCrawling = false;

    private bool isDashing;

    public float dashRate = 1f;
    [SerializeField] private float nextDash;


    private void Awake()
    {
        sprite = Resources.Load<Sprite>("Slime");
    }
    
    public override void Move(Rigidbody2D rb, Vector2 moveDirection, float moveSpeed, 
        float movementSmoothing)
    {
        if (isDashing) return;

        if (isPipeCrawling)
        {
            Vector2 velocity = Vector2.zero;
            Vector2 targetVelocity;
            rb.gravityScale = 0f;
            targetVelocity = new Vector3(moveDirection.x * moveSpeed * 0.75f, 
                moveDirection.y * moveSpeed * 0.75f);
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 
                movementSmoothing);
        }
        else
        {
            rb.gravityScale = 0.65f;

            base.Move(rb, moveDirection, moveSpeed, movementSmoothing);
        }

    }

    public override void Jump(Rigidbody2D rb, float jumpPower, AnimationCurve animationCurve)
    {
        if (!isPipeCrawling)
        {
            base.Jump(rb, jumpPower,animationCurve);
        }
    }
    
    public override void Attack()
    {
        //scr_EventSystem.instance.slimeHasAttacked.Invoke();
    }
    
     
    
    public override void MoveActiveSkill(Rigidbody2D rb, float direction,
        float dashSpeed, float dashTime, AnimationCurve dashSpeedCurve)
    {
        if (Time.time > nextDash)
        {
            nextDash = Time.time + dashRate;

            StartCoroutine(Dash(rb, direction, dashTime, dashSpeed, dashSpeedCurve));
        }
    }

    private IEnumerator Dash(Rigidbody2D rb, float direction, float dashTime, float dashSpeed, 
        AnimationCurve dashSpeedCurve)
    {
        if (isDashing) yield break;

        isDashing = true;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        
        var elapsedTime = 0f;
        while (elapsedTime < dashTime)
        {
            var progress = elapsedTime / dashTime;
            var velocityMultiplier = direction * dashSpeed * dashSpeedCurve.Evaluate(progress);

            //ApplyVelocity(direction, velocityMultiplier);
            rb.velocity = new Vector2(velocityMultiplier, 0);

            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isDashing = false;
        yield break;
    }
    
    public override void Skill_3()
    {

    }

    public override void PickObject()
    {
        if (GetInteractableObjects(formBehavior.gameObject.transform, interactionRadius, 
                LayerMask.GetMask("InteractableObjects")).Length != 0)
        {
            Collider2D[] targets = GetInteractableObjects(formBehavior.gameObject.transform, 
                interactionRadius, LayerMask.GetMask("InteractableObjects"));
            if (targets[0].gameObject.GetComponent<IPickable>() == null)
            {
                //Destroy(targets[0].gameObject);
            }
            else
            {
                PickObject(targets[0].gameObject);
            }
        }
        else
        {
            DropCurrentPickedObject();
        }
    }

 
    
    

    public override void StopUsingCurrentForm()
    {
        DropCurrentPickedObject();
    }

    public void PickObject(GameObject target)
    {
        DropCurrentPickedObject();

        target.GetComponent<IPickable>().StartInteraction();

        target.transform.parent = formBehavior.gameObject.transform;
        target.transform.position = new Vector3(formBehavior.gameObject.transform.position.x,
            formBehavior.gameObject.transform.position.y + 0.25f,
            formBehavior.gameObject.transform.position.z);
        
        scr_Player.currentPickedObject = target;
    }

    public void DropCurrentPickedObject()
    {
        if (scr_Player.currentPickedObject != null)
        {
            scr_Player.currentPickedObject.GetComponent<IPickable>().StopInteraction();

            scr_Player.currentPickedObject = null;
        }
    }

    
}

