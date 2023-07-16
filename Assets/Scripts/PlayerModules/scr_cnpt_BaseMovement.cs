using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class scr_cnpt_BaseMovement : MonoBehaviour
{
    [Range(-20, 0f)] [SerializeField] private float maxFallSpeed = -10f;
    
    
    Rigidbody2D _rb;
    [SerializeField]scr_cnpt_FormBehavior formBehavior;

    [SerializeField] private AnimationCurve _yAnimationCurve;
    [SerializeField] private float _jumpHeight = 5f;
    
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private float _moveSpeed = 5f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .01f;

    public Transform groundChecker;
    public LayerMask whatIsGround;
    [SerializeField] private float groundCheckRadius = 0.17f;

    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private AnimationCurve dashSpeedCurve;

    public bool isCharacterTurnedRight;

    InputManager input;

    scr_TilemapManager tilemapManager;

    public static bool jumpPressed = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); 
        //formBehavior = GetComponent<scr_cnpt_FormBehavior>();

        input = InputManager.instance;
        
        input.playerInput.actions["Jump"].performed += Jump;
        input.playerInput.actions["HoldSkill"].performed +=
            context => scr_PlayerFormBase.holdSkillisActive = true; 
        input.playerInput.actions["HoldSkill"].canceled +=
            context => scr_PlayerFormBase.holdSkillisActive = false;

        
        input.playerInput.actions["Skill"].performed += MoveActiveSkill;
    }

    private void Start()
    {
        tilemapManager = scr_TilemapManager.instance;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        //formBehavior._currentForm.Jump(_rb, _jumpPower);

        if (tilemapManager.GetTileCanJump(transform.position) && !scr_PlayerFormBase.inKnockback)
        {
            jumpPressed = true;
            transform.SetParent(null);
            formBehavior._currentForm.Jump(_rb, _jumpHeight, _yAnimationCurve);
        }

        //formBehavior._currentForm.Jump(_rb,_jumpHeight, _yAnimationCurve);
    }
    private void MoveActiveSkill(InputAction.CallbackContext context)
    {
        float direction = 1f;
        if (!isCharacterTurnedRight)
        {
            direction = -1f;
        }
        
        formBehavior._currentForm.MoveActiveSkill(_rb, direction, dashSpeed, 
            dashTime, dashSpeedCurve);
        
  
    }
    

    private void OnDestroy()
    {
        input.playerInput.actions["Jump"].performed -= Jump;

        input.playerInput.actions["HoldSkill"].performed -=
            context => scr_PlayerFormBase.holdSkillisActive = true; 
        input.playerInput.actions["HoldSkill"].canceled -=
            context => scr_PlayerFormBase.holdSkillisActive = false;

        
        input.playerInput.actions["Skill"].performed -=  MoveActiveSkill;
    }

    

    void FixedUpdate()
    {
        Vector2 moveDirection = input.playerInput.actions["Movement"].ReadValue<Vector2>();
        if (moveDirection.x > 0)
        {
            isCharacterTurnedRight = true;
        }
        else if (moveDirection.x < 0)
        {
            isCharacterTurnedRight = false;
        }

        float adjustedSpeed = tilemapManager.GetTileMovementSpeed(transform.position) * _moveSpeed;

        formBehavior._currentForm.Move(_rb, moveDirection, adjustedSpeed, movementSmoothing);

        if (_rb.velocity.y < maxFallSpeed)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, maxFallSpeed);
        }
    }
}
