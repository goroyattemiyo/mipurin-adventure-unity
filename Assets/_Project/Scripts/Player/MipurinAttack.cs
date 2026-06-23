using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MipurinAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRadius = 0.75f;
    [SerializeField] private float forwardOffset = 0.85f;
    [SerializeField] private float cooldown = 0.25f;

    [Header("Effect")]
    [SerializeField] private GameObject attackEffectPrefab;

    private PlayerController controller;
    private PlayerSpriteAnimator spriteAnimator;
    private MipurinHealth health;
    private float cooldownTimer;

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
        attackEffectPrefab = effectPrefab;
    }

    private void Attack()
    {
        cooldownTimer = cooldown;

        if (spriteAnimator != null)
        {
            spriteAnimator.PlayAttack();
        }

        Vector2 direction = controller != null ? controller.LastMoveDirection : Vector2.down;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        direction.Normalize();

        Vector2 center = (Vector2)transform.position + direction * forwardOffset;
        SpawnEffect(center, direction);
        HitTargets(center, direction);
    }

    private void SpawnEffect(Vector2 position, Vector2 direction)
    {
        if (attackEffectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(attackEffectPrefab, position, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        effect.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void HitTargets(Vector2 center, Vector2 direction)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius);

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

            damageable.TakeDamage(damage, center, direction);
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
        Vector2 direction = controller != null ? controller.LastMoveDirection : Vector2.down;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        direction.Normalize();
        Vector2 center = (Vector2)transform.position + direction * forwardOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
