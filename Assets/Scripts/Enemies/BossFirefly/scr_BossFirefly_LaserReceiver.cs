using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_LaserReceiver : MonoBehaviour
{
    private float laserReceivementTime = -1f;
    [SerializeField] private float timeRate = 0.1f;
    public bool isOn;

    [SerializeField] private Material requiredLaserMaterial;

    private void Update()
    {
        if (laserReceivementTime + timeRate <= Time.time)
        {
            if (isOn)
            {
                ChangeState();
            }
        }
        else
        {
            if (!isOn)
            {
                ChangeState();
            }
        }
    }

    public void Enable(Material laserMaterial)
    {
        if (laserMaterial == requiredLaserMaterial)
        {
            laserReceivementTime = Time.time;
        }
    }

    public void ChangeState()
    {
        isOn = !isOn;
    }

}
