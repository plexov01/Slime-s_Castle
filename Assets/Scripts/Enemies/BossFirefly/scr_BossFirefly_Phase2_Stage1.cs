using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

public class scr_BossFirefly_Phase2_Stage1 : State
{
    [Header("Next State")]
    public scr_BossFirefly_Phase2_Stage2 stage2;
    private bool changeStage = false;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public static bool changeCamera = false;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform enemyModel;
    [SerializeField] private GameObject enemyCollider;
    [SerializeField] private Animator anim;
    private Vector2 enemyStartPosition;
    public static int rockHits;
    [SerializeField] private int rockHitsNeeded;

    [Header("Background")]
    [SerializeField] private ScrollingBackground scrollingBG;

    [Header("Follow player")]
    [SerializeField] private float oldPosition;
    [SerializeField] private float updatePosDelay;
    [SerializeField] private float followSpeed;
    private Transform player;

    [Header("Rocks")]
    [SerializeField] private GameObject rock;
    public static List<GameObject> rocks = new List<GameObject>();
    [SerializeField] private float spawnTime;
    private float spawnTimer;
    [SerializeField] private float gravityTime;
    //private float gravityTimer;
    [SerializeField] private float gravity;
    [SerializeField] private float velocity;
    [SerializeField] private float leftEdge;
    [SerializeField] private float rightEdge;
    [SerializeField] private Transform ceiling;

    [Header("Move Objects")]
    [SerializeField] private Transform invisibleFloor;
    [SerializeField] private Transform platformFloor;
    private Transform platformToFall;
    private bool moveObjects = false;
    [SerializeField] private Transform stage2Center;
    [SerializeField] private Transform leftWall;
    private Vector2 leftWallStart;
    [SerializeField] private Transform rightWall;
    private Vector2 rightWallStart;

    [Header("Follow Bezier Curve")]
    [SerializeField] private Transform[] routes;
    private int routeToGo;
    private float tParam = 0;
    private Vector2 objectPosition;
    [SerializeField] private float startSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float deadSpeed;
    private bool coroutineGoByTheRouteAllowed = true;

    [Header("Reload")]
    [SerializeField] private Transform playerRespawn;
    private bool reloading = false;
    private bool spawnRocks = false;
    private bool firstTry = true;
    scr_SaveController SaveController;

    [SerializeField] private BoxCollider2D stage2Save;

    void Start()
    {
        player = scr_GameManager.instance.player.transform;
        //scr_GameManager.instance.transform.GetChild(4).GetChild(0).GetChild(2).GetChild(0).gameObject.SetActive(false);
        SaveController = scr_SaveController.instance;
        //enemy.gameObject.SetActive(false);
        enemyStartPosition = enemy.position;
        leftWallStart = leftWall.position;
        rightWallStart = rightWall.position;

        firstTry = SaveController.GetSaveGame(scr_GameManager.instance.currentSaveGame.numberOfSave).bossFireflyPhase2_stage1_firstTry;

        if (firstTry)
        {
            enemy.SetPositionAndRotation(routes[2].GetChild(0).position, Quaternion.Euler(0, 0, -135));
            StartCoroutine(GoByTheStartRoute());
        }
        else
        {
            StartCoroutine(UpdatePosition());
        }

        StartCoroutine(ChangeStage());
    }
    public override void Enter()
    {
        scr_BossFirefly_Phase2_ChooseStage.currentStage = "Stage1";
        spawnRocks = true;

        if (!firstTry)
        {
            scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5.5f;
            anim.SetBool("flying", true);
            GetComponent<BoxCollider2D>().enabled = false;
            scrollingBG.scroolBG = true;
        }
    }

    public override State RunCurrentStateLogic()
    {
        if (rockHits >= rockHitsNeeded)
        {
            rockHits = 0;
            spawnRocks = false;

            for (int i = 0; i < rocks.Count; i++)
            {
                if (rocks[i].transform.position.y - player.position.y <= -4)
                {
                    rocks[i].SetActive(false);
                }
            }

            enemyCollider.SetActive(false);

            anim.SetBool("flying", false);

            print("enemy position: " + enemy.position);
            routes[0].GetChild(0).position = new Vector2(enemy.position.x, routes[0].GetChild(0).position.y);
            routes[1].GetChild(0).position = new Vector2(enemy.position.x, routes[1].GetChild(0).position.y);

            if (enemy.position.x >= enemyStartPosition.x)
            {
                routeToGo = 0;
                platformToFall = platformFloor.GetChild(2);
            }
            else
            {
                routeToGo = 1;
                platformToFall = platformFloor.GetChild(0);
            }

            moveObjects = true;
            StopCoroutine(UpdatePosition());
        }

        if (changeStage)
        {
            return stage2;
        }

        if (scr_Player.playerDied && !reloading)
        {
            StartCoroutine(Reload());
        }

        return this;
    }

