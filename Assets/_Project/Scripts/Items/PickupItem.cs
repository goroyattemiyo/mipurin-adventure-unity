using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] private int nectarAmount = 1;
    [SerializeField] private float pickupRadius = 0.65f;
    [SerializeField] private float floatAmplitude = 0.08f;
    [SerializeField] private float floatSpeed = 4f;
    [SerializeField] private float lifeTime = 20f;

    private NectarWallet targetWallet;
    private Vector3 startPosition;
    private float age;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        age += Time.deltaTime;

        if (lifeTime > 0f && age >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = startPosition + new Vector3(0f, Mathf.Sin(Time.time * floatSpeed) * floatAmplitude, 0f);

        if (targetWallet == null)
        {
            targetWallet = FindObjectOfType<NectarWallet>();
        }

        if (targetWallet == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, targetWallet.transform.position);
        if (distance > pickupRadius)
        {
            return;
        }

        targetWallet.AddNectar(nectarAmount);
        Destroy(gameObject);
    }

    public void Configure(int amount, float radius, float targetLifeTime)
    {
        nectarAmount = Mathf.Max(1, amount);
        pickupRadius = Mathf.Max(0.05f, radius);
        lifeTime = Mathf.Max(0f, targetLifeTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
