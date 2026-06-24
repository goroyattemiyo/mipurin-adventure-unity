using UnityEngine;

public class MipurinContactDamage : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private MipurinHealth target;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 1.2f;
    [SerializeField] private float contactRadius = 0.85f;
    [SerializeField] private bool waitIntervalOnStart = true;

    private float timer;

    private void OnEnable()
    {
        if (waitIntervalOnStart)
        {
            timer = Mathf.Max(timer, damageInterval * 0.5f);
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (target == null)
        {
            target = FindObjectOfType<MipurinHealth>();
        }

        if (target == null || !target.IsAlive || timer > 0f)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > contactRadius)
        {
            return;
        }

        Vector2 direction = (target.transform.position - transform.position).normalized;
        target.TakeDamage(damage, transform.position, direction);
        timer = damageInterval;
    }

    public void Configure(MipurinHealth damageTarget, int contactDamage, float interval, float radius)
    {
        target = damageTarget;
        damage = Mathf.Max(0, contactDamage);
        damageInterval = Mathf.Max(0.05f, interval);
        contactRadius = Mathf.Max(0.05f, radius);
    }

    public void ResetDamageTimer()
    {
        timer = damageInterval;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactRadius);
    }
}
