using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PressMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float downSpeed;
    [SerializeField] private float upSpeed;
    public bool canKillPlayer = false;
    private float topEdge;
    private float bottomEdge;
    public bool movingUp = false;

    [Header("Checker")]
    [SerializeField] private Transform checker;
    [SerializeField] private LayerMask groundLayer;

    private BoxCollider2D physicsCollider;
    private BoxCollider2D triggerCollider;

    private float playerColliderDiameter;

    private void Awake()
    {
        physicsCollider = GetComponent<BoxCollider2D>();
        triggerCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        playerColliderDiameter = scr_GameManager.instance.player.transform.GetComponent<CircleCollider2D>().radius * 2f;

        physicsCollider.size = GetComponent<SpriteRenderer>().size;
        triggerCollider.size = new Vector2(GetComponent<SpriteRenderer>().size.x * 0.99f, playerColliderDiameter);
        triggerCollider.offset = new Vector2(0, -(GetComponent<SpriteRenderer>().size.y + playerColliderDiameter) / 2);
        triggerCollider.enabled = false;
        checker.localPosition = new Vector2(0, -(physicsCollider.size.y / 2 + 0.25f));

        //RaycastHit2D upHit = Physics2D.Raycast(checker.position, Vector2.up, 10f, groundLayer);
        //RaycastHit2D downHit = Physics2D.Raycast(checker.position, Vector2.down, 10f, groundLayer);

        topEdge = checker.position.y + Physics2D.Raycast(checker.position, Vector2.up, 10f, groundLayer).distance;
        bottomEdge = checker.position.y - Physics2D.Raycast(checker.position, Vector2.down, 10f, groundLayer).distance;
    }

    private void FixedUpdate()
    {
        if (movingUp && transform.position.y - physicsCollider.bounds.extents.y <= topEdge)
        {
            transform.Translate(upSpeed * Time.fixedDeltaTime * Vector3.up, Space.World);
        }
        else if (!movingUp && transform.position.y - physicsCollider.bounds.extents.y >= bottomEdge)
        {
            transform.Translate(downSpeed * Time.fixedDeltaTime * Vector3.down, Space.World);
        }

        if (Mathf.Abs(transform.position.y - physicsCollider.bounds.extents.y - topEdge) <= 0.01f
            || transform.position.y - physicsCollider.bounds.extents.y > topEdge)
        {
            movingUp = false;
        }
        else if (Mathf.Abs(transform.position.y - physicsCollider.bounds.extents.y - bottomEdge) <= 0.01f
            || transform.position.y - physicsCollider.bounds.extents.y < bottomEdge)
        {
            movingUp = true;
        }

        RaycastHit2D hit = Physics2D.Raycast(physicsCollider.bounds.center - new Vector3(0, physicsCollider.bounds.extents.y, 0), 
            Vector2.down, playerColliderDiameter, groundLayer);

        if (Mathf.Abs(hit.point.y - bottomEdge) <= playerColliderDiameter && !movingUp)
        {
            triggerCollider.enabled = true;
        }
        else
        {
            triggerCollider.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(GetComponent<SpriteRenderer>().size.x, GetComponent<SpriteRenderer>().size.y, 0));
    }
}
