using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Coin : MonoBehaviour
{
    private bool isInteractable = false;

    public delegate void TakeCoinAction();
    public static event TakeCoinAction PlayerGotACoin;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        StartCoroutine(BecomeInteractable());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isInteractable)
        {
            if (collision.CompareTag("Player"))
            {
                //��������� �������
                collision.gameObject.GetComponent<scr_Player>().AddCoin(1);
                //audioSource.Play();
                GetComponent<scr_persistedObject>().recoverable = false;
                Destroy(gameObject);

            }
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (isInteractable)
    //    {
    //        if (collision.CompareTag("Player"))
    //        {
    //            //��������� �������
    //            collision.gameObject.GetComponent<scr_Player>().AddCoin();
    //            //Destroy(gameObject);
    //        }
    //    }
    //}

    IEnumerator BecomeInteractable()
    {
        yield return new WaitForSeconds(0.5f);
        isInteractable = true;
    }
}
