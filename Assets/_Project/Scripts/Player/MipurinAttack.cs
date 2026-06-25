using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MipurinAttack : MonoBehaviour
{
    private const float MaxAttackEffectScale = 0.1f;
    private const float MaxHitEffectScale = 0.25f;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRadius = 0.85f;
    [SerializeField] private float forwardOffset = 1.05f;
    [SerializeField] private float cooldown = 0.4f;
    [SerializeField] private LayerMask targetLayers = ~0;

    [Header("Effect")]
    [SerializeField] private GameObject attackEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float attackEffectScale = MaxAttackEffectScale;
    [SerializeField] private float hitEffectScale = MaxHitEffectScale;

    [Header("Debug")]
    [SerializeField] private bool drawAttackRange = true;

    private PlayerController controller;
    private PlayerSpriteAnimator spriteAnimator;
    private MipurinHealth health;
    private float cooldownTimer;

    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);
    public float Cooldown => cooldown;
    public float AttackRadius => attackRadius;
    public float ForwardOffset => forwardOffset;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        spriteAnimator = GetComponent<PlayerSpriteAnimator>();
        health = GetComponent<MipurinHealth>();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (health != null && health.IsDown)
        {
            return;
        }

        if (cooldownTimer > 0f)
        {
            return;
        }

        if (WasAttackPressed())
        {
            Attack();
        }
    }

    public void Configure(GameObject effectPrefab)
    {
        Configure(effectPrefab, hitEffectPrefab, attackRadius, forwardOffset);
    }

    public void Configure(GameObject effectPrefab, GameObject targetHitEffectPrefab, float radius, float offset)
    {
        Configure(effectPrefab, targetHitEffectPrefab, radius, offset, cooldown, attackEffectScale, hitEffectScale);
    }

    public void Configure(GameObject effectPrefab, GameObject targetHitEffectPrefab, float radius, float offset, float cooldownSeconds, float attackScale, float hitScale)
    {
        attackEffectPrefab = effectPrefab;
        hitEffectPrefab = targetHitEffectPrefab;
        attackRadius = Mathf.Max(0.05f, radius);
        forwardOffset = Mathf.Max(0f, offset);
        cooldown = Mathf.Max(0.05f, cooldownSeconds);
        attackEffectScale = Mathf.Clamp(attackScale, 0.01f, MaxAttackEffectScale);
        hitEffectScale = Mathf.Clamp(hitScale, 0.01f, MaxHitEffectScale);
    }

    private void Attack()
    {
        cooldownTimer = cooldown;

        if (spriteAnimator != null)
        {
            spriteAnimator.PlayAttack();
        }

        Vector2 direction = GetAttackDirection();
        Vector2 center = GetAttackCenter(direction);

        SpawnAttackEffect(center, direction);
        HitTargets(center, direction);
    }

    private Vector2 GetAttackDirection()
    {
        Vector2 direction = controller != null ? controller.LastMoveDirection : Vector2.down;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        direction.Normalize();
        return direction;
    }

    private Vector2 GetAttackCenter(Vector2 direction)
    {
        return (Vector2)transform.position + direction * forwardOffset;
    }

    private void SpawnAttackEffect(Vector2 position, Vector2 direction)
    {
        if (attackEffectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(attackEffectPrefab, position, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        effect.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        effect.transform.localScale = Vector3.one * Mathf.Clamp(attackEffectScale, 0.01f, MaxAttackEffectScale);
    }

    private void SpawnHitEffect(Vector2 position)
    {
        if (hitEffectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        effect.transform.localScale = Vector3.one * Mathf.Clamp(hitEffectScale, 0.01f, MaxHitEffectScale);
    }

    private void HitTargets(Vector2 center, Vector2 direction)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, targetLayers);

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                continue;
            }

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null || !damageable.IsAlive)
            {
                continue;
            }

            Vector2 hitPoint = hit.ClosestPoint(center);

            if ((hitPoint - center).sqrMagnitude < 0.001f)
            {
                hitPoint = hit.transform.position;
            }

            damageable.TakeDamage(damage, hitPoint, direction);
            SpawnHitEffect(hitPoint);
        }
    }

    private bool WasAttackPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        bool keyboardPressed = keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
        bool mousePressed = mouse != null && mouse.leftButton.wasPressedThisFrame;

        if (keyboardPressed || mousePressed)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            return true;
        }
#endif

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawAttackRange)
        {
            return;
        }

        Vector2 direction = GetAttackDirection();
        Vector2 center = GetAttackCenter(direction);

        Gizmos.color = new Color(1f, 0.85f, 0.1f, 0.65f);
        Gizmos.DrawWireSphere(center, attackRadius);
        Gizmos.DrawLine(transform.position, center);
    }
}
