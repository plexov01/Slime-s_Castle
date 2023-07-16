using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_ParticlesDamage : MonoBehaviour
{
    Transform player;
    private ParticleSystem partSys;

    [Header("Damage")]
    public float damage;

    private void Awake()
    {
        partSys = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        player = scr_GameManager.instance.player.transform;
        partSys.trigger.AddCollider(player.GetComponent<CircleCollider2D>());
    }

    private void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> insideParticles = new List<ParticleSystem.Particle>();
        int insideCount = partSys.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideParticles);

        if (insideCount > 0)
        {
            player.GetComponent<scr_IDamageable>().ApplyDamage(damage);
        }
    }

}
