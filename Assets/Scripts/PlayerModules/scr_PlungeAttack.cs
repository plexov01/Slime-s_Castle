using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scr_PlungeAttack : MonoBehaviour
{
    private InputManager input;
    private Transform player;
    private scr_cnpt_FormBehavior formBehavior;
    private BoxCollider2D boxCollider2D;
    private GameObject plungeAttack;

    [Header("Checkers")]
    [SerializeField] private Transform groundChecker;
    [SerializeField] private LayerMask platformsLayer;
    private Vector2 boxSize;

    [Header("Attack")]
    [SerializeField] private Vector2 initColliderSize;
    [SerializeField] private float waveSize;
    [SerializeField] private float speed;
    [SerializeField] private int cooldown;
    [SerializeField] private float forceY;
    public static float damage = 1f;
    private Vector2 attackColliderSize;
    private Vector2 colliderSize;

    [Header("Debug Info")]
    public bool cooldownEnded = true;
    public bool canAttack = false;
    public bool attackPerformed = false;
    public bool damageWaveIsActive = false;
    public static bool isGrounded;
    public static bool movingDownAfterAttack = false;

    private void Awake()
    {
        input = InputManager.instance;
        
        input.playerInput.actions["SuperAttack"].performed += PressAction;
    }

    private void Start()
    {
        formBehavior = scr_cnpt_FormBehavior.instance;
        player = transform.parent;
        boxSize = new Vector2(player.GetComponent<CircleCollider2D>().radius * 0.95f, 0.1f);
        attackColliderSize = new Vector2(initColliderSize.x * waveSize, initColliderSize.y);
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapBox(groundChecker.position, boxSize, 0, platformsLayer);

        if (isGrounded && attackPerformed)
        {
            movingDownAfterAttack = false;
        }

        if (!isGrounded && !attackPerformed && cooldownEnded)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }

        if (isGrounded && attackPerformed && !damageWaveIsActive)
        {
            damageWaveIsActive = true;
            plungeAttack.transform.position = new Vector3(player.position.x, player.position.y);
        }
        else if (!damageWaveIsActive && attackPerformed)
        {
            plungeAttack.transform.position = new Vector3(player.position.x, player.position.y);
        }

        if (damageWaveIsActive)
        {
            colliderSize = Vector2.Lerp(boxCollider2D.size, attackColliderSize, speed * Time.deltaTime);
            boxCollider2D.size = new Vector2(colliderSize.x, initColliderSize.y);
            
            if (Mathf.Abs(boxCollider2D.size.x - attackColliderSize.x) <= 0.01f)
            {
                StartCoroutine(AttackEnd());
            }
        }
    }

    private void PressAction(InputAction.CallbackContext context)
    {
        if (formBehavior._currentForm.GetType() == typeof(scr_FireflyForm))
        {
            scr_TilemapManager.instance.SetTileOnFire(scr_GameManager.instance.player.transform.position);
        }

        //scr_AudioManager.StopRepeatSound(scr_AudioManager.Sound.PlayerAttack, player.gameObject);
        if (canAttack && formBehavior._currentForm.GetType() == typeof(scr_SlimeForm))
        {
            canAttack = false;
            attackPerformed = true;
            movingDownAfterAttack = true;
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -forceY);

            plungeAttack = new GameObject("plungeAttack")
            {
                tag = "PlungeAttack",
                layer = 12
            };
            plungeAttack.AddComponent<scr_PlungeAttackDamage>();
            boxCollider2D = plungeAttack.AddComponent<BoxCollider2D>();
            boxCollider2D.size = initColliderSize;
            boxCollider2D.isTrigger = true;
        }
    }

    private IEnumerator AttackEnd()
    {
        cooldownEnded = false;
        boxCollider2D.enabled = false;
        yield return new WaitForSeconds(0f);
        damageWaveIsActive = false;
        Destroy(plungeAttack);
        attackPerformed = false;
        yield return new WaitForSeconds(cooldown);
        cooldownEnded = true;
    }

    private void OnDestroy()
    {
        input.playerInput.actions["SuperAttack"].performed -= PressAction;
    }
}
