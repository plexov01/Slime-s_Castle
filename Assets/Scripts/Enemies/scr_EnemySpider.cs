using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemySpider : MonoBehaviour
{
    [Header("Player")]
    private Transform player;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    [SerializeField] [Range(0, 30f)] private float spotTriggerSize;
    private BoxCollider2D spotCollider;
    private BoxCollider2D damageCollider;
    private Vector3 defaultPosition;

    [Header("Web")]
    [SerializeField] private float webWidth;
    [SerializeField] private Transform web;
    private SpriteRenderer webSprite;
    private Vector3 webDefaultPosition;
    private float webDefaultY;

    [Header("Attack Parameters")]
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackIdleDuration;
    private Vector3 attackSpot;

    [Header("Movement Parameters")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool[] waypointsToStay;
    [SerializeField] private float waypoint0Position;
    [SerializeField] private float waypoint1Position;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float idleDuration;
    private int waypointIndex = 0;
    private Vector3 initScale;
    private bool moveCoroutineIsRunning = false;
    private bool directionSelected = true;
    private float deltaX;

    [Header("Sight")]
    [SerializeField] private LayerMask raycastMask;
    private RaycastHit2D[] sightHits;
    [SerializeField] private LayerMask groundLayer;
    private bool platformOnPath;
    private bool playerOnPath;

    [Header("Debug Info")]
    public bool playerSpotted = false;
    public bool playerDamaged = false;
    public bool returnedToStart = true;
    public bool reachedBottom = false;
    public bool enteredTrigger = false;

    private void Awake()
    {
        webSprite = web.GetComponent<SpriteRenderer>();
        damageCollider = enemy.GetComponent<BoxCollider2D>();
        spotCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        player = scr_GameManager.instance.player.transform; ;
        initScale = enemy.localScale;
        webSprite.size = Vector2.zero;
        webDefaultY = web.position.y;

        if (waypoint0Position <= waypoint1Position)
        {
            waypoints[0].localPosition = new Vector3(waypoint0Position, 0f);
            waypoints[1].localPosition = new Vector3(waypoint1Position, 0f);
        }
        else
        {
            waypoints[0].localPosition = new Vector3(waypoint1Position, 0f);
            waypoints[1].localPosition = new Vector3(waypoint0Position, 0f);
        }

        spotCollider.offset = new Vector2(0, -(0.3f + spotTriggerSize / 2));
        spotCollider.size = new Vector2(0.04f, spotTriggerSize);
        StartCoroutine(Move());
    }

    private void FixedUpdate()
    {
        if (!moveCoroutineIsRunning && !playerSpotted && returnedToStart)
        {
            if (!directionSelected)
            {
                Flip(2);
                directionSelected = true;
            }

            StartCoroutine(Move());
        }

        if (playerSpotted)
        {
            returnedToStart = false;
            enemy.position = Vector2.MoveTowards(enemy.position, attackSpot, attackSpeed * Time.deltaTime);
            web.position = Vector2.MoveTowards(web.position, attackSpot, attackSpeed / 2 * Time.deltaTime);
            webSprite.size = new Vector2(webWidth, Vector2.Distance(enemy.position, web.position) * 2f);

            if (enemy.position == attackSpot || CheckIfOverlap(groundLayer) || playerDamaged)
            {
                StartCoroutine(Wait());
            }
        }

        if (reachedBottom)
        {
            enemy.position = Vector2.MoveTowards(enemy.position, defaultPosition, attackSpeed * Time.deltaTime);
            web.position = Vector2.MoveTowards(web.position, webDefaultPosition, attackSpeed / 2 * Time.deltaTime);
            webSprite.size = new Vector2(webWidth, Vector2.Distance(enemy.position, web.position) * 2f);
            
            if (enemy.position == defaultPosition)
            {
                enteredTrigger = false;
                reachedBottom = false;
                returnedToStart = true;
                directionSelected = false;
                spotCollider.enabled = true;
                web.position = webDefaultPosition;
                webSprite.size = Vector2.zero;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryAttack(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryAttack(collision);
    }

    private void TryAttack(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !enteredTrigger)
        {
            enteredTrigger = true;
            defaultPosition = enemy.position;
            webDefaultPosition = web.position;
            attackSpot = spotCollider.bounds.center - new Vector3(0, spotCollider.bounds.extents.y) + new Vector3(0, damageCollider.bounds.extents.y);

            if (PlayerInSight())
            {
                spotCollider.enabled = false;
                playerSpotted = true;
            }
            else
            {
                enteredTrigger = false;
            }
        }
    }

    private IEnumerator Wait()
    {
        playerSpotted = false;
        Flip(1);
        yield return new WaitForSeconds(attackIdleDuration);
        reachedBottom = true;
    }

    private IEnumerator Move()
    {
        moveCoroutineIsRunning = true;

        enemy.position = Vector2.MoveTowards(enemy.position,
            waypoints[waypointIndex].position, moveSpeed * Time.fixedDeltaTime);
        spotCollider.transform.position = Vector2.MoveTowards(spotCollider.transform.position,
            waypoints[waypointIndex].position, moveSpeed * Time.fixedDeltaTime);
        web.position = Vector2.MoveTowards(web.position,
            new Vector2(waypoints[waypointIndex].position.x, webDefaultY), moveSpeed * Time.fixedDeltaTime);

        if (enemy.position == waypoints[waypointIndex].position)
        {
            if (waypointsToStay[waypointIndex])
            {
                yield return new WaitForSeconds(idleDuration);
            }

            if (waypointIndex < waypoints.Length - 1)
            {
                deltaX = waypoints[waypointIndex + 1].position.x - waypoints[waypointIndex].position.x;
                waypointIndex++;
            }
            else
            {
                deltaX = waypoints[0].position.x - waypoints[waypointIndex].position.x;
                waypointIndex = 0;
            }

            Flip(0);
        }

        moveCoroutineIsRunning = false;
    }

    private void Flip(int reason)
    {
        switch (reason)
        {
            case 0:
                enemy.localScale = (deltaX > 0) ? new Vector3(initScale.x, initScale.y, initScale.z) : enemy.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
                break;
            case 1:
                enemy.localScale = (enemy.position.x > player.position.x) ? new Vector3(-initScale.x, initScale.y, initScale.z) : enemy.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
                break;
            case 2:
                if (enemy.position.x > player.position.x)
                {
                    waypointIndex = 0;
                    enemy.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
                }
                else
                {
                    waypointIndex = 1;
                    enemy.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
                }
                break;
            default:
                break;
        }
    }

    private bool CheckIfOverlap(LayerMask mask)
    {
        return Physics2D.OverlapBoxAll(damageCollider.transform.position, damageCollider.bounds.size, 0, mask).Length != 0;
    }

    private bool PlayerInSight()
    {
        platformOnPath = false;
        playerOnPath = false;

        //Debug.DrawRay(transform.position, new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y).normalized * aggroRange, Color.red);
        sightHits = Physics2D.BoxCastAll(enemy.position, damageCollider.bounds.size, 0, Vector2.down, raycastMask);

        for (int x = 1; x < sightHits.Length; x++)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(new Vector3(transform.parent.position.x + waypoint0Position, transform.position.y), 0.1f);
        Gizmos.DrawWireSphere(new Vector3(transform.parent.position.x + waypoint1Position, transform.position.y), 0.1f);
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - (0.3f + spotTriggerSize / 2)), new Vector3(0.04f, spotTriggerSize));
    }

}
