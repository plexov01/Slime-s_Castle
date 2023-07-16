using System.Collections;
using UnityEngine;

public class scr_Player : MonoBehaviour, scr_IDamageable
{
    public static scr_Player instance;

    public static float damageRate = 1f;
    public static float nextDamage;
    public static bool canTakeDamage = true;

    public static GameObject currentPickedObject = null;

    [SerializeField]private bool canHealing;
    public float maxHealth;
    public float currentHealth;
    public float healingRate;
    private SpriteRenderer spriteRenderer;
    public static bool invulnerable = false;

    private bool healing;

    public int currentNumberOfCoins;

    public Transform spawnPosition;
    [SerializeField]private float respawnTime;

    public delegate void Action(float health);
    public static event Action PlayerWasDamaged;

    public delegate void TakeCoinAction(int number);
    public static event Action PlayerGotACoin;

    public static bool playerDied = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        scr_EventSystem.instance.playerAwake.Invoke(transform);

        MenuController.SetSpawnPositionEvent+=SetSpawnPosition;
        MenuController.GetSpawnPositionEvent+=GetSpawnPosition;

        scr_GameManager GameManager = scr_GameManager.instance;
        GameManager.player = gameObject;

        EditorManager editorManager = EditorManager.instance;

        scr_MiniMapController MiniMapController = scr_MiniMapController.instance;// �������� ������ �� GameObject
        MiniMapController.player = this.gameObject;//
        // MiniMapController.showMiniMap = true;
        MiniMapController.playerExistOnScene = true;


        if (!GameManager.currentSaveGame.newGame)
        {
            transform.position = GameManager.currentSaveGame.position;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    private void Start()
    {
        PlayerWasDamaged(currentHealth);

    }

    private void Update(){
        healingPlayer();
    }

    public void AddCoin(int coins)
    {
        currentNumberOfCoins += coins;
        
        PlayerGotACoin(currentNumberOfCoins);
    }

    public void ApplyDamage(float damage, string tag, bool instantKill)
    {
        if (instantKill && canTakeDamage)
        {
            currentHealth = 0;
            canTakeDamage = false;
            StartCoroutine(Die());
            PlayerWasDamaged(currentHealth);
        }

        else if (Time.time > nextDamage && canTakeDamage)
        {
            nextDamage = Time.time + damageRate;
            currentHealth -= damage;

            StartCoroutine(DamageEffect());

            if (currentHealth <= 0)
            {
                canTakeDamage = false;
                StartCoroutine(Die());
            }

            PlayerWasDamaged(currentHealth);
        }
 
    }

    private IEnumerator DamageEffect()
    {
        invulnerable = true;
        spriteRenderer.color = new Color(1, 0, 0, 0.75f);
        yield return new WaitForSeconds(damageRate / 2);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(damageRate / 2);
        invulnerable = false;
    }

    public void healingPlayer(){
        if(!canHealing)return;
        //�������� ����������� �������������� HP
        healing = healingRate*Time.deltaTime + currentHealth <= maxHealth;

        if(healing){
            currentHealth += healingRate*Time.deltaTime;
        }

        PlayerWasDamaged(currentHealth);

    }
        

    public IEnumerator Die()
    {
        yield return null;
        playerDied = true;

        InputManager.instance.playerInput.actions.FindActionMap("Slime").Disable();
        MenuController.instance.ShowOrHideDiePanel();
        
        scr_EventSystem.instance.playerDeath.Invoke();

        StartCoroutine(Respawn(spawnPosition));
    }

    IEnumerator Respawn(Transform spawnPosition)
    {
        currentHealth = maxHealth;
        PlayerWasDamaged(currentHealth);

        yield return new WaitForSeconds(respawnTime);
        gameObject.transform.position = spawnPosition.position;

        InputManager.instance.playerInput.actions.FindActionMap("Slime").Enable();
        MenuController.instance.ShowOrHideDiePanel();

        canTakeDamage = true;
        playerDied = false;
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition.position=position;
        gameObject.transform.position = spawnPosition.position;
        Debug.Log(spawnPosition.position);
        Debug.Log(gameObject.transform.position);
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition.position;
    }

    private void OnDestroy() 
    {
        MenuController.SetSpawnPositionEvent-=SetSpawnPosition;
        MenuController.GetSpawnPositionEvent-=GetSpawnPosition;
    }


}
