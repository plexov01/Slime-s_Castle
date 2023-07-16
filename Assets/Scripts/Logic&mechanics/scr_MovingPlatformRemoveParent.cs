using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_MovingPlatformRemoveParent : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
