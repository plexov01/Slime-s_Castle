using UnityEngine;

public class ResurrectStone : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private bool canHealing;
    [SerializeField] private float healingRateStone;
    private float healingRatePlayer;
    [SerializeField] private bool bossFireflyCheckpoint;
    [SerializeField] private bool bossFireflyExitCheckpoint;
    [SerializeField] private Transform savePosition;

    scr_SaveController SaveController;
    scr_GameManager GameManager;
    scr_TimeManager TimeManager;
    scr_Player Player;

    // Start is called before the first frame update
    void Start()
    {
        anim=GetComponent<Animator>();

        if (bossFireflyCheckpoint)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        SaveController = scr_SaveController.instance;
        GameManager = scr_GameManager.instance;
        TimeManager = scr_TimeManager.instance;
        Player = scr_Player.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D myTrigger)
    {
        if (myTrigger.CompareTag("Player"))
        {
            Player.spawnPosition = transform;

            if (canHealing)
            {
                healingRatePlayer = Player.healingRate;
                Player.healingRate = healingRateStone;
            }

            AutoSave();

            if (!anim.GetBool("Active"))
            {
                anim.SetBool("Active",true);
                if (!bossFireflyCheckpoint)
                {
                    scr_AudioManager.PlaySound("Save", gameObject);
                }
            }
            
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player")){
            
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            if(canHealing){
                Player.healingRate = healingRatePlayer;
            }
        }
    }

    public void AutoSave()
    {
        int numberOfSave = GameManager.currentSaveGame.numberOfSave;
        SaveGame save = SaveController.GetSaveGame(numberOfSave);
        save.UpdateTimeSave();
        save.newGame = false;
        save.position = transform.position;
        save.playerCoins = scr_Player.instance.currentNumberOfCoins;
        save.totalTime = GameManager.currentSaveGame.totalTime;
        save.totalTime += TimeManager.GetTimeSinceGetLastTime();
        save.nameScene = GameManager.nameScene;

        save.bossFireflyPhase1_firstTry = true;
        save.bossFireflyPhase2_stage = "";
        save.bossFireflyPhase2_stage1_firstTry = true;
        save.bossFireflyIsDead = false;

        if (bossFireflyCheckpoint)
        {
            if (GameManager.nameScene == "scn_bossFirefly_phase1")
            {
                save.bossFireflyPhase1_firstTry = false;
            }
            else if (GameManager.nameScene == "scn_bossFirefly_phase2")
            {
                save.bossFireflyPhase2_stage = scr_BossFirefly_Phase2_ChooseStage.currentStage;
                save.bossFireflyPhase2_stage1_firstTry = false;
            }
        }

        if (bossFireflyExitCheckpoint)
        {
            save.position = savePosition.position;
            save.bossFireflyIsDead = true;
        }

        GameManager.currentSaveGame=save;
        SaveController.SetSaveGame(numberOfSave,save);

    }

}
