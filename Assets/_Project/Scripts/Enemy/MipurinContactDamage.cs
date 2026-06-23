using UnityEngine;

public class MipurinContactDamage : MonoBehaviour
{
    [SerializeField] private MipurinHealth target;
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float contactRadius = 0.8f;

    private float timer;

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
        damage = contactDamage;
        damageInterval = interval;
        contactRadius = radius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactRadius);
    }
}
