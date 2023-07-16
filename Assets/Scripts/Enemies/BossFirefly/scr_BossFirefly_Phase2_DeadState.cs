using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_Phase2_DeadState : State
{
    [SerializeField] private Transform endWall;
    private Vector3 endWallStartPosition;

    private void Start()
    {
        endWallStartPosition = endWall.position;
    }

    public override void Enter()
    {
        scr_MiniMapController.instance.showMiniMap = true;

        if (scr_SaveController.instance.GetSaveGame(scr_GameManager.instance.currentSaveGame.numberOfSave).bossFireflyIsDead && endWall.position == endWallStartPosition)
        {
            LoadBossDead();
        }
    }

    public override State RunCurrentStateLogic()
    {
        return this;
    }

    private void LoadBossDead()
    {
        endWall.gameObject.SetActive(true);
        endWall.position = new Vector2(endWall.position.x - 1.5f, endWall.position.y);
    }
}
