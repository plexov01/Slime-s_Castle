using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class scr_BossFirefly_Phase1_IdleState : State
{
    [Header("Next State")]
    public scr_BossFirefly_Phase1_AttackState attackState;

    [Header("Phase start")]
    [SerializeField] private float firstTryDuration;
    public bool firstTry = true;
    public bool enterAttackState = false;
    public bool startPhase = false;
    [SerializeField] private GameObject leftStartWall;
    [SerializeField] private GameObject rightStartWall;
    [SerializeField] private Transform PlayerLaserReceiver;
    private Transform player;
    scr_SaveController SaveController;
    [SerializeField] private Transform enemyStartPosition;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform roomCenter;
    [SerializeField] private GameObject bossCameraTrigger;
    [SerializeField] private GameObject roomCameraTrigger;

    [Header("Follow Bezier Curve")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform route;
    private float tParam = 0;
    private Vector2 objectPosition;
    [SerializeField] private float wakeUpSpeed;

    private void Awake()
    {
        bossCameraTrigger.SetActive(false);
        roomCameraTrigger.SetActive(false);
    }

    private void Start()
    {
        SaveController = scr_SaveController.instance;
        player = scr_GameManager.instance.player.transform;
        virtualCamera = scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>();

        firstTry = SaveController.GetSaveGame(scr_GameManager.instance.currentSaveGame.numberOfSave).bossFireflyPhase1_firstTry;

        if (firstTry)
        {
            leftStartWall.SetActive(true);
            rightStartWall.SetActive(true);
        }
    }

    public override State RunCurrentStateLogic()
    {
        if (startPhase)
        {
            StartCoroutine(PhaseStart());
        }

        if (enterAttackState)
        {
            return attackState;
        }
            
        return this;
    }

    public override void Exit()
    {
        enterAttackState = false;
    }

    public IEnumerator PhaseStart()
    {
        startPhase = false;
        //scr_GameManager.instance.transform.GetChild(4).GetChild(0).GetChild(2).GetChild(0).gameObject.SetActive(false);
        scr_MiniMapController.instance.showMiniMap = false;
        PlayerLaserReceiver.position = player.position;
        PlayerLaserReceiver.parent = player;

        if (firstTry)
        {
            firstTry = false;
            bossCameraTrigger.SetActive(true);
            print("First try; wake up animation");
            yield return new WaitForSeconds(firstTryDuration);
            virtualCamera.m_Lens.OrthographicSize = 5.5f;
            scr_BossFirefly_Phase1.anim.SetBool("flying", true);
            
            Vector2 p0 = route.GetChild(0).position;
            Vector2 p1 = route.GetChild(1).position;
            Vector2 p2 = route.GetChild(2).position;
            Vector2 p3 = route.GetChild(3).position;

            while (tParam < 1)
            {
                tParam += Time.deltaTime * wakeUpSpeed;

                objectPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                    3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                    3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                    Mathf.Pow(tParam, 3) * p3;

                enemy.position = objectPosition;
                yield return new WaitForFixedUpdate();
            }

            leftStartWall.SetActive(false);
            rightStartWall.SetActive(false);
        }
        else
        {
            print("Not first try; no wake up animation");
            virtualCamera.m_Lens.OrthographicSize = 5.5f;
            scr_BossFirefly_Phase1.anim.SetBool("flying", true);
            enemy.position = enemyStartPosition.position;
        }

        roomCameraTrigger.SetActive(true);
        enterAttackState = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            startPhase = true;
        }
    }
}
