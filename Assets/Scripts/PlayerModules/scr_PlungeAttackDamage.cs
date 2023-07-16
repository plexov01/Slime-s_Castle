using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PlungeAttackDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            collider.gameObject.GetComponent<scr_IDamageable>().ApplyDamage(scr_PlungeAttack.damage, gameObject.transform.tag);
        }
    }
}
