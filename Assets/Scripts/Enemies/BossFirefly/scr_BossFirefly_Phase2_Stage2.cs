using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class scr_BossFirefly_Phase2_Stage2 : State
{
    [Header("Next State")]
    public scr_BossFirefly_Phase2_DeadState deadState;
    private bool changeStage = false;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private float virtualCameraSize;
    private float virtualCameraStartHeight;
    private Transform player;
    [SerializeField] private Transform stage2Center;
    private bool loadComplete = false;
    public static bool changeCamera = false;

    [Header("Reload")]
    [SerializeField] private Transform playerRespawn;
    private bool reloading = false;

    [SerializeField] private BoxCollider2D stage2Save;

    [Header("Moving Objects")]
    [SerializeField] private Transform fallingGround;
    private Rigidbody2D fallingGroundRB;
    private Vector2 fallingGroundStart;
    [SerializeField] private float fallingGroundSpeed;
    [SerializeField] private Transform movingWalls;
    private Rigidbody2D movingWallsRB;
    private Vector2 movingWallsStart;
    [SerializeField] private float movingWallsSpeed;
    [SerializeField] private Transform movingBackground;
    private Rigidbody2D movingBackgroundRB;
    private Vector2 movingBackgroundStart;
    [SerializeField] private float movingBackgroundSpeed;
    //[SerializeField] private ScrollingBG scrollingBG;

    [Header("Stage End")]
    [SerializeField] private scr_BossFirefly_Phase2_Stage2_EndTrigger endTrigger;
    [SerializeField] private Transform rightEndWall;
    [SerializeField] private Transform leftEndWall;
    private Vector2 endWallTarget;
    private bool moveWall = false;
    private bool stopFall = false;
    [SerializeField] private Transform savePosition;

    private void Awake()
    {
        fallingGroundRB = fallingGround.GetComponent<Rigidbody2D>();
        movingWallsRB = movingWalls.GetComponent<Rigidbody2D>();
        movingBackgroundRB = movingBackground.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
        rightEndWall.gameObject.SetActive(false);
        leftEndWall.gameObject.SetActive(false);
        fallingGroundStart = fallingGround.position;
        movingWallsStart = movingWalls.position;
        movingBackgroundStart = movingBackground.position;
    }

    public override void Enter()
    {
        scr_BossFirefly_Phase2_ChooseStage.currentStage = "Stage2";
        stage2Save.enabled = true;
        virtualCameraStartHeight = virtualCamera.transform.position.y;
        virtualCameraSize = 4.5f;
        scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = virtualCameraSize;
        //scrollingBG.scroolBG = true;
        var noisePerlin = scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noisePerlin.m_AmplitudeGain = 0;
        noisePerlin.m_FrequencyGain = 0;
        StartCoroutine(Load());
    }

    public override State RunCurrentStateLogic()
    {
        if (changeStage)
        {
            return deadState;
        }

        if (scr_Player.playerDied && !reloading)
        {
            StartCoroutine(Reload());
        }

        if (endTrigger.enteredTrigger && !changeCamera)
        {
            changeCamera = true;
            StartCoroutine(EndStage());
        }

        return this;
    }

    public override void RunCurrentStatePhysics()
    {   
        if (!changeCamera && loadComplete && (virtualCamera.transform.position.y - player.position.y > 1.1 * virtualCameraSize))
        {
            player.gameObject.GetComponent<scr_IDamageable>().ApplyDamage(instantKill: true);
        }

        if (moveWall)
        {
            rightEndWall.position = Vector2.MoveTowards(rightEndWall.position, endWallTarget, Time.fixedDeltaTime);
        }
        else
        {
            movingWallsRB.velocity = new Vector2(0, movingWallsSpeed);
            movingBackgroundRB.velocity = new Vector2(0, movingBackgroundSpeed);
            //movingWalls.Translate(movingWallsSpeed * Time.fixedDeltaTime * Vector3.up, Space.World);
        }

        if (!stopFall)
        { 
            fallingGroundRB.velocity = new Vector2(0, -fallingGroundSpeed);
            //fallingGround.Translate(fallingGroundSpeed * Time.fixedDeltaTime * Vector3.down, Space.World);
        }

    }

    private IEnumerator Load()
    {
        player.position = new Vector2(stage2Center.position.x + 1, stage2Center.position.y);
        virtualCamera.ForceCameraPosition(new Vector3(stage2Center.position.x, stage2Center.position.y, virtualCamera.transform.position.z), new Quaternion(0, 0, 0, 0));
        yield return new WaitForSeconds(0.5f);
        loadComplete = true;
    }

    private IEnumerator Reload()
    {
        print("reloading stage2");
        reloading = true;
        virtualCamera.transform.position = new Vector3(virtualCamera.transform.position.x, virtualCameraStartHeight, virtualCamera.transform.position.z);
        fallingGround.position = fallingGroundStart;
        movingWalls.position = movingWallsStart;
        movingBackground.position = movingBackgroundStart;
        yield return new WaitForSeconds(1f);
        reloading = false;
    }

    private IEnumerator EndStage()
    {
        rightEndWall.gameObject.SetActive(true);
        leftEndWall.gameObject.SetActive(true);
        endWallTarget = new Vector2(rightEndWall.position.x - 1.5f, rightEndWall.position.y);
        moveWall = true;
        movingWallsRB.velocity = Vector2.zero;
        movingBackgroundRB.velocity = Vector2.zero;
        fallingGroundRB.bodyType = RigidbodyType2D.Dynamic;
        fallingGroundRB.freezeRotation = true;
        fallingGroundRB.gravityScale = 5f;
        var noisePerlin = scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noisePerlin.m_AmplitudeGain = 1;
        noisePerlin.m_FrequencyGain = 1;
        scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 4f;
        //scrollingBG.scroolBG = false;
        yield return new WaitForSeconds(3f);
        stopFall = true;
        Destroy(fallingGround.gameObject);
        MenuController.instance.ShowBlackScreen();
        noisePerlin.m_AmplitudeGain = 0;
        noisePerlin.m_FrequencyGain = 0;
        movingWalls.position = movingWallsStart;
        movingBackground.position = movingBackgroundStart;
        player.position = new Vector3(player.position.x, savePosition.position.y, player.position.z);
        yield return new WaitForSeconds(0.5f);
        leftEndWall.gameObject.SetActive(false);
        MenuController.instance.HideBlackScreen();
        changeStage = true;
    }
}
