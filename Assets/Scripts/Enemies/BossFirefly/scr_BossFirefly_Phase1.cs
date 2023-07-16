using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BossFirefly_Phase1 : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform enemyModel;
    public static Vector2 initScale;
    public static Animator anim;

    [Header("Laser")]
    [SerializeField] private Transform laserObject;
    public static scr_laser laserScript;

    [Header("Wind")]
    public static bool spawnWind = false;

    private void Awake()
    {
        initScale = enemy.localScale;
        anim = enemyModel.GetComponent<Animator>();
        laserScript = laserObject.GetComponent<scr_laser>();
    }

    private void LaserAttackStart()
    {
        laserScript.isOn = true;
    }

    private void LaserAttackEnd()
    {
        laserScript.isOn = false;
        anim.SetBool("lightAttack", false);
    }

    private void WindAttack()
    {
        spawnWind = true;
        anim.SetBool("windAttack", false);
    }
}
