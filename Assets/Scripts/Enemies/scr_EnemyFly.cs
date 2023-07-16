using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class scr_EnemyFly : MonoBehaviour, scr_IDamageable
{
    public struct Graph
    {
        public GridGraph gridGraph;
        public Vector3 center;
        public Vector2 size;
    }

    [Header("Graph")]
    private Graph graph;

    [Header("Components")]
    private Transform enemy;
    private Seeker seeker;
    private Rigidbody2D rb;
    private AIPath aiPath;
    private Transform player;

    [Header("Enemy")]
    [SerializeField] private Transform model;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float aggroRange;
    [SerializeField] private float aggroRangeChasing;
    [SerializeField] private float aggroRangeReturning;
    private float initAggroRange;
    private float initAggroRangeChasing;
    private float initAggroRangeReturning;
    private Vector3 defaultPosition;
    private bool dead = false;

    [Header("Sight")]
    [SerializeField] private LayerMask raycastMask;
    private RaycastHit2D[] sightHits;
    private bool playerInRange = false;
    private bool playerIsVisible = false;
    private bool platformOnPath;
    private bool playerOnPath;

    [Header("Enemy Behaviour")]
    [SerializeField] private bool aggressive;
    [SerializeField] private bool checkLastSeenPosition;
    [SerializeField] private float waitOnLastSeenDuration;
    [SerializeField] private bool aggroAfterHit;
    [SerializeField] private bool toxic;

    [Header("Toxic Cloud")]
    [SerializeField] private Transform toxicCloud;
    [SerializeField] private float timeBeforeExplode;
    [SerializeField] private float timeBeforeDisappear;
    private bool startToxicCloudFirstStage = false;
    private bool startToxicCloudLastStage = false;
    [SerializeField] private Vector3 zeroScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 firstScale = new Vector3(2.5f, 2.5f, 2.5f);
    [SerializeField] private Vector3 secondScale = new Vector3(5f, 5f, 5f);
    [SerializeField] private Vector3 thirdScale = new Vector3(7.5f, 7.5f, 7.5f);
    [SerializeField] private float appearSpeed;
    [SerializeField] private float disappearSpeed;
    [SerializeField] private float disableColliderAlpha;
    [SerializeField] private float deleteToxicCloudAlpha;
    private bool deleteToxicCloud = false;
    private const int stages = 3;
    private Transform[] stage = new Transform[stages];
    private SpriteRenderer[] stageSpriteRenderer = new SpriteRenderer[stages];
    private CircleCollider2D toxicCloudCollider;

    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float idleDuration;
    private Vector3 initScale;
    private bool moveCoroutineIsRunning = false;
    private bool movingLeft = false;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool[] waypointsToStay;
    private int waypointIndex = 0;
    private Vector3 lastSeenPosition;
    private float deltaX;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] [Range(0, 10f)] private float damageRate;
    private float nextDamage;
    public int mobID;

    [Header("Debug info")]
    public bool returnedToStart = true;
    public bool returnedToLastSeen = false;
    public bool aggroed = false;
    public bool wasHit = false;
    public bool outOfZone = true;
    public bool isWaiting = false;
    public bool hasWaited = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    private void Awake()
    {
        enemy = GetComponent<Transform>();
        aiPath = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = model.GetComponent<SpriteRenderer>();

        if (toxic)
        {
            for (int i = 0; i < stages; i++)
            {
                stage[i] = toxicCloud.GetChild(i);
                stageSpriteRenderer[i] = stage[i].GetComponent<SpriteRenderer>();
            }
            toxicCloudCollider = stage[2].GetComponent<CircleCollider2D>();
        }
    }

    private void Start()
    {
        initAggroRange = aggroRange;
        initAggroRangeChasing = aggroRangeChasing;
        initAggroRangeReturning = aggroRangeReturning;

        initScale = (enemy.localScale.x >= 0) ? enemy.localScale : new Vector3(-enemy.localScale.x, enemy.localScale.y, enemy.localScale.z);

        player = scr_GameManager.instance.player.transform;

        uint graphId = (uint)Mathf.Log(seeker.graphMask.value, 2);
        print("name: " + gameObject.transform.parent.name + "; graphId: " + graphId);

        graph.gridGraph = AstarPath.active.data.FindGraphsOfType(typeof(GridGraph)).Cast<GridGraph>().ToArray()[graphId];
        graph.center = graph.gridGraph.center;
        graph.size = graph.gridGraph.size;

        //gridGraph = AstarPath.active.data.FindGraphsOfType(typeof(GridGraph)).Cast<GridGraph>().ToArray()[graphId];
        //gridGraphCenter = gridGraph.center;
        //gridGraphSize = gridGraph.size;

        if (toxic)
        {
            toxicCloud.gameObject.SetActive(false);

            for (int i = 0; i < stages; i++)
            {
                stage[i].localScale = zeroScale;
                stage[i].gameObject.SetActive(false);
            }
        }

        transform.position = waypoints[waypointIndex].transform.position;
        defaultPosition = enemy.position;

        currentHealth = maxHealth;
        lastSeenPosition = player.position;

        if (waypointsToStay[0])
        {
            waypointIndex++;
        }

        StartCoroutine(Move());
    }

    private void UpdatePath()
    {
        if (seeker.IsDone() && !dead)
        {
            seeker.StartPath(rb.position, player.position);
        }
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            if (aiPath.desiredVelocity.x >= 0.01f)
            {
                transform.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
                movingLeft = false;
            }
            else if (aiPath.desiredVelocity.x <= -0.01f)
            {
                transform.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
                movingLeft = true;
            }

            Vector3 playerPosistion = player.position;

            outOfZone = playerPosistion.x - (graph.center.x + graph.size.x / 2) >= 0.1f ||
                        playerPosistion.x - (graph.center.x - graph.size.x / 2) <= -0.1f ||
                        playerPosistion.y - (graph.center.y + graph.size.y / 2) >= 0.1f ||
                        playerPosistion.y - (graph.center.y - graph.size.y / 2) <= -0.1f;

            playerInRange = Vector2.Distance(rb.position, playerPosistion) <= aggroRange;
            playerIsVisible = PlayerInSight();

            if (returnedToStart)
            {
                aiPath.enabled = false;
                aggroRange = initAggroRange;
                CancelInvoke("CheckDist");
                CancelInvoke("UpdatePath");

                if (!moveCoroutineIsRunning)
                {
                    StartCoroutine(Move());
                }
            }

            if (playerIsVisible && (aggressive || wasHit))
            {
                aiPath.enabled = true;
                aggroed = true;
                InvokeRepeating("CheckDist", 0, 1f);
            }

            if (aggroed)
            {
                hasWaited = false;
                lastSeenPosition = (outOfZone) ? defaultPosition : playerPosistion;
            }
        }

        if (toxic && dead)
        {
            if (startToxicCloudFirstStage)
            {
                stage[0].localScale = Vector3.Lerp(stage[0].localScale, firstScale, appearSpeed * Time.fixedDeltaTime);
                stage[1].localScale = Vector3.Lerp(stage[1].localScale, secondScale, appearSpeed * Time.fixedDeltaTime);
                stage[2].localScale = Vector3.Lerp(stage[2].localScale, thirdScale, appearSpeed * Time.fixedDeltaTime);
            }

            if (startToxicCloudLastStage)
            {
                for (int i = 0; i < stages; i++)
                {
                    stageSpriteRenderer[i].color = Color.Lerp(stageSpriteRenderer[i].color,
                        new Color(stageSpriteRenderer[i].color.r, stageSpriteRenderer[i].color.g, stageSpriteRenderer[i].color.b, 0), 
                        disappearSpeed * Time.fixedDeltaTime);
                }

                if (stageSpriteRenderer[0].color.a <= disableColliderAlpha)
                {
                    toxicCloudCollider.enabled = false;

                    if (stageSpriteRenderer[0].color.a <= deleteToxicCloudAlpha)
                    {
                        deleteToxicCloud = true;
                    }
                }
            }
        }
    }

    private void CheckDist()
    {
        if (!dead)
        {
            if (aggroed)
            {
                waypointIndex = 0;
                aggroRange = initAggroRangeChasing;
                returnedToLastSeen = false;
                returnedToStart = false;
                InvokeRepeating("UpdatePath", 0, 0.1f);
            }
            else
            {
                if (Vector2.Distance(rb.position, lastSeenPosition) >= 0.3f && !returnedToLastSeen && checkLastSeenPosition)
                {
                    if (outOfZone || !playerInRange || !playerIsVisible)
                    { 
                        ReturnToLastSeen(); 
                    }
                }
                else if (Vector2.Distance(rb.position, lastSeenPosition) < 0.3f || !checkLastSeenPosition)
                {
                    if (checkLastSeenPosition && !isWaiting && !hasWaited && lastSeenPosition != defaultPosition)
                    {
                        StartCoroutine(WaitOnLastSeen());
                    }

                    returnedToLastSeen = true;
                    aggroed = false;
                }

                if (Vector2.Distance(rb.position, defaultPosition) >= 0.1f && returnedToLastSeen && !returnedToStart)
                {
                    if (outOfZone || !playerInRange || !playerIsVisible)
                    {
                        ReturnToStart();
                    }
                }
                else if (Vector2.Distance(rb.position, defaultPosition) < 0.1f)
                {
                    returnedToStart = true;
                    wasHit = false;
                    aggroed = false;
                    waypointIndex = 0;
                }
            }

            if (!playerInRange || outOfZone)
            {
                CancelInvoke("UpdatePath");
                aggroed = false;
            }
        }
    }

    private bool PlayerInSight()
    {
        platformOnPath = false;
        playerOnPath = false;

        Vector2 direction = (player.position - transform.position).normalized;

        //Player is behind/under/above enemy
        if (player.position.x - transform.position.x <= 0 && !movingLeft || player.position.x - transform.position.x >= 0 && movingLeft)
        {
            if (!aggroed)
            {
                Debug.DrawRay(transform.position, direction * aggroRange / 2, Color.red);
            }

            sightHits = Physics2D.RaycastAll(transform.position, direction, aggroRange / 2, raycastMask);
        }
        //Player is in front of enemy
        else if (player.position.x - transform.position.x > 0 && !movingLeft || player.position.x - transform.position.x < 0 && movingLeft)
        {
            if (!aggroed)
            {
                Debug.DrawRay(transform.position, direction * aggroRange, Color.red);
            }

            sightHits = Physics2D.RaycastAll(transform.position, direction, aggroRange, raycastMask);
        }

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

        return playerOnPath && !platformOnPath && !outOfZone;
    }

    private void ReturnToStart()
    {
        aggroed = false;
        CancelInvoke("CheckDist");
        aggroRange = initAggroRange;
        seeker.StartPath(rb.position, defaultPosition);
        InvokeRepeating("CheckDist", 0, 1f);
    }

    private void ReturnToLastSeen()
    {
        aggroed = false;
        CancelInvoke("CheckDist");

        if (outOfZone && lastSeenPosition == defaultPosition)
        {
            aggroRange = initAggroRange;
        }
        else if (lastSeenPosition != defaultPosition)
        {
            aggroRange = initAggroRangeReturning;
        }

        seeker.StartPath(rb.position, lastSeenPosition);
        InvokeRepeating("CheckDist", 0, 1f);
    }

    private IEnumerator WaitOnLastSeen()
    {
        isWaiting = true;
        aiPath.enabled = false;
        yield return new WaitForSeconds(waitOnLastSeenDuration);
        aiPath.enabled = true;
        isWaiting = false;
        hasWaited = true;
    }

    private IEnumerator Move()
    {
        moveCoroutineIsRunning = true;

        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, speed * Time.deltaTime);

        if (transform.position == waypoints[waypointIndex].position)
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

            if (deltaX > 0)
            {
                movingLeft = false;
                transform.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
            }
            else if (deltaX < 0)
            {
                movingLeft = true;
                transform.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
            }
        }

        moveCoroutineIsRunning = false;
    }

    public void ApplyDamage(float damage, string tag, bool instantKill)
    {
        if (Time.time > nextDamage && canTakeDamage)
        {
            nextDamage = Time.time + damageRate;
            currentHealth -= damage;
            StartCoroutine(DamageEffect());

            if (aggroAfterHit)
            {
                wasHit = true;
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
        yield return null;
        scr_EventSystem.instance.mobDeath.Invoke(mobID);

        if (!toxic)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            dead = true;
            toxicCloud.gameObject.SetActive(true);
            model.gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetComponent<CircleCollider2D>().enabled = false;
            seeker.enabled = false;
            yield return new WaitForSeconds(timeBeforeExplode);

            for (int i = 0; i < stages; i++)
            {
                stage[i].gameObject.SetActive(true);
            }

            startToxicCloudFirstStage = true;
            yield return new WaitForSeconds(timeBeforeDisappear);
            startToxicCloudLastStage = true;
            yield return new WaitUntil(() => deleteToxicCloud);
            Destroy(transform.parent.gameObject);
        }
    }
}
