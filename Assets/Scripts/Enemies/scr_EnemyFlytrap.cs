using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyFlytrap : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform rightHalf;
    [SerializeField] private Transform leftHalf;
    [SerializeField] private BoxCollider2D damageCollider;
    [SerializeField] private scr_FlytrapHealth health;

    private bool active = false;

    private Vector3 initPositionLeftHalf;
    private Vector3 initPositionRightHalf;
    private Vector3 zAxis = new Vector3(0, 0, 1);

    [Header("Attack")]
    [SerializeField] private float activateDelay;
    [SerializeField] private float activeTime;
    [SerializeField] private float attackDuration;

    [Header("Debug Info")]
    public bool EnteredTrigger = false;
    public bool trapActivated = false;
    public bool recharge = false;
    public bool activateCoroutineIsRunning = false;

    private void Start()
    {
        damageCollider.enabled = false;
        initPositionLeftHalf = leftHalf.localPosition;
        initPositionRightHalf = rightHalf.localPosition;
    }


    private void FixedUpdate()
    {
        if (trapActivated)
        {
            if (rightHalf.localRotation.eulerAngles.z >= 270 && rightHalf.localRotation.eulerAngles.z <= 345)
            {
                rightHalf.RotateAround(transform.position, zAxis, 90f / attackDuration * Time.fixedDeltaTime);
            }

            if (leftHalf.localRotation.eulerAngles.z >= 195 && leftHalf.localRotation.eulerAngles.z <= 270)
            {
                leftHalf.RotateAround(transform.position, zAxis, -90f / attackDuration * Time.fixedDeltaTime);
            }
        }

        if (recharge)
        {
            if (rightHalf.localRotation.eulerAngles.z >= 270 || rightHalf.localRotation.eulerAngles.z < 1)
            {
                rightHalf.RotateAround(transform.position, zAxis, -90f / attackDuration * Time.fixedDeltaTime);
            }
            else
            {
                EndRecharge();
            }

            if (leftHalf.localRotation.eulerAngles.z <= 270)
            {
                leftHalf.RotateAround(transform.position, zAxis, 90f / attackDuration * Time.fixedDeltaTime);
            }
            else
            {
                EndRecharge();
            }
        }
    }

    private void EndRecharge()
    {
        recharge = false;
        active = false;
        health.canTakeDamageFromNormalAttack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        React(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        React(collision);
    }

    private void React(Collider2D collision)
    {
        if (!EnteredTrigger && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")) && !active)
        {
            //print("Entered trigger: " + collision.gameObject.name);
            EnteredTrigger = true;
            active = true;
        }

        if (EnteredTrigger && !trapActivated && !activateCoroutineIsRunning)
        {
            StartCoroutine(Activate());
        }
    }

    private IEnumerator Activate()
    {
        activateCoroutineIsRunning = true;
        rightHalf.localRotation = Quaternion.Euler(0, 0, 270);
        rightHalf.localPosition = initPositionRightHalf;
        leftHalf.localRotation = Quaternion.Euler(0, 0, 270);
        leftHalf.localPosition = initPositionLeftHalf;
        yield return new WaitForSeconds(activateDelay);
        trapActivated = true;
        yield return new WaitForSeconds(attackDuration * 0.2f);
        health.canTakeDamageFromNormalAttack = true;
        damageCollider.enabled = true;
        yield return new WaitForSeconds(attackDuration * 0.8f);
        trapActivated = false;
        rightHalf.localRotation = Quaternion.Euler(0, 0, 345);
        leftHalf.localRotation = Quaternion.Euler(0, 0, 195);
        damageCollider.enabled = false;
        //rightHalf.localRotation = Quaternion.Euler(0, 0, 0);
        //leftHalf.localRotation = Quaternion.Euler(0, 0, 180);
        yield return new WaitForSeconds(activeTime);
        //rightHalf.localRotation = Quaternion.Euler(0, 0, 270);
        //leftHalf.localRotation = Quaternion.Euler(0, 0, 270);
        recharge = true;
        EnteredTrigger = false;
        activateCoroutineIsRunning = false;
    }
}
