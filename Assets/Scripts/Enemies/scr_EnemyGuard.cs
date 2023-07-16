using UnityEngine;
using System.Collections;

public class scr_EnemyGuard : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField][Range(0, 50f)] private float speed;
    [SerializeField][Range(0, 30f)] private float patrolDistance;
    [SerializeField] private Transform endOfPlatformChecker;
    [SerializeField] private LayerMask groundLayer;
    private float checkerRadius = 0.1f;
    private bool closeToEndOfPlatform;
    private Vector3 startPosition;
    private Vector2 velocityVector;
    private float leftEgde;
    private float rightEgde;
    private bool movingRight;
    private bool flipCoroutineIsRunning = false;

    [Header("Sight")]
    [SerializeField] private LayerMask raycastMask;
    private RaycastHit2D[] sightHits;
    private bool platformOnPath;
    private bool playerOnPath;
    private bool playerIsGrounded;

    [Header("Attack")]
    [SerializeField] private bool aggressive;
    [SerializeField][Range(0, 30f)] private float attackDistance;
    private float initAttackDistance;

    [Header("Status")]   
    [SerializeField] private bool patrol;
    [SerializeField] private bool attack;
    [SerializeField] private bool goBack;
    //[SerializeField] private bool immobilized;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
        startPosition = transform.position;
        initAttackDistance = attackDistance;
        leftEgde = startPosition.x - patrolDistance;
        rightEgde = startPosition.x + patrolDistance;
    }

    private void FixedUpdate() 
    {
        closeToEndOfPlatform = !Physics2D.OverlapCircle(endOfPlatformChecker.position, checkerRadius, groundLayer);
        playerIsGrounded = scr_PlungeAttack.isGrounded;
        Vector3 playerPosition = player.position;

        if (movingRight)
        {
            transform.localScale = new Vector2(1, 1);
            velocityVector = new Vector2(speed, 0);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
            velocityVector = new Vector2(-speed, 0);
        }

        if (!closeToEndOfPlatform)
        {
            rb.velocity = velocityVector;
        }
        else if (!flipCoroutineIsRunning)
        {
            StartCoroutine(Flip());
        }

        if (Mathf.Abs(transform.position.x - startPosition.x) < patrolDistance + 0.1f && !attack)
        {
            patrol = true;
            goBack = false;
        }

        if (aggressive)
        {
            if (Mathf.Abs(transform.position.x - playerPosition.x) < attackDistance - 0.1f 
                && !(playerPosition.y - transform.position.y > 0.2f && playerIsGrounded)
                && playerPosition.y - transform.position.y <= 1.25f
                && playerPosition.y - transform.position.y >= -0.2f
                && PlayerInSight())
            {
                attack = true;
                patrol = false;
                goBack = false;
            }

            if (Mathf.Abs(transform.position.x - playerPosition.x) > attackDistance && attack)
            {
                goBack = true;
                attack = false;
            }
        }

        if (patrol)
        {
            Patrol();
        }
        else if (attack)
        {
            Attack(playerPosition);
        }
        else if (goBack)
        {
            GoBack();
        }
    }

    private IEnumerator Flip()
    {
        flipCoroutineIsRunning = true;
        attackDistance = 0;
        rb.velocity = Vector2.zero;
        movingRight = !movingRight;
        goBack = true;
        attack = false;
        patrol = false;
        yield return new WaitForSeconds(1.5f);
        attackDistance = initAttackDistance;
        flipCoroutineIsRunning = false;
    }


    private void Attack(Vector3 playerPosition)
    {
        if (playerPosition.y - transform.position.y > 1.25f || playerPosition.y - transform.position.y < -0.2f
            || (playerPosition.y - transform.position.y > 0.2f || !PlayerInSight()) && playerIsGrounded)
        {
            goBack = true;
            attack = false;
        }

        if (playerPosition.x - transform.position.x > 0.4f && playerPosition.y - transform.position.y <= 1.25f
            && playerPosition.y - transform.position.y >= -0.2f)
        {
            movingRight = true;
        }
        else if (playerPosition.x - transform.position.x < -0.4f)
        {
            movingRight = false;
        }
    }

    private void Patrol()
    {
        if (transform.position.x - leftEgde <= 0.01f)
        {
            movingRight = true;
        }
        else if (transform.position.x - rightEgde >= -0.01f)
        {
            movingRight = false;
        }
    }

    private void GoBack()
    {
        if (transform.position.x - rightEgde > 0)
        {
            movingRight = false;
        }
        else if (transform.position.x - leftEgde < 0)
        {
            movingRight = true;
        }
    }

    //public void Immobilize()
    //{
    //
    //}

    private bool PlayerInSight()
    {
        platformOnPath = false;

        Vector2 direction = (player.position - transform.position).normalized;

        Debug.DrawRay(transform.position, direction * attackDistance, Color.red);

        sightHits = Physics2D.RaycastAll(transform.position, direction, attackDistance, raycastMask);

        for (int x = 0; x < sightHits.Length; x++)
        {
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

    private void OnDrawGizmos() 
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawWireCube(transform.parent.position, new Vector3(patrolDistance * 2, 0.5f, 0));
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireCube(transform.position, new Vector3(attackDistance * 2, 0.5f, 0));
    }

}
