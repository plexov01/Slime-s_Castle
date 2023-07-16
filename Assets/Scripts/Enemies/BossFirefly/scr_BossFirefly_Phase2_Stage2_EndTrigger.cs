using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_Phase2_Stage2_EndTrigger : MonoBehaviour
{
    public bool enteredTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enteredTrigger = true;
        }
    }
}
