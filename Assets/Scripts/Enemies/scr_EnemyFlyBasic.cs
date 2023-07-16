using UnityEngine;

public class scr_EnemyFlyBasic : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] [Range(0, 20)] private float flightRadius;
    [SerializeField] [Range(0, 5)] private float timeStaying;
    [SerializeField] private bool randomTimeStaying;
    [SerializeField] private float maxTime;
    [SerializeField] [Range(0, 5)] private float speedFlying;
    private Vector3 startPosition;
    private Vector3 fliesPosition;
    private Vector3 initScale;
    private float oldDestination;
    private float newDestination;
    private float deltaX;
    private float timeTempVariable;
    
    [Header("Status")]
    [SerializeField] private bool staying;
    [SerializeField] private bool fliesTo;


    private void Start()
    {
        staying = true;

        initScale = (transform.localScale.x >= 0) ? transform.localScale : new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        startPosition = transform.position;
        fliesPosition = startPosition;

        if (randomTimeStaying)
        {
            timeStaying = Random.Range(0f, maxTime);
        }
    }

    private void FixedUpdate() 
    {
        if (Vector2.Distance(transform.position, fliesPosition) < 0.01f)
        {
            staying = true;
            fliesTo = false;
        }

        if (timeStaying < timeTempVariable)
        {
            if (randomTimeStaying)
            {
                timeStaying = Random.Range(0f, maxTime);
            }

            staying = false;
            fliesTo = true;
            timeTempVariable = 0;
            oldDestination = fliesPosition.x;
            fliesPosition = (Vector2)startPosition + Random.insideUnitCircle * flightRadius;
            newDestination = fliesPosition.x;

            deltaX = newDestination - oldDestination;

            if (deltaX > 0)
            {
                transform.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
            }
            else if (deltaX < 0)
            {
                transform.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
            }
        }

        if (staying)
        {
            timeTempVariable += Time.fixedDeltaTime;
        }
        else if (fliesTo)
        {
            FliesTo();
        }
    }

    private void FliesTo()
    {
        transform.position = Vector3.MoveTowards(transform.position, fliesPosition, speedFlying * Time.fixedDeltaTime);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.parent.position, flightRadius);
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(fliesPosition, flightRadius / 10);
    }
}
