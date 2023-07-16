using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class scr_BossFirefly_Phase2_ChooseStage : State
{
    [Header("Next State")]
    public scr_BossFirefly_Phase2_Stage1 stage1;
    public scr_BossFirefly_Phase2_Stage2 stage2;
    public scr_BossFirefly_Phase2_DeadState deadState;
    public static string currentStage = "Stage1";
    public string stage;
    public bool bossIsDead;
    scr_GameManager GameManager;
    scr_SaveController SaveController;

    private void Start()
    {
        SaveController = scr_SaveController.instance;
        GameManager = scr_GameManager.instance;
        stage = SaveController.GetSaveGame(GameManager.currentSaveGame.numberOfSave).bossFireflyPhase2_stage;

        if (stage.Length != 0)
        {
            currentStage = stage;
        }
        //print(stage);
        bossIsDead = SaveController.GetSaveGame(GameManager.currentSaveGame.numberOfSave).bossFireflyIsDead;
        scr_MiniMapController.instance.showMiniMap = false;

        StartCoroutine(HideBlackScreen());
    }

    public override State RunCurrentStateLogic()
    {
        if (bossIsDead)
        {
            return deadState;
        }  
        else if (currentStage == "Stage1")
        {
            return stage1;
        }
        else if (currentStage == "Stage2")
        {
            return stage2;
        }
        
        return this;
    }

    public override void Exit()
    {

    }

    private IEnumerator HideBlackScreen()
    {
        var noisePerlin = scr_CameraManager.instance.transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noisePerlin.m_AmplitudeGain = 0;
        noisePerlin.m_FrequencyGain = 0;
        yield return new WaitForSeconds(0.1f);
        MenuController.instance.HideBlackScreen();
    }
}
