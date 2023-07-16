using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_Phase1_ReloadState : State
{
    [Header("Next State")]
    public scr_BossFirefly_Phase1_AttackState attackState;

    [Header("Reload")]
    [SerializeField] private Transform mirror;
    [SerializeField] private Transform angleLever;
    private scr_BossFirefly_AngleLever angleLeverScript;
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform enemyStartPosition;
    [SerializeField] private Transform leftCeiling;
    private GameObject leftWood;
    private Transform leftRocks;
    private Vector2[] leftRocksStartPositions;
    [SerializeField] private Transform rightCeiling;
    private GameObject rightWood;
    private Transform rightRocks;
    private Vector2[] rightRocksStartPositions;
    private int numberOfRocks;
    private bool reloadComplete = false;

    private void Awake()
    {
        angleLeverScript = angleLever.GetComponent<scr_BossFirefly_AngleLever>();
        leftWood = leftCeiling.GetChild(0).gameObject;
        leftRocks = leftCeiling.GetChild(1);
        rightWood = rightCeiling.GetChild(0).gameObject;
        rightRocks = rightCeiling.GetChild(1);
    }

    private void Start()
    {
        numberOfRocks = leftRocks.childCount;

        leftRocksStartPositions = new Vector2[numberOfRocks];
        rightRocksStartPositions = new Vector2[numberOfRocks];

        for (int i = 0; i < numberOfRocks; i++)
        {
            leftRocksStartPositions[i] = leftRocks.GetChild(i).position;
            rightRocksStartPositions[i] = rightRocks.GetChild(i).position;
        }
    }

    public override void Enter()
    {
        StartCoroutine(Reload());
    }

    public override State RunCurrentStateLogic()
    {
        if (reloadComplete)
        {
            return attackState;
        }

        return this;
    }

    public override void Exit()
    {
        reloadComplete = false;
    }

    public IEnumerator Reload()
    {
        enemy.position = enemyStartPosition.position;
        enemy.localScale = new Vector3(1, 1, 1);

        scr_BossFirefly_Phase1.anim.SetBool("lightAttack", false);
        scr_BossFirefly_Phase1.anim.SetBool("windAttack", false);

        angleLever.GetChild(0).gameObject.SetActive(true);
        angleLever.GetChild(1).gameObject.SetActive(true);

        mirror.GetComponent<scr_RotateModule>().toDegrees = Quaternion.Euler(0, 0, 0);

        angleLeverScript.turnNumber = 0;

        leftCeiling.gameObject.SetActive(true);
        leftWood.SetActive(true);
        rightCeiling.gameObject.SetActive(true);
        rightWood.SetActive(true);

        for (int i = 0; i < numberOfRocks; i++)
        {
            leftRocks.GetChild(i).position = leftRocksStartPositions[i];
            rightRocks.GetChild(i).position = rightRocksStartPositions[i];
        }

        scr_BossFirefly_Phase1_AttackState.stage = 0;
        scr_BossFirefly_Phase1_AttackState.laserScript.isOn = false;

        for (int i = scr_BossFirefly_Phase1_AttackState.winds.Count - 1; i >= 0; i--)
        {
            Destroy(scr_BossFirefly_Phase1_AttackState.winds[i]);
        }

        scr_BossFirefly_Phase1_AttackState.winds.Clear();

        yield return new WaitForSeconds(1f);
        reloadComplete = true;
    }
}
