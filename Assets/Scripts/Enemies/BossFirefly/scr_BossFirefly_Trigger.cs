using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_Trigger : MonoBehaviour
{
    public string objectName;
    [SerializeField] [Range(1, 2)] private int stage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            scr_EventSystem.instance.playerEnteredObjectTrigger.Invoke(objectName);

            switch (stage)
            {
                case 1:
                    StartCoroutine(DestroyTriggerStage1());
                    break;
                case 2:
                    StartCoroutine(DestroyTriggerStage2());
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator DestroyTriggerStage1()
    {
        yield return new WaitUntil(() => scr_BossFirefly_Phase2_Stage1.changeCamera);
        scr_EventSystem.instance.playerLeftObjectTrigger.Invoke(objectName);
        Destroy(gameObject);
    }

    private IEnumerator DestroyTriggerStage2()
    {
        yield return new WaitUntil(() => scr_BossFirefly_Phase2_Stage2.changeCamera);
        scr_EventSystem.instance.playerLeftObjectTrigger.Invoke(objectName);
        Destroy(gameObject);
    }
}

