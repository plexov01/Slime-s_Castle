using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_Phase1_AttackState : State
{
    [Header("Next State")]
    public scr_BossFirefly_Phase1_ChaseState chaseState;
    public scr_BossFirefly_Phase1_DeathState deathState;
    public scr_BossFirefly_Phase1_ReloadState reloadState;
    public bool enterChaseState;
    public bool enterDeathState;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    private bool hitByFirstStone = false;
    private bool hitBySecondStone = false;

    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    private int chosenAttack;
    private Transform player;
    public static int stage = 0;

    [Header("Wind Attack")]
    [SerializeField] private int maxRepeatWind;
    private int numberOfRepeatedWindAttacks = 0;
    [SerializeField] private float windSpeed;
    [SerializeField] private GameObject rightWind;
    [SerializeField] private Transform rightWindEnd;
    [SerializeField] private GameObject leftWind;
    [SerializeField] private Transform leftWindEnd;
    [SerializeField] private Transform groundLevel;
    private GameObject wind;
    private Transform windEnd;
    public static List<GameObject> winds = new List<GameObject>();
    private BoxCollider2D windCollider;
    private float defaultLowerWindHeight;

    [Header("Laser Attack")]
    [SerializeField] private int maxRepeatLaser;
    private int numberOfRepeatedLaserAttacks = 0;
    [SerializeField] private float laserDamage;
    [SerializeField] private Transform laserObject;
    public static scr_laser laserScript;
    [SerializeField] private scr_BossFirefly_LaserReceiver playerLaserReceiverScript;
    
    [Header("Burning Objcets")]
    [SerializeField] private float burnDuration;
    [SerializeField] private Transform angleLever;
    private GameObject leverBox;
    private scr_BossFirefly_LaserReceiver leverLaserReveiver;
    private bool deleteLeverBox = false;
    [SerializeField] private Transform leftCeiling;
    private GameObject leftWood;
    private scr_BossFirefly_LaserReceiver leftLaserReceiver;
    private bool deleteLeftCeiling = false;
    [SerializeField] private Transform rightCeiling;
    private GameObject rightWood;
    private scr_BossFirefly_LaserReceiver rightLaserReceiver;
    private bool deleteRightCeiling = false;

    private void Awake()
    {
        windCollider = rightWind.GetComponent<BoxCollider2D>();
        laserScript = laserObject.GetComponent<scr_laser>();
        leverBox = angleLever.GetChild(0).gameObject;
        leverLaserReveiver = angleLever.GetChild(1).GetComponent<scr_BossFirefly_LaserReceiver>();

        leftWood = leftCeiling.GetChild(0).gameObject;
        leftLaserReceiver = leftCeiling.GetChild(2).GetComponent<scr_BossFirefly_LaserReceiver>();
        rightWood = rightCeiling.GetChild(0).gameObject;
        rightLaserReceiver = rightCeiling.GetChild(2).GetComponent<scr_BossFirefly_LaserReceiver>();
    }

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
        rightWind.SetActive(false);
        leftWind.SetActive(false);
        defaultLowerWindHeight = groundLevel.position.y + windCollider.size.y / 2;
    }

    private void FixedUpdate()
    {
        attackCooldownTimer += Time.fixedDeltaTime;

        if (winds.Count != 0)
        {
            for (int i = 0; i < winds.Count; i++)
            {
                if (Mathf.Abs(winds[i].transform.position.x - windEnd.position.x) > 0.01f)
                {
                    winds[i].transform.position = Vector2.MoveTowards(winds[i].transform.position, new Vector2(windEnd.position.x, winds[i].transform.position.y), windSpeed * Time.deltaTime);
                }
                else
                {
                    Destroy(winds[i]);
                    winds.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public override void Enter()
    {
        attackCooldownTimer = attackCooldown / 3;
        numberOfRepeatedLaserAttacks = 0;
        numberOfRepeatedWindAttacks = 0;
        stage++;

        if (stage == 1 || stage == 3)
        {
            enemy.localScale = new Vector2(scr_BossFirefly_Phase1.initScale.x, scr_BossFirefly_Phase1.initScale.y);
            wind = rightWind;
            windEnd = rightWindEnd;
        }
        else
        {
            enemy.localScale = new Vector2(-scr_BossFirefly_Phase1.initScale.x, scr_BossFirefly_Phase1.initScale.y);
            wind = leftWind;
            windEnd = leftWindEnd;
        }
    }

    public override State RunCurrentStateLogic()
    {
        if (enterChaseState)
        {
            return chaseState;
        }

        else if (enterDeathState)
        {
            return deathState;
        }

        if (scr_Player.playerDied)
        {
            return reloadState;
        }

        if (hitByFirstStone)
        {
            enterChaseState = true;
        }
        else if (hitBySecondStone)
        {
            enterDeathState = true;
        }

        else if (attackCooldownTimer >= attackCooldown)
        {
            chosenAttack = Random.Range(0, 2);

            if (numberOfRepeatedLaserAttacks >= maxRepeatLaser && chosenAttack == 0)
            {
                chosenAttack = 1;
            }
            else if (numberOfRepeatedWindAttacks >= maxRepeatWind && chosenAttack == 1)
            {
                chosenAttack = 0;
            }

            attackCooldownTimer = 0f;

            switch (chosenAttack)
            {
                case 0:
                    LaserAttack();
                    break;
                case 1:
                    StartCoroutine(WindAttack());
                    break;
                default:
                    break;
            }
        }

        if (playerLaserReceiverScript.isOn)
        {
            player.GetComponent<scr_IDamageable>().ApplyDamage(laserDamage);
        }

        if (leverLaserReveiver.isOn && !deleteLeverBox)
        {
            StartCoroutine(BurnLeverBox());
        }

        if (leftLaserReceiver.isOn && !deleteLeftCeiling)
        {
            StartCoroutine(BurnLeftCeiling());
        }

        if (rightLaserReceiver.isOn && !deleteRightCeiling)
        {
            StartCoroutine(BurnRightCeiling());
        }

        return this;
    }

    public override void Exit()
    {
        StopAllCoroutines();
        enterChaseState = false;
        laserScript.isOn = false;
        hitByFirstStone = false;
        hitBySecondStone = false;
        deleteLeverBox = false;
        deleteLeftCeiling = false;
        deleteRightCeiling = false;
        scr_BossFirefly_Phase1.anim.SetBool("lightAttack", false);
        scr_BossFirefly_Phase1.anim.SetBool("windAttack", false);
    }

    private IEnumerator WindAttack()
    {
        scr_BossFirefly_Phase1.anim.SetBool("windAttack", true);
        yield return new WaitUntil(() => scr_BossFirefly_Phase1.spawnWind);
        scr_BossFirefly_Phase1.spawnWind = false;
        numberOfRepeatedWindAttacks++;
        numberOfRepeatedLaserAttacks = 0;
        GameObject windAttack = Instantiate(rightWind);

        if (player.position.y - windCollider.size.y / 2 <= groundLevel.position.y)
        {
            windAttack.transform.position = new Vector2(wind.transform.position.x, defaultLowerWindHeight);
        }
        else
        {
            windAttack.transform.position = new Vector2(wind.transform.position.x, player.position.y);
        }

        windAttack.SetActive(true);
        winds.Add(windAttack);
        print("wind attack ¹" + numberOfRepeatedWindAttacks);
    }

    private void LaserAttack()
    {
        numberOfRepeatedLaserAttacks++;
        numberOfRepeatedWindAttacks = 0;
        print("laser attack ¹" + numberOfRepeatedLaserAttacks);
        var perpendicular = Vector3.Cross(laserObject.forward, player.position - laserObject.position);
        laserObject.rotation = Quaternion.LookRotation(laserObject.forward, perpendicular);
        scr_BossFirefly_Phase1.anim.SetBool("lightAttack", true);
    }

    private IEnumerator BurnLeverBox()
    {
        deleteLeverBox = true;
        print("hit lever box");
        yield return new WaitForSeconds(burnDuration);
        leverBox.SetActive(false);
        leverLaserReveiver.gameObject.SetActive(false);
        enterChaseState = true;
    }

    private IEnumerator BurnLeftCeiling()
    {
        deleteLeftCeiling = true;
        print("hit left ceiling");
        yield return new WaitForSeconds(burnDuration);
        leftWood.SetActive(false);
        yield return new WaitForSeconds(0.75f);
        hitByFirstStone = true;
        leftCeiling.gameObject.SetActive(false);
    }

    private IEnumerator BurnRightCeiling()
    {
        deleteRightCeiling = true;
        print("hit right ceiling");
        yield return new WaitForSeconds(burnDuration);
        rightWood.SetActive(false);
        yield return new WaitForSeconds(0.75f);
        hitBySecondStone = true;
        rightCeiling.gameObject.SetActive(false);
    }
}
