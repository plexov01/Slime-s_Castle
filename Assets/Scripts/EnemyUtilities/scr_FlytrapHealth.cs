using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_FlytrapHealth : MonoBehaviour, scr_IDamageable
{
    [SerializeField] private GameObject enemy;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;
    public bool canTakeDamageFromNormalAttack = false;
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] [Range(0, 10f)] private float damageRate;
    private float nextDamage;

    public int mobID;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(float damage, string tag, bool instantKill)
    {
        if (Time.time > nextDamage && canTakeDamage)
        {
            if (tag == "SlimeAttack" && canTakeDamageFromNormalAttack)
            {
                nextDamage = Time.time + damageRate;
                currentHealth -= damage;
            }
            else if (tag == "PlungeAttack")
            {
                nextDamage = Time.time + damageRate;
                currentHealth -= damage;
            }

            if (currentHealth <= 0)
            {
                canTakeDamage = false;
                StartCoroutine(Die());
            }
        }
    }

    public IEnumerator Die()
    {
        yield return null;
        scr_EventSystem.instance.mobDeath.Invoke(mobID);
        Destroy(enemy);
    }
}
