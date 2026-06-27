using UnityEngine;

public class PoisonMushroomAura : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private MipurinHealth target;

    [Header("Poison Aura")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float radius = 1.05f;
    [SerializeField] private float interval = 2.2f;
    [SerializeField] private bool onlyForPoisonMushroom = true;

    [Header("Visual")]
    [SerializeField] private Color auraColor = new Color(0.55f, 0.15f, 0.9f, 0.28f);
    [SerializeField] private float pulseInterval = 0.8f;

    private float damageTimer;
    private float pulseTimer;

    private void Awake()
    {
        if (onlyForPoisonMushroom && !gameObject.name.Contains("PoisonMushroom"))
        {
            enabled = false;
            return;
        }

        damageTimer = interval * 0.5f;
        pulseTimer = Random.Range(0f, pulseInterval);
    }

    private void Update()
    {
        if (target == null)
        {
            target = FindObjectOfType<MipurinHealth>();
        }

        if (target == null || !target.IsAlive)
        {
            return;
        }

        UpdateAuraPulse();
        UpdateDamage();
    }

    public void Configure(MipurinHealth poisonTarget, int poisonDamage, float poisonRadius, float poisonInterval)
    {
        target = poisonTarget;
        damage = Mathf.Max(0, poisonDamage);
        radius = Mathf.Max(0.05f, poisonRadius);
        interval = Mathf.Max(0.1f, poisonInterval);
    }

    private void UpdateDamage()
    {
        damageTimer -= Time.deltaTime;

        if (damageTimer > 0f)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance > radius)
        {
            return;
        }

        Vector2 direction = (target.transform.position - transform.position).normalized;
        target.TakeDamage(damage, transform.position, direction);
        damageTimer = interval;
    }

    private void UpdateAuraPulse()
    {
        pulseTimer -= Time.deltaTime;

        if (pulseTimer > 0f)
        {
            return;
        }

        pulseTimer = pulseInterval;
        PlayAuraPulse();
    }

    private void PlayAuraPulse()
    {
        GameObject effectObject = new GameObject("PoisonMushroom_AuraPulse");
        effectObject.transform.position = transform.position;

        ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.25f;
        main.loop = false;
        main.startLifetime = 0.45f;
        main.startSpeed = 0.18f;
        main.startSize = 0.12f;
        main.startColor = auraColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)10) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius * 0.35f;

        ParticleSystemRenderer renderer = effectObject.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 15;
        }

        particles.Play();
        Destroy(effectObject, 0.7f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.55f, 0.15f, 0.9f, 0.45f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
