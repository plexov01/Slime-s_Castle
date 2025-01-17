using UnityEngine;

public class scr_Timecounter : MonoBehaviour
{
    private scr_TimeManager TimeManager;

    private AudioSource soundCounterActive;


    private bool active;

    [SerializeField]private GameObject TopTimecounterWhite;
    [SerializeField]private GameObject TopTimecounterGreen;

    void Start()
    {
        TimeManager = scr_TimeManager.instance;
        soundCounterActive = GetComponent<AudioSource>();
        active = false;
        
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.CompareTag("Player"))
        {
            if(!active)
            {
                active = true;
                TopTimecounterWhite.SetActive(false);
                TopTimecounterGreen.SetActive(true);
                soundCounterActive.Play();
                TimeManager.SetTimeStartLevel();
            }
            
        }
        
    }
}
