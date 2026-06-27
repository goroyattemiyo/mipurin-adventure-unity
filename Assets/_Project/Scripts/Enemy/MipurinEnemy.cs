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
    private EnemyDropper enemyDropper;

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
        enemyDropper = GetComponent<EnemyDropper>();

        if (spriteBlink == null)
        {
            spriteBlink = gameObject.AddComponent<SpriteBlink>();
        }

        ApplyPrototypeTraits();
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

    public void ConfigureHp(int targetMaxHp, bool refillHp = true)
    {
        maxHp = Mathf.Max(1, targetMaxHp);

        if (refillHp || currentHp <= 0 || currentHp > maxHp)
        {
            currentHp = maxHp;
            isDefeated = false;
        }
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

        Debug.Log($"{gameObject.name} HP: {currentHp}/{maxHp}");

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
        DisablePoisonAura();
        DropItem();

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

    private void DropItem()
    {
        if (enemyDropper == null)
        {
            enemyDropper = GetComponent<EnemyDropper>();
        }

        if (enemyDropper != null)
        {
            enemyDropper.Drop();
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

    private void ApplyPrototypeTraits()
    {
        string enemyName = gameObject.name;

        if (enemyName.Contains("HoneySlime"))
        {
            if (moveSpeed <= 0f)
            {
                moveSpeed = 0.58f;
            }

            stopDistance = Mathf.Min(stopDistance, 0.78f);
            idleFps = Mathf.Max(idleFps, 2.8f);
            damageColor = new Color(1f, 0.58f, 0.08f, 1f);
            knockbackDistance = Mathf.Max(knockbackDistance, 0.42f);
            destroyDelay = Mathf.Max(destroyDelay, 0.22f);
            ConfigureContactDamage(1, 1.35f, 0.72f);
        }
        else if (enemyName.Contains("PoisonMushroom"))
        {
            if (moveSpeed <= 0f)
            {
                moveSpeed = 0.2f;
            }

            stopDistance = Mathf.Min(stopDistance, 1.0f);
            idleFps = Mathf.Min(idleFps, 1.6f);
            damageColor = new Color(0.55f, 0.25f, 1f, 1f);
            knockbackDistance = Mathf.Max(knockbackDistance, 0.55f);
            destroyDelay = Mathf.Max(destroyDelay, 0.25f);
            ConfigureContactDamage(1, 1.9f, 0.78f);
            EnsurePoisonAura();
        }
    }

    private void ConfigureContactDamage(int damage, float interval, float radius)
    {
        MipurinContactDamage contactDamage = GetComponent<MipurinContactDamage>();
        if (contactDamage != null)
        {
            contactDamage.Configure(null, damage, interval, radius);
            contactDamage.ResetDamageTimer();
        }
    }

    private void EnsurePoisonAura()
    {
        PoisonMushroomAura aura = GetComponent<PoisonMushroomAura>();
        if (aura == null)
        {
            aura = gameObject.AddComponent<PoisonMushroomAura>();
        }

        aura.Configure(null, 1, 0.95f, 2.6f);
    }

    private void DisablePoisonAura()
    {
        PoisonMushroomAura aura = GetComponent<PoisonMushroomAura>();
        if (aura != null)
        {
            aura.enabled = false;
        }
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