    public override void RunCurrentStatePhysics()
    {
        virtualCamera.ForceCameraPosition(new Vector3(virtualCamera.transform.position.x, player.position.y + 1.5f, virtualCamera.transform.position.z), new Quaternion(0, 0, 0, 0));

        if (spawnRocks)
        {
            enemy.position = Vector2.MoveTowards(enemy.position, new Vector2(oldPosition, enemy.position.y), followSpeed * Time.fixedDeltaTime);

            spawnTimer += Time.fixedDeltaTime;
            //gravityTimer += Time.fixedDeltaTime;

            if (spawnTimer >= spawnTime)
            {
                spawnTimer = 0;

                float rockPositionX = Random.Range(leftEdge, rightEdge);

                GameObject newRock = Instantiate(rock);
                newRock.transform.SetParent(rock.transform.parent);
                Rigidbody2D rb = newRock.GetComponent<Rigidbody2D>();
                newRock.transform.position = new Vector2(rock.transform.position.x + rockPositionX, rock.transform.position.y);
                newRock.SetActive(true);
                rocks.Add(newRock);
                //rb.gravityScale = gravity;
                rb.AddForce(new Vector2(0, velocity), ForceMode2D.Impulse);

                //if (gravityTimer >= gravityTime)
                //{
                //    gravityTime = 0;
                //    rb.gravityScale = 0;
                //}
            }
        }

        if (moveObjects && platformFloor.position.y < invisibleFloor.position.y)
        {
            platformFloor.position = Vector2.MoveTowards(platformFloor.position, new Vector2(platformFloor.position.x, invisibleFloor.position.y), followSpeed * Time.fixedDeltaTime);

            if (coroutineGoByTheRouteAllowed)
            {
                StartCoroutine(GoByTheRoute(routeToGo));
            }
        }
    }

    public override void Exit()
    {
        spawnRocks = false;
        moveObjects = false;
        scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 4.5f;
        StopAllCoroutines();
    }

    private IEnumerator UpdatePosition()
    {
        while (true)
        {
            oldPosition = player.position.x;

            if (enemy.position.x - oldPosition < -0.02f)
            {
                enemy.localScale = new Vector3(-1, 1, 1);
            }
            else if (enemy.position.x - oldPosition > 0.02f)
            {
                enemy.localScale = new Vector3(1, 1, 1);
            }

            yield return new WaitForSeconds(updatePosDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spawnRocks = true;
            anim.SetBool("flying", true);
            GetComponent<BoxCollider2D>().enabled = false;
            scrollingBG.scroolBG = true;
        }
    }

    private IEnumerator Reload()
    {
        print("reloading stage1");
        reloading = true;
        spawnRocks = false;
        moveObjects = false;
        StopCoroutine(UpdatePosition());
        enemy.position = enemyStartPosition;
        enemyCollider.SetActive(true);
        anim.SetBool("flying", true);

        leftWall.position = leftWallStart;
        rightWall.position = rightWallStart;

        for (int i = rocks.Count - 1; i >= 0; i--)
        {
            Destroy(rocks[i]);
        }

        rocks.Clear();
        rockHits = 0;
        yield return new WaitForSeconds(1f);
        spawnRocks = true;
        StartCoroutine(UpdatePosition());
        reloading = false;
    }

    private IEnumerator ChangeStage()
    {
        yield return new WaitUntil(() => platformFloor.position.y >= invisibleFloor.position.y);
        scrollingBG.scroolBG = false;
        invisibleFloor.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        platformToFall.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.5f);
        Rigidbody2D rb = platformToFall.gameObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0.5f;
        enemy.GetComponent<Rigidbody2D>().gravityScale = 0.5f;

        yield return new WaitForSeconds(1.25f);
        MenuController.instance.ShowBlackScreen();
        stage2Save.enabled = false;
        player.position = new Vector2(stage2Center.position.x + 1, stage2Center.position.y);

        for (int i = rocks.Count - 1; i >= 0; i--)
        {
            if (!rocks[i].activeInHierarchy)
            {
                Destroy(rocks[i]);
            }
        }

        rocks.Clear();

        yield return new WaitForSeconds(0.5f);
        MenuController.instance.HideBlackScreen();
        changeStage = true;
    }

    private IEnumerator GoByTheRoute(int routeNumber)
    {
        coroutineGoByTheRouteAllowed = false;

        Vector2 p0 = routes[routeNumber].GetChild(0).position;
        Vector2 p1 = routes[routeNumber].GetChild(1).position;
        Vector2 p2 = routes[routeNumber].GetChild(2).position;
        Vector2 p3 = routes[routeNumber].GetChild(3).position;

        Vector2 leftWallTarget = new Vector2(leftWall.position.x + 5f, leftWall.position.y);
        Vector2 rightWallTarget = new Vector2(rightWall.position.x - 5f, rightWall.position.y);

        while (tParam < 1 && !reloading)
        {
            tParam += Time.deltaTime * deadSpeed;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;

            enemy.position = objectPosition;
            leftWall.position = Vector2.MoveTowards(leftWall.position, leftWallTarget, Time.fixedDeltaTime);
            rightWall.position = Vector2.MoveTowards(rightWall.position, rightWallTarget, Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        if (reloading)
        {
            coroutineGoByTheRouteAllowed = true;
            yield break;
        }

        changeCamera = true;
        var noisePerlin = scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noisePerlin.m_AmplitudeGain = 2;
        noisePerlin.m_FrequencyGain = 2;
    }

    private IEnumerator GoByTheStartRoute()
    {
        Vector2 p0 = routes[2].GetChild(0).position;
        Vector2 p1 = routes[2].GetChild(1).position;
        Vector2 p2 = routes[2].GetChild(2).position;
        Vector2 p3 = routes[2].GetChild(3).position;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * startSpeed;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;

            enemy.SetPositionAndRotation(objectPosition, Quaternion.RotateTowards(enemy.rotation, Quaternion.Euler(0, 0, 180), -rotationSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }

        tParam = 0;
        enemy.rotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(UpdatePosition());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + leftEdge, transform.position.y), 0.25f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + rightEdge, transform.position.y), 0.25f);
    }
}
