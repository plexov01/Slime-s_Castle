using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyHealth : MonoBehaviour, scr_IDamageable
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject enemy;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] [Range(0, 10f)] private float damageRate;
    private float nextDamage;

    public int mobID;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(float damage, string tag, bool instantKill)
    {
        if (Time.time > nextDamage && canTakeDamage)
        {
            nextDamage = Time.time + damageRate;
            currentHealth -= damage;
            StartCoroutine(DamageEffect());

            if (currentHealth <= 0)
            {
                canTakeDamage = false;
                StartCoroutine(Die());
            }
        }
    }

    private IEnumerator DamageEffect()
    {
        spriteRenderer.color = new Color(1, 0, 0, 0.75f);
        yield return new WaitForSeconds(damageRate / 2);
        spriteRenderer.color = Color.white;
    }

    public IEnumerator Die()
    {
        yield return null;
        scr_EventSystem.instance.mobDeath.Invoke(mobID);
        Destroy(enemy);
    }
}

