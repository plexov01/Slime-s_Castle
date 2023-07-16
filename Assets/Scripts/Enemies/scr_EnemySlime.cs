using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemySlime : MonoBehaviour, scr_IDamageable
{
    [Header("Player")]
    private Transform player;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Checkers")]
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Transform endOfPlatformChecker;
    [SerializeField] private Transform wallChecker;
    [SerializeField] private LayerMask groundLayer;
    private Vector2 boxSize;
    private readonly float checkerRadius = 0.1f;

    [Header("Enemy Behaviour")]
    [SerializeField] private bool aggressive;
    [SerializeField] private bool toxic;

    [Header("Movement parameters")]
    [SerializeField] private float moveSpeed;
    private float moveDirection = 1;
    private bool movingLeft = false;
    private bool moveCoroutineIsRunning = false;
    private bool waiting = false;
    [SerializeField] private float idleDuration;

    [Header("Jump Attack")]
    [SerializeField] private Vector2 jumpZone;
    [SerializeField] private float jumpZoneOffset;
    private float initJumpZoneY;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float jumpHeight;
    private bool jumpWasPerformed = false;
    [SerializeField] private float jumpCooldown;
    private float jumpCooldownTimer;
    [SerializeField] private float maxHeight;
    private RaycastHit2D[] jumpHits;
    [SerializeField] private BoxCollider2D physicsBoxCollider;

    [Header("Sight")]
    [SerializeField] private LayerMask raycastMask;
    private RaycastHit2D[] sightHits;
    private bool platformOnPath;
    private bool playerOnPath;
    private float aggroRange;
    private float initAggroRange;

    [Header("Particles")]
    [SerializeField] private GameObject trailParticles;
    [SerializeField] private GameObject splashParticles;
    private ParticleSystem trailPartSys;
    private ParticleSystem splashPartSys;
    private bool tookDamage;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] [Range(0, 10f)] private float damageRate;
    private float nextDamage;
    public int mobID;
    private bool dead;

    [Header("Knockback")]
    [SerializeField] private float forceX;
    [SerializeField] private float forceY;

    [Header("Debug Info")]
    public bool closeToEndOfPlatform;
    public bool closeToWall;
    public bool isGrounded;
    public bool playerInJumpZone;
    public bool playerIsVisible;
    public bool playerSpotted = false;
    public bool flippedTowardsPlayer = false;


    private void Awake()
    {
        if (toxic)
        {
            trailPartSys = trailParticles.GetComponent<ParticleSystem>();
            splashPartSys = splashParticles.GetComponent<ParticleSystem>();
        }

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
        boxSize = new Vector2(transform.GetComponent<BoxCollider2D>().bounds.size.x * 0.9f, 0.1f);
        initJumpZoneY = jumpZone.y;
        currentHealth = maxHealth;
        aggroRange = jumpZone.x / 2 + jumpZoneOffset;
        initAggroRange = aggroRange;

        maxHeight = jumpHeight * jumpHeight / (2f * -Physics2D.gravity.y * rb.gravityScale);
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            closeToEndOfPlatform = !Physics2D.OverlapCircle(endOfPlatformChecker.position, checkerRadius, groundLayer);
            closeToWall = Physics2D.OverlapCircle(wallChecker.position, checkerRadius, groundLayer);
            isGrounded = Physics2D.OverlapBox(groundChecker.position, boxSize, 0, groundLayer);

            playerInJumpZone = (movingLeft) ? Physics2D.OverlapBox(transform.position - new Vector3(jumpZoneOffset, 0), jumpZone, 0, playerLayer)
                                            : Physics2D.OverlapBox(transform.position + new Vector3(jumpZoneOffset, 0), jumpZone, 0, playerLayer);

            playerIsVisible = PlayerInSight();

            AnimationController();

            if (isGrounded)
            {
                if (playerInJumpZone && playerIsVisible && aggressive)
                {
                    FlipToPlayerDuringAttack();
                    jumpZone.y = initJumpZoneY * 8f;
                    aggroRange = initAggroRange * 2f;
                }
                else
                {
                    jumpZone.y = initJumpZoneY;
                    aggroRange = initAggroRange;

                    if (!moveCoroutineIsRunning)
                    {
                        if (playerSpotted)
                        {
                            playerSpotted = false;
                            FlipToPlayerDuringAttack();
                        }

                        StartCoroutine(Move());
                    }
                }

                if (jumpWasPerformed)
                {
                    jumpWasPerformed = false;
                    rb.velocity = Vector2.zero;

                    if (toxic)
                    {
                        splashPartSys.Play();
                        trailPartSys.Play();
                    }

                    FlipToPlayerDuringAttack();
                }

                if (tookDamage && toxic)
                {
                    tookDamage = false;
                    trailPartSys.Play();
                }
            }

            jumpCooldownTimer += Time.fixedDeltaTime;
        }
    }

    private IEnumerator Move()
    {
        moveCoroutineIsRunning = true;
        flippedTowardsPlayer = false;
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);

        if (closeToEndOfPlatform || closeToWall)
        {
            rb.velocity = Vector2.zero;
            waiting = true;
            yield return new WaitForSeconds(idleDuration);
            waiting = false;

            if (isGrounded && !playerSpotted && !flippedTowardsPlayer)
            {
                Flip();
            }
        }

        moveCoroutineIsRunning = false;
    }

    private void JumpAttack()
    {
        //print("jump attack");
        if (jumpCooldownTimer >= jumpCooldown)
        {
            jumpCooldownTimer = 0f;
        }
        else
        {
            return;
        }
        //print("can jump attack");

        playerSpotted = false;
        float distance = player.position.x - transform.position.x;
        float height;

        if (isGrounded && playerInJumpZone)
        {
            if (toxic)
            {
                trailPartSys.Stop();
            }

            jumpHits = Physics2D.BoxCastAll(transform.position + new Vector3(0, maxHeight / 2 + 0.5f, 0), 
                new Vector2(physicsBoxCollider.size.x * 3, 0.01f), 0, Vector2.up, maxHeight - 0.5f, groundLayer);

            if (jumpHits.Length != 0)
            {
                height = Mathf.Sqrt(2 * -Physics2D.gravity.y * rb.gravityScale * (jumpHits[jumpHits.Length - 1].point.y - transform.position.y) / 2);
            }
            else
            {
                height = jumpHeight;
            }

            rb.AddForce(new Vector2(distance, height), ForceMode2D.Impulse);

            StartCoroutine(Wait(0));
        }
    }

    private IEnumerator Wait(int reason)
    {
        yield return new WaitForSeconds(0.1f);

        switch (reason)
        {
            case 0:
                jumpWasPerformed = true;
                break;
            case 1:
                tookDamage = true;
                break;
            default:
                break;
        }
    }

    private void FlipToPlayer()
    {
        rb.velocity = Vector2.zero;
        playerSpotted = true;
        float distance = player.position.x - transform.position.x;

        if (distance < 0 && !movingLeft)
        {
            Flip();
        }
        else if (distance > 0 && movingLeft)
        {
            Flip();
        }

        flippedTowardsPlayer = true;
    }

    private void FlipToPlayerDuringAttack()
    {
        float distance = player.position.x - transform.position.x;

        if (distance < 0 && !movingLeft)
        {
            Flip();
        }
        else if (distance > 0 && movingLeft)
        {
            Flip();
        }

        flippedTowardsPlayer = true;
    }

    private void Flip()
    {
        moveDirection *= -1;
        movingLeft = !movingLeft;
        transform.Rotate(0, 180, 0);
    }

    private void AnimationController()
    {
        if (aggressive)
        {
            anim.SetBool("canSeePlayer", playerInJumpZone && playerIsVisible || playerSpotted);
            anim.SetBool("isGrounded", isGrounded);
        }

        anim.SetBool("waiting", waiting);
    }

    private void OnDrawGizmosSelected()
    {
        /*Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(endOfPlatformChecker.position, checkerRadius);
        Gizmos.DrawWireSphere(wallChecker.position, checkerRadius);
        Gizmos.DrawWireCube(groundChecker.position, boxSize);*/
        Gizmos.color = Color.red;

        if (movingLeft)
        {
            Gizmos.DrawWireCube(transform.position - new Vector3(jumpZoneOffset, 0), jumpZone);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(jumpZoneOffset, 0), jumpZone);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, maxHeight / 2 + 0.5f, 0), new Vector2(physicsBoxCollider.size.x * 3, maxHeight - 0.5f));
    }

    private bool PlayerInSight()
    {
        platformOnPath = false;
        playerOnPath = false;

        Vector3 raycastStart = transform.position + new Vector3(0, physicsBoxCollider.bounds.extents.y);

        Debug.DrawRay(raycastStart, (player.position - raycastStart).normalized * aggroRange, Color.red);

        sightHits = Physics2D.RaycastAll(raycastStart, (player.position - raycastStart).normalized, aggroRange, raycastMask);

        for (int x = 0; x < sightHits.Length; x++)
        {
            //if (hits[x].transform.CompareTag("Platform"))
            if (sightHits[x].transform.gameObject.layer == LayerMask.NameToLayer("Platforms"))
            {
                platformOnPath = true;
                break;
            }

            if (sightHits[x].transform.CompareTag("Player"))
            {
                playerOnPath = true;
                break;
            }
        }

        return playerOnPath && !platformOnPath;
    }

    public void ApplyDamage(float damage, string tag, bool instantKill)
    {
        if (Time.time > nextDamage && canTakeDamage)
        {
            nextDamage = Time.time + damageRate;
            currentHealth -= damage;

            if (currentHealth > 0)
            {
                scr_AudioManager.PlaySound("EnemyHit", gameObject);
            }
            
            StartCoroutine(DamageEffect());

            if (tag == "SlimeAttack" || tag == "PlungeAttack")
            {
                if (toxic)
                {
                    trailPartSys.Stop();
                }

                if (isGrounded)
                {
                    rb.velocity = (player.position.x >= transform.position.x) ? new Vector2(-forceX, forceY) : rb.velocity = new Vector2(forceX, forceY);
                }

                StartCoroutine(Wait(1));
            }

            if (currentHealth <= 0)
            {
                canTakeDamage = false;
                StartCoroutine(Die());
            }
        }
    }

    private IEnumerator DamageEffect()
    {
        spriteRenderer.color = new Color(1, 0, 0, 0.75f);
        yield return new WaitForSeconds(damageRate / 2);
        spriteRenderer.color = Color.white;
    }

    public IEnumerator Die()
    {
        dead = true;
        yield return null;
        scr_EventSystem.instance.mobDeath.Invoke(mobID);
        scr_AudioManager.PlaySoundAtPosition("EnemyHit", gameObject.transform.position);
        
        if (toxic)
        {
            trailPartSys.Stop();
            splashPartSys.Stop();
            GetComponent<BoxCollider2D>().enabled = false;
            spriteRenderer.enabled = false;
            yield return new WaitUntil(() => trailPartSys.particleCount == 0 && splashPartSys.particleCount == 0);
        }
        
        Destroy(transform.parent.gameObject);
    }
}
