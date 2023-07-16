using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class scr_FireflyForm : scr_PlayerFormBase
{
    private int jumpsLeft = 1;
    private float glideSpeed = 2f;
    public float interactionRadius = 0.3f;

    [FormerlySerializedAs("light2D")] [SerializeField] private Light2D light;
    [SerializeField] private float flashOuterRadius;
    [SerializeField] private float flashInnerRadius;
    [SerializeField] private float flashIntensity;
    [SerializeField] private float flashTime;
    
    [SerializeField] private GameObject lightPrefab;

    private void Awake()
    {
        sprite = Resources.Load<Sprite>("Firefly");
        //lightIntensity = 1.1f;
    }
    
    public override void Move(Rigidbody2D rb, Vector2 moveDirection, float moveSpeed, 
        float movementSmoothing)
    {
        Vector2 velocity = Vector2.zero;
        Vector2 targetVelocity;
        rb.gravityScale = 0.65f;

        if (holdSkillisActive && rb.velocity.y < 0)
        {
            targetVelocity = new Vector3(moveDirection.x * moveSpeed, -glideSpeed);
        }
        else
        {
            targetVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y);
        }

        if (!inKnockback)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        }

        if (isGrounded)
        {
            jumpsLeft = 1;
        }

        isGrounded = CheckIfOverlap(checkers.GetChild(1).transform, overlapRadius, 
            LayerMask.GetMask("Platforms"));
    }
    
    public override void Attack()
    {

    }

    public override void Jump(Rigidbody2D rb, float jumpPower, AnimationCurve animationCurve)
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
        else if (jumpsLeft > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpsLeft -= 1;
        }
    }
    
    public override void Skill_3()
    {
        Instantiate(lightPrefab,transform);
        //Instantiate(lightPrefab,transform.position,transform.rotation);
    }

    // IEnumerator Flash()
    // {
    //     float tempPointLightOuterRadius = light.pointLightOuterRadius;
    //     float tempPointLightInnerRadius = light.pointLightInnerRadius;
    //     float tempIntensity = light.intensity;
    //     
    //     light.pointLightOuterRadius = flashOuterRadius;
    //     light.pointLightInnerRadius = flashInnerRadius;
    //     light.intensity = flashIntensity;
    //
    //     yield return new WaitForSeconds(flashTime);
    //
    //     light.pointLightOuterRadius = tempPointLightOuterRadius;
    //     light.pointLightInnerRadius = tempPointLightInnerRadius;
    //     light.intensity = tempIntensity;
    // }
    


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
}
