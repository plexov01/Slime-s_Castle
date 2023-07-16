using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_Phase2_Stage1_RemoveRock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock"))
        {
            Destroy(collision.gameObject);
            scr_BossFirefly_Phase2_Stage1.rocks.RemoveAt(0);
        }
    }
}
