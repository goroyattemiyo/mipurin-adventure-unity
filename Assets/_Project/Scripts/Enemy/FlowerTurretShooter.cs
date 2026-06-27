using UnityEngine;

public class FlowerTurretShooter : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private MipurinHealth target;

    [Header("Shot")]
    [SerializeField] private float fireInterval = 1.7f;
    [SerializeField] private float projectileSpeed = 2.2f;
    [SerializeField] private float projectileLifeTime = 4f;
    [SerializeField] private int projectileDamage = 1;

    [Header("Visual")]
    [SerializeField] private Color chargeColor = new Color(1f, 0.45f, 0.9f, 0.55f);

    private MipurinEnemy enemy;
    private float timer;

    private void Awake()
    {
        enemy = GetComponent<MipurinEnemy>();
        timer = Random.Range(0.4f, fireInterval);
    }

    private void Update()
    {
        if (enemy != null && !enemy.IsAlive)
        {
            return;
        }

        if (target == null)
        {
            target = FindObjectOfType<MipurinHealth>();
        }

        if (target == null || !target.IsAlive)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer > 0f)
        {
            return;
        }

        Fire();
        timer = fireInterval;
    }

    public void Configure(MipurinHealth shootTarget, float interval, float speed)
    {
        target = shootTarget;
        fireInterval = Mathf.Max(0.2f, interval);
        projectileSpeed = Mathf.Max(0.1f, speed);
    }

    private void Fire()
    {
        Vector2 direction = target.transform.position - transform.position;
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        direction.Normalize();
        PlayChargePulse();

        GameObject projectileObject = new GameObject("FlowerTurret_Projectile");
        projectileObject.transform.position = transform.position + (Vector3)(direction * 0.25f) + new Vector3(0f, 0.12f, 0f);

        EnemyProjectile projectile = projectileObject.AddComponent<EnemyProjectile>();
        projectile.Configure(direction, projectileDamage, projectileSpeed, projectileLifeTime);
    }

    private void PlayChargePulse()
    {
        GameObject effectObject = new GameObject("FlowerTurret_ShotPulse");
        effectObject.transform.position = transform.position;

        ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.18f;
        main.loop = false;
        main.startLifetime = 0.22f;
        main.startSpeed = 0.45f;
        main.startSize = 0.08f;
        main.startColor = chargeColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)8) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.12f;

        ParticleSystemRenderer renderer = effectObject.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 20;
        }

        particles.Play();
        Destroy(effectObject, 0.45f);
    }
}
