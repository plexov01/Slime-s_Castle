using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Fireflies : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] private Transform[] particleSystemTransforms;
    private ParticleSystem[] particleSystems;
    private bool[] particleOnPlace;
    private int index;

    [Header("Area")]
    [SerializeField] private float areaRadius;
    [SerializeField] private Transform area1;
    [SerializeField] private Transform area2;

    [Header("Change Area")]
    [SerializeField] [Range(0, 20)] private float NewAreaRadius;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    [SerializeField] private float timeStaying;
    [SerializeField] private float stayingTimer;
    [SerializeField] [Range(0, 5)] private float flyingSpeed;
    public Vector3 startPosition;
    public Vector3 NewAreaPosition;
    private bool areaChanged;
    private bool changingArea;
    private int numberChangedArea;

    private void Awake()
    {
        area1.localScale = new Vector3(areaRadius, areaRadius, areaRadius) / 1.25f;
        area2.localScale = new Vector3(areaRadius, areaRadius, areaRadius) / 1.25f;
    }

    private void Start()
    {
        timeStaying = Random.Range(minTime, maxTime);
        
        areaChanged = true;
        numberChangedArea = 0;

        startPosition = transform.position;
        NewAreaPosition = startPosition;
        area2.gameObject.SetActive(false);

        index = 0;
        particleSystems = new ParticleSystem[particleSystemTransforms.Length];
        particleOnPlace = new bool[particleSystemTransforms.Length];

        for (int i = 0; i < particleSystemTransforms.Length; i++)
        {
            particleSystems[i] = particleSystemTransforms[i].GetComponent<ParticleSystem>();
            particleOnPlace[i] = true;
        }
    }

    private void FixedUpdate()
    {
        if (areaChanged && !changingArea && timeStaying < stayingTimer)
        {
            StartCoroutine(ChangeArea());
        }

        for (int i = 0; i < particleSystemTransforms.Length; i++)
        {
            var particle = new ParticleSystem.Particle[1];
            particleSystems[i].GetParticles(particle);

            if (Vector2.Distance(particle[0].position, area1.position) > areaRadius 
                && Vector2.Distance(particle[0].position, area2.position) > areaRadius 
                && particleSystems[i].GetParticles(particle) == 1
                && particleOnPlace[i])
            {
                //print("restart " + i);
                //print(Vector2.Distance(particle[0].position, NewAreaPosition));
                particleSystems[i].Clear();
                particleSystems[i].Play();
            }
        }

        stayingTimer += Time.fixedDeltaTime;
    }

    private IEnumerator ChangeArea()
    {
        areaChanged = false;
        changingArea = true;

        Transform oldArea = null;
        Transform newArea = null;

        if (area1.gameObject.activeInHierarchy)
        {
            oldArea = area1;
            newArea = area2;
        }
        else if (area2.gameObject.activeInHierarchy)
        {
            oldArea = area2;
            newArea = area1;
        }

        NewAreaPosition = (Vector2)startPosition + Random.insideUnitCircle * NewAreaRadius;
        //print("new area position: " + NewAreaPosition);

        while (Vector2.Distance(oldArea.position, NewAreaPosition) <= 2f * areaRadius)
        {
            NewAreaPosition = (Vector2)startPosition + Random.insideUnitCircle * NewAreaRadius;
            //print("new area position: " + NewAreaPosition);
        }

        newArea.position = NewAreaPosition;
        newArea.gameObject.SetActive(true);

        for (int i = 0; i < particleSystemTransforms.Length; i++)
        {
            particleSystemTransforms[i].position = NewAreaPosition;
        }

        int numberOfParticles = particleSystems.Length;

        while (index < numberOfParticles)
        {
            if (index != 0)
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            }

            int fliesNumber = Random.Range(1, 4);

            if (index + fliesNumber > numberOfParticles - 1)
            {
                fliesNumber = numberOfParticles - index;
            }

            //print("fliesNumber: " + fliesNumber);

            for (int i = index; i < index + fliesNumber; i++)
            {
                yield return new WaitForSeconds(Random.Range(0f, 0.25f));
                StartCoroutine(ChangeAreaForOne(i));
            }

            yield return new WaitUntil(() => numberChangedArea == fliesNumber);
            //print("changed: " + (numberChangedArea));
            index += fliesNumber;
            numberChangedArea = 0;
        }

        oldArea.gameObject.SetActive(false);

        timeStaying = Random.Range(minTime, maxTime);
        stayingTimer = 0f;
        index = 0;
        changingArea = false;
        areaChanged = true;
    }

    private IEnumerator ChangeAreaForOne(int index)
    {
        Vector3 fliesPosition = (Vector2)NewAreaPosition + 0.9f * areaRadius * Random.insideUnitCircle;
        var coll = particleSystems[index].collision;
        coll.enabled = false;
        var particle = new ParticleSystem.Particle[1];
        particleOnPlace[index] = false;

        while (Vector2.Distance(particle[0].position, fliesPosition) >= 0.01f)
        {
            yield return new WaitForFixedUpdate();
            int numberOfParticles = particleSystems[index].GetParticles(particle);
            particle[0].position = Vector3.MoveTowards(particle[0].position, fliesPosition, flyingSpeed * Time.fixedDeltaTime);
            particleSystems[index].SetParticles(particle, numberOfParticles);
            //print("moveTowards " + index);
        }

        particleOnPlace[index] = true;
        coll.enabled = true;
        numberChangedArea++;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, NewAreaRadius);
        Gizmos.DrawWireSphere(area1.position, areaRadius);
        Gizmos.DrawWireSphere(area2.position, areaRadius);
    }
}
