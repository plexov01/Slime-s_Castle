using System.Collections.Generic;
using UnityEngine;

public class scr_Trap : MonoBehaviour
{
    public float damage;
    [SerializeField] List<CreatureType> whoCanBeDamaged = new List<CreatureType>();
    [SerializeField] private bool instantKill;

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
                if (instantKill)
                {
                    col.gameObject.GetComponent<scr_IDamageable>().ApplyDamage(instantKill: instantKill);
                }
                else
                {
                    col.gameObject.GetComponent<scr_IDamageable>().ApplyDamage(damage);
                }
                
                break;
            }
        }
    }

    enum CreatureType
    {
        Player,
        Enemy
    }

}
