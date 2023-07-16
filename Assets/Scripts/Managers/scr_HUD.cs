using UnityEngine;
using UnityEngine.UI;

public class scr_HUD : MonoBehaviour
{
    [SerializeField]private GameObject[] HPbarArray;

    private GameObject hpBar;
    private GameObject coinCounter;

    private void Start()
    {
        hpBar = GameObject.Find("HP_bar");
        coinCounter = GameObject.Find("Counter");

        scr_Player.PlayerWasDamaged += UpdateHPBar;
        scr_Player.PlayerGotACoin += UpdateCoins;

        HPbarArray = new GameObject[hpBar.transform.childCount];

        for (int i = 0; i < hpBar.transform.childCount; i++)
        {
            HPbarArray[i] = hpBar.transform.GetChild(i).gameObject;
        }
    }

    void UpdateHPBar(float currentHealth)
    {
        for (int i = 0; i < HPbarArray.Length; i++)
        {
            Image heart = HPbarArray[i].GetComponent<Image>();
            
            if(currentHealth-i<=1f && currentHealth-i>0){
                // Debug.Log("прозрачно" + (currentHealth-i));
                heart.fillAmount = currentHealth-i;
                // fillAmount 
                // heart.color = new Color(heart.color.r,heart.color.g,heart.color.b,currentHealth-i);
            }
            
            if(currentHealth-i<=0){
                heart.fillAmount = 0f;
                // heart.color = new Color(heart.color.r,heart.color.g,heart.color.b,0f);
            }

            if(currentHealth-i>1){
                heart.fillAmount = 1f;
                // heart.color = new Color(heart.color.r,heart.color.g,heart.color.b,1f);
            }

            // else{
                
            // }

                //             Image obj = HPbarArray[i].GetComponent<Image>();
                // Color col1 = new Color(currentHealth-i, obj.color.r, obj.color.g, obj.color.b);
                // obj.color = col1;

            // if (i < currentHealth)
            // {
            //     HPbarArray[i].SetActive(true);

                
            // }
            // else
            // {
            //     HPbarArray[i].SetActive(false);
            // }

            

            // if(currentHealth-i<=1f&&currentHealth-i>0){
            //     Debug.Log("прозрачно" + (currentHealth-i));

                
                // if(currentHealth-i<=1f).color.a = currentHealth-i;
        }
    }

    void UpdateCoins(float currentCoins)
    {
        coinCounter.gameObject.GetComponent<Text>().text = currentCoins.ToString();
    }

    private void OnDisable()
    {
        scr_Player.PlayerWasDamaged -= UpdateHPBar;
        scr_Player.PlayerGotACoin -= UpdateCoins;
    }
}
