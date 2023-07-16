using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

public class scr_BossFirefly_Phase1_DeathState : State
{
    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform headBone;
    [SerializeField] private Transform bodyBone;
    [SerializeField] private float rotationSpeed;
    private Quaternion angle = Quaternion.Euler(0, 0, -135);

    [Header("Objects")]
    [SerializeField] private GameObject roomCameraTrigger;
    [SerializeField] private GameObject PlayerLaserReceiver; 

    [Header("Break floor")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int tileStartX;
    [SerializeField] private int tileEndX;
    [SerializeField] private int tileY;
    public bool playerInTrigger = false;

    [Header("Next Phase")]
    [SerializeField] private Vector2 nextPhasePosition;
    private Transform player;
    private bool loadNextPhase = false;

    private void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        player = scr_GameManager.instance.player.transform;
    }

    public override void Enter()
    {
        scr_BossFirefly_Phase1_AttackState.laserScript.isOn = false;
        scr_BossFirefly_Phase1.anim.SetBool("flying", false);
        enemy.GetComponent<Rigidbody2D>().gravityScale = 0.5f;
        enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(1.5f, 0), ForceMode2D.Impulse);
        headBone.GetComponent<PolygonCollider2D>().enabled = false;
        bodyBone.GetComponent<PolygonCollider2D>().enabled = false;
        Destroy(PlayerLaserReceiver);
        StartCoroutine(Death());
    }

    public override State RunCurrentStateLogic()
    {
        return this;
    }

    public override void RunCurrentStatePhysics()
    {
        enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, angle, rotationSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider2D>().enabled = true;
        roomCameraTrigger.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (scr_PlungeAttack.movingDownAfterAttack)
            { 
                playerInTrigger = true; 
            }

            if (playerInTrigger && scr_PlungeAttack.isGrounded)
            {
                for (int i = tileStartX; i <= tileEndX; i++)
                {
                    tilemap.SetTile(new Vector3Int(i, tileY, 0), null);
                }

                if (!loadNextPhase)
                {
                    StartCoroutine(LoadNextPhase());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInTrigger = false;
    }

    private IEnumerator LoadNextPhase()
    {
        loadNextPhase = true;
        var noisePerlin = scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noisePerlin.m_AmplitudeGain = 2;
        noisePerlin.m_FrequencyGain = 2;
        yield return new WaitForSeconds(1f);
        MenuController.instance.ShowBlackScreen();
        scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5.5f;
        player.position = nextPhasePosition;
    }
}
