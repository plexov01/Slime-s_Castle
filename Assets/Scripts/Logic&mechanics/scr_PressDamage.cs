using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PressDamage : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<scr_IDamageable>().ApplyDamage(instantKill: true);
        }
    }
}
