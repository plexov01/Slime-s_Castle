using UnityEngine;

public class jumpPlatform : MonoBehaviour
{
    [SerializeField] [Range(0, 30f)] private float force;
    [SerializeField] private bool launchOnJump;
    private Animator anim;
    private bool playerIsGrounded;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        playerIsGrounded = scr_PlungeAttack.isGrounded;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!scr_PlungeAttack.movingDownAfterAttack)
            {
                if (launchOnJump && !playerIsGrounded)
                {
                    Launch(collision);
                }
                else if (!launchOnJump)
                {
                    Launch(collision);
                }
            }
        }
        else if (collision.CompareTag("MovableCube"))
        {
            Launch(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("MovableCube"))
        {
            if (anim.GetBool("Active"))
            {
                anim.SetBool("Active", false);
            }
        }
    }

    private void Launch(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(rb.velocity.x, force);
        anim.SetBool("Active", true);
    }
}
