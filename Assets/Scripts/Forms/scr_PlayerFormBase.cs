using System;
using System.Collections;
using UnityEngine;

public abstract class scr_PlayerFormBase: MonoBehaviour
{
    public scr_cnpt_FormBehavior formBehavior;
    public static bool holdSkillisActive;
    public static bool isGrounded;
    protected float overlapRadius = 0.07f;
    public float lightIntensity = 0.5f;
    public Sprite sprite;

    [SerializeField]protected Transform checkers;

    public static bool inKnockback = false;

    public virtual void Move(Rigidbody2D rb, Vector2 moveDirection, float moveSpeed, 
        float movementSmoothing)
    {
        rb.gravityScale = 0.65f;
        Vector2 targetVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y);
        Vector2 velocity = Vector2.zero;

        if (!inKnockback)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        }

        isGrounded = CheckIfOverlap(rb.transform.GetChild(2).transform, overlapRadius, 
            LayerMask.GetMask("Platforms"));
    }

    public virtual void Jump(Rigidbody2D rb, float jumpPower, AnimationCurve animationCurve)
    {
        bool isGrounded = CheckIfOverlap(checkers.GetChild(1).transform, overlapRadius, 
            LayerMask.GetMask("Platforms"));
        
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            StartCoroutine(JumpTimer());
        }
    }
    
    public virtual void Skill_3()
    {

    }

    public virtual void StopUsingCurrentForm()
    {

    }

    public virtual void PickObject()
    {

    }

    public virtual void MoveActiveSkill(Rigidbody2D rb, float direction,
            float dashSpeed, float dashTime, AnimationCurve dashSpeedCurve)
    {

    }
    
    public virtual void Attack()
    {

    }
    

    protected bool CheckIfOverlap(Transform checker, float radius, LayerMask mask)
    {
        return Physics2D.OverlapCircleAll(checker.position, radius, mask).Length != 0;
    }
    
    protected bool CheckIfOverlap(Transform[] checkers, float radius, LayerMask mask)
    {
        bool isOverlap = false;

        foreach (var checker in checkers)
        {
            isOverlap = isOverlap || Physics2D.OverlapCircleAll(checker.position, 
                radius, mask).Length != 0;
        }
        return isOverlap;
    }

    protected Collider2D[] GetInteractableObjects(Transform checker, float radius, LayerMask mask)
    {
        return Physics2D.OverlapCircleAll(checker.position, radius, mask);
    }

    private IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(0.1f);
        scr_cnpt_BaseMovement.jumpPressed = false;
    }
}
