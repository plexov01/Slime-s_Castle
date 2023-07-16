using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class scr_BossFirefly_Phase1_ChaseState : State
{
    [Header("Next State")]
    public scr_BossFirefly_Phase1_AttackState attackState;
    public scr_BossFirefly_Phase1_ReloadState reloadState;
    public bool enterAttackState;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    private Seeker seeker;
    private AIPath aiPath;

    [Header("Chase parameters")]
    [SerializeField] private float chaseDuration;
    [SerializeField] private bool chaseTimerIsRunning = false;
    [SerializeField] private bool chaseTimerEnded = false;
    private Transform player;
    private Vector2 defaultPosition;
    [SerializeField] private Transform defPos1;
    [SerializeField] private Transform defPos2;

    private void Awake()
    {
        aiPath = enemy.GetComponent<AIPath>();
        seeker = enemy.GetComponent<Seeker>();
    }

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
        aiPath.enabled = false;
    }

    public override void Enter()
    {
        AstarPath.active.Scan();
        StartCoroutine(ChaseTimer());

        if (scr_BossFirefly_Phase1_AttackState.stage == 1 || scr_BossFirefly_Phase1_AttackState.stage == 3)
        {
            defaultPosition = defPos1.position;
        }
        else
        {
            defaultPosition = defPos2.position;
        }

        aiPath.enabled = true;
    }

    public override State RunCurrentStateLogic()
    {
        if (enterAttackState)
        {
            return attackState;
        }

        if (scr_Player.playerDied)
        {
            return reloadState;
        }

        return this;
    }

    public override void RunCurrentStatePhysics()
    {
        InvokeRepeating("UpdatePath", 0, 1f);

        if (chaseTimerEnded)
        {
            ReturnToStart();

            if (Vector2.Distance(enemy.position, defaultPosition) < 0.1f)
            {
                enterAttackState = true;
            }
        }

        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            enemy.localScale = new Vector2(-scr_BossFirefly_Phase1.initScale.x, scr_BossFirefly_Phase1.initScale.y);
        }
        else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            enemy.localScale = new Vector2(scr_BossFirefly_Phase1.initScale.x, scr_BossFirefly_Phase1.initScale.y);
        }
    }

    public override void Exit()
    {
        aiPath.enabled = false;
        CancelInvoke("UpdatePath");
        chaseTimerEnded = false;
        enterAttackState = false;
        StopAllCoroutines();
    }

    private IEnumerator ChaseTimer()
    {
        yield return new WaitForSeconds(chaseDuration);
        chaseTimerEnded = true;
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(enemy.position, player.position);
        }
    }

    private void ReturnToStart()
    {
        CancelInvoke("UpdatePath");
        seeker.StartPath(enemy.position, defaultPosition);
        InvokeRepeating("UpdatePath", 0, 1f);
    }
}
