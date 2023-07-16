using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_SpiderDamage : MonoBehaviour
{
    public float damage;
    [SerializeField] private scr_EnemySpider enemySpider;
    [SerializeField] List<CreatureType> whoCanBeDamaged = new List<CreatureType>();

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
                col.gameObject.GetComponent<scr_IDamageable>().ApplyDamage(damage);
                enemySpider.playerDamaged = true;
                StartCoroutine(Wait());
                break;
            }
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.05f);
        enemySpider.playerDamaged = false;
    }

    enum CreatureType
    {
        Player,
        Enemy
    }
}
