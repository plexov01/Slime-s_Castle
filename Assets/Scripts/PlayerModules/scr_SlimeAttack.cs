using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class scr_SlimeAttack : MonoBehaviour
{
    Transform player;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider2D;

    [SerializeField] private float groundAttackDuration;
    [SerializeField] private float airAttackDuration;
    [SerializeField] private float damage;

    private Vector3 zAxis = new Vector3(0, 0, 1);
    private Vector3 initPosition;
    private float angle = 90f;
    private bool canAttack = true;
    public bool isGrounded;

    private InputManager input;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        
        input = InputManager.instance;
        
        input.playerInput.actions["Attack"].performed += SlimeAttack;
    }


    private void Start()
    {
        //scr_EventSystem.instance.slimeHasAttacked.AddListener(SlimeAttack);
        
        player = transform.parent;
        initPosition = transform.localPosition;
    }

    private void Update()
    {
        isGrounded = scr_PlungeAttack.isGrounded;
        
        if (transform.localRotation.eulerAngles.z > 0)
        {
            if (player.localScale.x > 0)
                transform.RotateAround(player.position, zAxis, -angle / airAttackDuration * Time.deltaTime);
            else if (player.localScale.x < 0)
                transform.RotateAround(player.position, zAxis, angle / airAttackDuration * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        //scr_EventSystem.instance.slimeHasAttacked.RemoveListener(SlimeAttack);
        input.playerInput.actions["Attack"].performed -= SlimeAttack;
    }

    private void SlimeAttack(InputAction.CallbackContext context)
    {
        
        if (canAttack && scr_cnpt_FormBehavior.instance._currentForm.GetType() == typeof(scr_SlimeForm))
        {
            if (isGrounded)
                StartCoroutine(GroundAttack());
            else
                StartCoroutine(AirAttack());
        }
    }

    IEnumerator GroundAttack()
    {
        scr_AudioManager.PlaySound("PlayerAttack", gameObject);
        //scr_AudioManager.PlayMusic(scr_AudioManager.Music.BossBattle);
        canAttack = false;

        spriteRenderer.enabled = true;
        capsuleCollider2D.enabled = true;
        yield return new WaitForSeconds(groundAttackDuration);
        spriteRenderer.enabled = false;
        capsuleCollider2D.enabled = false;

        canAttack = true;
    }

    IEnumerator AirAttack()
    {
        scr_AudioManager.PlaySound("AirAttack", gameObject);
        //scr_AudioManager.PlayMusic(scr_AudioManager.Music.BackGround);
        canAttack = false;
        transform.localRotation = Quaternion.Euler(0, 0, 90);
        transform.localPosition = new Vector3(0, initPosition.x, 0);

        spriteRenderer.enabled = true;
        capsuleCollider2D.enabled = true;
        yield return new WaitForSeconds(airAttackDuration);
        transform.localPosition = initPosition;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(groundAttackDuration - airAttackDuration);
        spriteRenderer.enabled = false;
        capsuleCollider2D.enabled = false;

        canAttack = true;
    }


    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Enemy"))
        {
            if (collider.gameObject.GetComponent<scr_FlytrapHealth>() != null)
            {
                if (collider.gameObject.GetComponent<scr_FlytrapHealth>().canTakeDamageFromNormalAttack)
                {
                    collider.gameObject.GetComponent<scr_IDamageable>().ApplyDamage(damage, gameObject.tag);
                }
            }
            else
            {
                collider.gameObject.GetComponent<scr_IDamageable>().ApplyDamage(damage, gameObject.tag);
            }
        }
    }

    protected bool CheckIfOverlap(Transform checker, float radius, LayerMask mask)
    {
        return Physics2D.OverlapCircleAll(checker.position, radius, mask).Length != 0;
    }
}
