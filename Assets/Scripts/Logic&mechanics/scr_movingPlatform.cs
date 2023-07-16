using System.Collections;
using UnityEngine;

public class scr_MovingPlatform : MonoBehaviour
{
    [Header("Platform")]
    [SerializeField][Range(0, 30f)] private float speed;
    [Header("Horizontal")]
    [SerializeField] private bool movingHorizontal;
    [SerializeField][Range(0, 30f)] private float pathLengthHorizontal;
    [Header("Vertical")]
    [SerializeField] private bool movingVertical;
    [SerializeField][Range(0, 30f)] private float pathLengthVertical;
    [SerializeField] private bool waitForPlayer;
    private bool playerWasOnPlatform;
    public bool canMove;
    public bool staying;
    public bool firstMovement;
    private float topEdge;
    private float bottomEdge;

    private Vector2 startPosition;
    private float widthPlatform;
    private float heightPlatform;
    
    [Header("Для настройки изначального направления движения платформы")]
    [SerializeField] private bool movingRight;
    [SerializeField] private bool movingUp;

    private Transform player;

    private void Awake() 
    {
        widthPlatform = GetComponent<RectTransform>().sizeDelta.x;
        heightPlatform = GetComponent<RectTransform>().sizeDelta.y;

        startPosition = transform.parent.position;
        topEdge = startPosition.y + pathLengthVertical / 2 - heightPlatform / 2;
        bottomEdge = startPosition.y - pathLengthVertical / 2 + heightPlatform / 2;

        if (movingVertical)
        {
            if (transform.position.y > topEdge)
            {
                transform.position = new Vector3(transform.position.x, topEdge, transform.position.z);
            }
            else if (transform.position.y < bottomEdge)
            {
                transform.position = new Vector3(transform.position.x, bottomEdge, transform.position.z);
            }
        }
    }

    void Start()
    {
        player = scr_GameManager.instance.player.transform;
        GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().size.x, heightPlatform);
        BoxCollider2D trigger = transform.GetChild(0).GetComponent<BoxCollider2D>();
        trigger.size = new Vector2(GetComponent<SpriteRenderer>().size.x, 2 * heightPlatform);
        trigger.offset = new Vector2(0, heightPlatform / 2);
        firstMovement = true;
    }

    private void FixedUpdate()
    {
        if (movingHorizontal)
        {
            if (transform.position.x <= startPosition.x - pathLengthHorizontal / 2 + widthPlatform / 2)
            {
                movingRight = true;
            }
            else if (transform.position.x >= startPosition.x + pathLengthHorizontal / 2 - widthPlatform / 2)
            {
                movingRight = false;
            }

            if (movingRight)
            {
                transform.position += new Vector3(speed * Time.fixedDeltaTime, 0, 0);
            }
            else
            {
                transform.position -= new Vector3(speed * Time.fixedDeltaTime, 0, 0);
            }

        }

        if (movingVertical)
        {
            if (!staying)
            {
                if (movingUp)
                {
                    transform.position += new Vector3(0, speed * Time.fixedDeltaTime, 0);
                }
                else
                {
                    transform.position -= new Vector3(0, speed * Time.fixedDeltaTime, 0);
                }
            }

            if (waitForPlayer)
            {
                if (player.parent == transform)
                {
                    canMove = true;
                }
                else if (player.parent != transform && transform.position.y >= topEdge && player.position.y > topEdge)
                {
                    canMove = false;
                }

                if (player.position.y < transform.position.y || transform.position.y >= topEdge)
                {
                    canMove = false;
                    playerWasOnPlatform = false;
                }
            }
            else if (!waitForPlayer)
            {
                canMove = true;
            }

            if (canMove)
            {
                firstMovement = false;

                if (waitForPlayer)
                {
                    if (transform.position.y <= bottomEdge || player.parent == transform
                        || player.parent != transform && player.position.y > transform.position.y && transform.position.y < topEdge)
                    {
                        staying = false;
                        movingUp = true;
                    }
                    else if (transform.position.y >= topEdge)
                    {
                        staying = true;
                    }
                }
                else
                {
                    if (transform.position.y <= bottomEdge)
                    {
                        staying = false;
                        movingUp = true;
                    }
                    else if (transform.position.y >= topEdge)
                    {
                        staying = false;
                        movingUp = false;
                    }
                }
            }
            else
            {
                if (firstMovement)
                {
                    staying = false;
                    movingUp = false;

                    if (transform.position.y < bottomEdge)
                    {
                        staying = true;
                        firstMovement = false;
                    }
                }
                else 
                {
                    if (player.position.y >= topEdge)
                    {
                        if (transform.position.y <= topEdge && playerWasOnPlatform)
                        {
                            staying = false;
                            movingUp = true;
                        }
                        else if (transform.position.y > topEdge)
                        {
                            staying = true;
                        }
                    }
                    if (player.position.y < transform.position.y && transform.position.y >= bottomEdge)
                    {
                        staying = false;
                        movingUp = false;
                    }
                    else if (transform.position.y < bottomEdge)
                    {
                        staying = true;
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && player.position.y > (transform.position.y + heightPlatform / 2))
        {
            collision.transform.SetParent(transform);
            playerWasOnPlatform = true;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawWireCube(transform.parent.position, new Vector3(pathLengthHorizontal, 0.5f, 0));
        Gizmos.DrawWireCube(transform.parent.position, new Vector3(0.5f, pathLengthVertical, 0));
    }
}