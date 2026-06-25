using UnityEngine;

public class MipurinEnemy : MonoBehaviour, IDamageable
{
    [Header("HP")]
    [SerializeField] private int maxHp = 3;

    [Header("Movement")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float stopDistance = 1.2f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private EnemySpriteAnimator spriteAnimator;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float idleFps = 2f;
    [SerializeField] private float hurtDuration = 0.18f;

    [Header("Hit Reaction")]
    [SerializeField] private float knockbackDistance = 0.35f;
    [SerializeField] private float destroyDelay = 0.2f;
    [SerializeField] private float blinkDuration = 0.08f;

    private int currentHp;
    private bool isDefeated;
    private SpriteBlink spriteBlink;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsAlive => !isDefeated;

    private void Reset()
    {
        AutoFindRenderer();
        AutoFindSpriteAnimator();
    }

    private void Awake()
    {
        AutoFindRenderer();
        AutoFindSpriteAnimator();
        spriteBlink = GetComponent<SpriteBlink>();

        if (spriteBlink == null)
        {
            spriteBlink = gameObject.AddComponent<SpriteBlink>();
        }

        ConfigureSpriteAnimator();
        currentHp = maxHp;
    }

    private void Update()
    {
        if (isDefeated || target == null || moveSpeed <= 0f)
        {
            return;
        }

        Vector2 toTarget = target.position - transform.position;

        if (toTarget.magnitude <= stopDistance)
        {
            return;
        }

        transform.position += (Vector3)(toTarget.normalized * moveSpeed * Time.deltaTime);
    }

    public void Configure(SpriteRenderer targetRenderer, Transform chaseTarget, Sprite targetHurtSprite, Sprite targetDownSprite)
    {
        spriteRenderer = targetRenderer;
        target = chaseTarget;
        hurtSprite = targetHurtSprite;
        downSprite = targetDownSprite;
        AutoFindRenderer();
        AutoFindSpriteAnimator();
        ConfigureSpriteAnimator();
    }

    public void ConfigureSprites(SpriteRenderer targetRenderer, Sprite[] targetIdleSprites, Sprite targetHurtSprite, Sprite targetDownSprite)
    {
        spriteRenderer = targetRenderer;
        idleSprites = targetIdleSprites;
        hurtSprite = targetHurtSprite;
        downSprite = targetDownSprite;
        AutoFindRenderer();
        AutoFindSpriteAnimator();
        ConfigureSpriteAnimator();
    }

    public void ConfigureHitReaction(float distance, float defeatDelay, Color blinkColor, float targetBlinkDuration)
    {
        knockbackDistance = Mathf.Max(0f, distance);
        destroyDelay = Mathf.Max(0f, defeatDelay);
        damageColor = blinkColor;
        blinkDuration = Mathf.Max(0.01f, targetBlinkDuration);
    }

    public void SetTarget(Transform chaseTarget)
    {
        target = chaseTarget;
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, transform.position, Vector2.zero);
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 knockbackDirection)
    {
        if (amount <= 0 || isDefeated)
        {
            return;
        }

        currentHp = Mathf.Max(0, currentHp - amount);

        ApplyKnockback(hitPoint, knockbackDirection);
        Blink();

        Debug.Log($"Enemy HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            Defeat();
        }
        else
        {
            PlayHurtVisual();
        }
    }

    private void ApplyKnockback(Vector2 hitPoint, Vector2 knockbackDirection)
    {
        Vector2 direction = knockbackDirection;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = (Vector2)transform.position - hitPoint;
        }

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.right;
        }

        transform.position += (Vector3)(direction.normalized * knockbackDistance);
    }

    private void Blink()
    {
        if (spriteBlink == null)
        {
            return;
        }

        spriteBlink.Configure(GetComponentsInChildren<SpriteRenderer>(), damageColor, blinkDuration);
        spriteBlink.Blink();
    }

    private void PlayHurtVisual()
    {
        if (spriteAnimator != null)
        {
            spriteAnimator.PlayHurt();
            return;
        }

        if (spriteRenderer != null && hurtSprite != null)
        {
            spriteRenderer.sprite = hurtSprite;
        }
    }

    private void Defeat()
    {
        isDefeated = true;

        if (spriteAnimator != null)
        {
            spriteAnimator.PlayDown();
            Destroy(gameObject, destroyDelay);
            return;
        }

        if (spriteRenderer != null && downSprite != null)
        {
            spriteRenderer.sprite = downSprite;
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ConfigureSpriteAnimator()
    {
        if (spriteAnimator == null)
        {
            return;
        }

        spriteAnimator.Configure(spriteRenderer, idleSprites, hurtSprite, downSprite, idleFps, hurtDuration);
    }

    private void AutoFindRenderer()
    {
        if (spriteRenderer != null)
        {
            return;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void AutoFindSpriteAnimator()
    {
        if (spriteAnimator != null)
        {
            return;
        }

        spriteAnimator = GetComponent<EnemySpriteAnimator>();

        if (spriteAnimator == null)
        {
            spriteAnimator = gameObject.AddComponent<EnemySpriteAnimator>();
        }
    }
}
