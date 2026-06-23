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
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Color damageColor = Color.red;

    [Header("Hit Reaction")]
    [SerializeField] private float knockbackDistance = 0.25f;
    [SerializeField] private float destroyDelay = 0.2f;

    private int currentHp;
    private bool isDefeated;
    private SpriteBlink spriteBlink;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsAlive => !isDefeated;

    private void Reset()
    {
        AutoFindRenderer();
    }

    private void Awake()
    {
        AutoFindRenderer();
        spriteBlink = GetComponent<SpriteBlink>();

        if (spriteBlink == null)
        {
            spriteBlink = gameObject.AddComponent<SpriteBlink>();
        }

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

        if (knockbackDirection.sqrMagnitude > 0.01f)
        {
            transform.position += (Vector3)(knockbackDirection.normalized * knockbackDistance);
        }

        if (spriteBlink != null)
        {
            spriteBlink.Configure(GetComponentsInChildren<SpriteRenderer>(), damageColor, 0.08f);
            spriteBlink.Blink();
        }

        Debug.Log($"Enemy HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            Defeat();
        }
        else if (spriteRenderer != null && hurtSprite != null)
        {
            spriteRenderer.sprite = hurtSprite;
        }
    }

    private void Defeat()
    {
        isDefeated = true;

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

    private void AutoFindRenderer()
    {
        if (spriteRenderer != null)
        {
            return;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}
