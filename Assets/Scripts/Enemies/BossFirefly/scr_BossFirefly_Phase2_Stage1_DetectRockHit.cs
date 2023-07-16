using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_Phase2_Stage1_DetectRockHit : MonoBehaviour
{
    [SerializeField] private float rockHitCooldown;
    private float rockHitTimer;

    private void FixedUpdate()
    {
        rockHitTimer += Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock") && rockHitTimer >= rockHitCooldown)
        {
            rockHitTimer = 0;
            collision.GetComponent<SpriteRenderer>().enabled = false;
            scr_BossFirefly_Phase2_Stage1.rockHits++;
            print("rock hits: " + scr_BossFirefly_Phase2_Stage1.rockHits);
        }
    }
}
