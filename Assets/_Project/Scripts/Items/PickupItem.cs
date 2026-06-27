using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] private int nectarAmount = 1;
    [SerializeField] private float pickupRadius = 0.65f;
    [SerializeField] private float magnetRadius = 2.2f;
    [SerializeField] private float magnetSpeed = 4.8f;
    [SerializeField] private float floatAmplitude = 0.08f;
    [SerializeField] private float floatSpeed = 4f;
    [SerializeField] private float lifeTime = 20f;

    private NectarWallet targetWallet;
    private Vector3 basePosition;
    private float age;
    private bool isMagnetized;

    private void Awake()
    {
        basePosition = transform.position;
    }

    private void Update()
    {
        age += Time.deltaTime;

        if (lifeTime > 0f && age >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        if (targetWallet == null)
        {
            targetWallet = FindObjectOfType<NectarWallet>();
        }

        if (targetWallet == null)
        {
            FloatInPlace();
            return;
        }

        float distance = Vector2.Distance(transform.position, targetWallet.transform.position);

        if (distance <= pickupRadius)
        {
            Collect();
            return;
        }

        if (distance <= magnetRadius)
        {
            MoveTowardTarget(distance);
            return;
        }

        isMagnetized = false;
        FloatInPlace();
    }

    public void Configure(int amount, float radius, float targetLifeTime)
    {
        nectarAmount = Mathf.Max(1, amount);
        pickupRadius = Mathf.Max(0.05f, radius);
        lifeTime = Mathf.Max(0f, targetLifeTime);
    }

    public void ConfigureMagnet(float radius, float speed)
    {
        magnetRadius = Mathf.Max(pickupRadius, radius);
        magnetSpeed = Mathf.Max(0.1f, speed);
    }

    private void FloatInPlace()
    {
        transform.position = basePosition + new Vector3(0f, Mathf.Sin(Time.time * floatSpeed) * floatAmplitude, 0f);
    }

    private void MoveTowardTarget(float distance)
    {
        isMagnetized = true;

        Vector3 targetPosition = targetWallet.transform.position + new Vector3(0f, 0.1f, 0f);
        float speedMultiplier = Mathf.InverseLerp(magnetRadius, pickupRadius, distance);
        float currentSpeed = magnetSpeed * Mathf.Lerp(1f, 1.8f, speedMultiplier);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        basePosition = transform.position;
    }

    private void Collect()
    {
        targetWallet.AddNectar(nectarAmount);
        PickupFloatingText.Show(transform.position, $"Nectar +{nectarAmount}");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
