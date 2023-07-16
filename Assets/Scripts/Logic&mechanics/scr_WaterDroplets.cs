using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_WaterDroplets : MonoBehaviour
{
    Transform player;
    ParticleSystem partSys;
    scr_cnpt_FormBehavior formBehavior;

    [SerializeField] private float activeTime;
    [SerializeField] private float inactiveTime;
    private float activeDuration = 0;
    private float inactiveDuration = 0;
    private bool active = true;

    private void Awake()
    {
        partSys = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
        formBehavior = scr_cnpt_FormBehavior.instance;
        partSys.trigger.AddCollider(player.GetComponent<CircleCollider2D>());
    }

    private void Update()
    {
        if (activeDuration < activeTime && active)
        {
            activeDuration += Time.deltaTime;
        }
        else if (active)
        {
            activeDuration = 0;
            active = false;
            partSys.Stop();
        }

        if (inactiveDuration < inactiveTime && !active)
        {
            inactiveDuration += Time.deltaTime;
        }
        else if (!active)
        {
            inactiveDuration = 0;
            active = true;
            partSys.Play();
        }

    }

    private void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> insideParticles = new List<ParticleSystem.Particle>();
        int insideCount = partSys.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideParticles);

        if (insideCount > 0)
        {
            if (formBehavior._currentForm.GetType() != typeof(scr_SlimeForm))
            {
                formBehavior.NextForm(enum_forms.Slime);
            }
        }
    }

}
