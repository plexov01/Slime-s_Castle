using UnityEngine;

public class scr_SpiderForm : scr_PlayerFormBase
{
    public GameObject SpiderWebShot;

    public float interactionRadius = 0.3f;

    private void Awake()
    {
        sprite = Resources.Load<Sprite>("Spider");
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


    public override void Jump(Rigidbody2D rb, float jumpPower, AnimationCurve animationCurve)
    {
        bool isGrounded = CheckIfOverlap(checkers.GetChild(1).transform, overlapRadius, 
            LayerMask.GetMask("Platforms"));

        if (isGrounded && !(holdSkillisActive && IsTouchingPlatform(rb)))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
    }

    public override void Move(Rigidbody2D rb, Vector2 moveDirection, float moveSpeed, 
        float movementSmoothing)
    {

        Vector2 velocity = Vector2.zero;
        Vector2 targetVelocity;

        if (holdSkillisActive && IsTouchingPlatform(rb))
        {
            rb.gravityScale = 0;
            targetVelocity = new Vector3(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
            isGrounded = true;
        }
        else
        {
            rb.gravityScale = 0.65f;
            targetVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y);
            isGrounded = false;
        }

        if (!inKnockback)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        }
    }
    
    public override void Attack()
    {

    }
    
    public override void Skill_3()
    {

    }
    
    private bool IsTouchingPlatform(Rigidbody2D rb)
    {
        
        Transform[] checkersArray = new Transform[] {
            checkers.GetChild(0),
            checkers.GetChild(1),
            checkers.GetChild(3)
        };

        return CheckIfOverlap(checkersArray, overlapRadius, LayerMask.GetMask("Platforms"));
    }
}
