using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCubeScript : MonoBehaviour
{
    void Start()
    {
        //scr_EventSystem.instance.mobDeath.Invoke(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"cube is triggering by {other.gameObject.name}");
    }
}
