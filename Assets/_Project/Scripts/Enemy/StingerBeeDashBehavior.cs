using UnityEngine;

public class StingerBeeDashBehavior : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private MipurinHealth target;

    [Header("Dash")]
    [SerializeField] private float prepareTime = 0.35f;
    [SerializeField] private float dashDuration = 0.32f;
    [SerializeField] private float dashSpeed = 3.8f;
    [SerializeField] private float dashInterval = 1.8f;
    [SerializeField] private float minDashDistance = 0.55f;

    [Header("Visual")]
    [SerializeField] private Color warningColor = new Color(1f, 0.75f, 0.08f, 0.65f);

    private MipurinEnemy enemy;
    private Vector2 dashDirection;
    private float timer;
    private float dashTimer;
    private bool preparing;
    private bool dashing;

    private void Awake()
    {
        enemy = GetComponent<MipurinEnemy>();
        timer = Random.Range(0.4f, dashInterval);
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

        if (dashing)
        {
            UpdateDash();
            return;
        }

        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            return;
        }

        Vector2 toTarget = target.transform.position - transform.position;
        if (toTarget.magnitude < minDashDistance)
        {
            timer = 0.35f;
            return;
        }

        if (!preparing)
        {
            preparing = true;
            dashDirection = toTarget.normalized;
            dashTimer = prepareTime;
            PlayWarningPulse();
            return;
        }

        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f)
        {
            preparing = false;
            dashing = true;
            dashTimer = dashDuration;
        }
    }

    public void Configure(MipurinHealth dashTarget, float speed, float interval)
    {
        target = dashTarget;
        dashSpeed = Mathf.Max(0.1f, speed);
        dashInterval = Mathf.Max(0.2f, interval);
    }

    private void UpdateDash()
    {
        transform.position += (Vector3)(dashDirection * dashSpeed * Time.deltaTime);
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            dashing = false;
            timer = dashInterval;
        }
    }

    private void PlayWarningPulse()
    {
        GameObject effectObject = new GameObject("StingerBee_DashWarning");
        effectObject.transform.position = transform.position;

        ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.2f;
        main.loop = false;
        main.startLifetime = 0.25f;
        main.startSpeed = 0.55f;
        main.startSize = 0.08f;
        main.startColor = warningColor;
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
            renderer.sortingOrder = 18;
        }

        particles.Play();
        Destroy(effectObject, 0.45f);
    }
}
