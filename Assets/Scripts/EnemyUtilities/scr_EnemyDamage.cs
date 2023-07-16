using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyDamage : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D playerRigidbody;

    [Header("Knockback")]
    [SerializeField] private bool enableKnockback;
    [SerializeField] private float forceX = 2f;
    [SerializeField] private float forceY = 2.5f;
    private bool knockbackCoroutineIsRunning = false;
    [SerializeField] private float knockbackMaxTime;
    private bool knockbackTimerFinished = false;

    [Header("Damage")]
    [SerializeField] private float damage = 1f;
    [SerializeField] List<CreatureType> whoCanBeDamaged = new List<CreatureType>();

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
        playerRigidbody = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        TryDamage(col);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        TryDamage(col);
    }

    private void TryDamage(Collider2D col)
    {
        foreach (var type in whoCanBeDamaged)
        {
            if (col.CompareTag(type.ToString()))
            {
                if (!knockbackCoroutineIsRunning && enableKnockback && !scr_Player.invulnerable)
                {
                    StartCoroutine(Knockback());
                }

                if (col.gameObject.transform.parent != transform.parent)
                {
                    col.gameObject.GetComponent<scr_IDamageable>().ApplyDamage(damage);
                }

                break;
            }
        }
    }

    private enum CreatureType
    {
        Player,
        Enemy
    }

    private IEnumerator Knockback()
    {
        knockbackCoroutineIsRunning = true;

        if (!scr_PlungeAttack.movingDownAfterAttack)
        {
            scr_PlayerFormBase.inKnockback = true;

            if (player.position.x >= transform.position.x)
            {
                //rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
                playerRigidbody.velocity = new Vector2(forceX, forceY);
            }
            else
            {
                //rb.AddForce(new Vector2(-forceX, forceY), ForceMode2D.Impulse);
                playerRigidbody.velocity = new Vector2(-forceX, forceY);
            }

            knockbackTimerFinished = false;
            StartCoroutine(Wait());
            yield return new WaitUntil(() => !scr_PlungeAttack.isGrounded);
            yield return new WaitUntil(() => scr_PlungeAttack.isGrounded || knockbackTimerFinished);
            StopCoroutine(Wait());
            scr_PlayerFormBase.inKnockback = false;
        }
        
        knockbackCoroutineIsRunning = false;
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(knockbackMaxTime);
        knockbackTimerFinished = true;
    }
}
