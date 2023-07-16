using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyShootingSpider : MonoBehaviour
{
    [SerializeField] private Vector2 shootAreaSize;
    [SerializeField] private Vector2 shootAreaOffset;
    [SerializeField] private float shootCooldown;
    private float shootTimer;
    [SerializeField] private float shootForce;

    [SerializeField] private GameObject webProjectile;
    [SerializeField] private Transform model;
    [SerializeField] private Transform firePoint;

    private Transform player;
    private bool playerInArea;

    [Header("Sight")]
    [SerializeField] private LayerMask raycastMask;
    private RaycastHit2D[] sightHits;
    private bool platformOnPath;
    private bool playerOnPath;

    void Start()
    {
        player = scr_GameManager.instance.player.transform;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = shootAreaSize;
        boxCollider.offset = shootAreaOffset;
    }

    void Update()
    {
        if (shootTimer < shootCooldown)
        {
            shootTimer += Time.deltaTime;
        }

        if (playerInArea)
        {
            Vector3 raycastStart = model.position;
            float distance = Vector2.Distance(player.position, raycastStart);

            Debug.DrawRay(raycastStart, (player.position - raycastStart).normalized * distance, Color.red);
        }
    }

    private void FixedUpdate()
    {
        if (playerInArea)
        {
            Vector3 relativePos = player.position - model.position;
            model.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), -relativePos);

            if (shootTimer >= shootCooldown && PlayerInSight())
            {
                shootTimer = 0f;
                firePoint.position = model.position - model.up * 0.25f;
                GameObject newWebProjectile = Instantiate(webProjectile, firePoint.position, Quaternion.Euler(0, 0, 0));
                newWebProjectile.transform.parent = transform.parent;
                Rigidbody2D rb = newWebProjectile.GetComponent<Rigidbody2D>();
                rb.AddForce(-model.up * shootForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            model.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private bool PlayerInSight()
    {
        platformOnPath = false;
        playerOnPath = false;

        Vector3 raycastStart = model.position;
        float distance = Vector2.Distance(player.position, raycastStart);

        sightHits = Physics2D.RaycastAll(raycastStart, (player.position - raycastStart).normalized, distance, raycastMask);

        for (int i = 0; i < sightHits.Length; i++)
        {
            if (sightHits[i].transform.gameObject.layer == LayerMask.NameToLayer("Platforms"))
            {
                platformOnPath = true;
                break;
            }

            if (sightHits[i].transform.CompareTag("Player"))
            {
                playerOnPath = true;
                break;
            }
        }

        return playerOnPath && !platformOnPath;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInArea = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.parent.position + shootAreaOffset, shootAreaSize);
    }
}
