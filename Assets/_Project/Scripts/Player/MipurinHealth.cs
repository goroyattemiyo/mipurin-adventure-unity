using System.Collections;
using UnityEngine;

public class MipurinHealth : MonoBehaviour, IDamageable
{
    [Header("HP")]
    [SerializeField] private int maxHp = 5;
    [SerializeField] private float invincibleDuration = 0.5f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Color damageColor = new Color(1f, 0.16f, 0.12f, 1f);

    [Header("Hurt Feedback")]
    [SerializeField] private float hurtKnockbackDistance = 0.45f;
    [SerializeField] private float hurtBlinkDuration = 0.14f;
    [SerializeField] private Color hurtBurstColor = new Color(1f, 0.35f, 0.08f, 0.9f);

    private int currentHp;
    private bool isInvincible;
    private bool isDown;
    private SpriteBlink spriteBlink;
    private PlayerSpriteAnimator spriteAnimator;
    private Rigidbody2D rb;
    private string stateLabel = "Normal";

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsAlive => !isDown;
    public bool IsDown => isDown;
    public bool IsInvincible => isInvincible;
    public string StateLabel => stateLabel;

    private void Reset()
    {
        AutoFindBodyRenderer();
    }

    private void Awake()
    {
        AutoFindBodyRenderer();
        spriteAnimator = GetComponent<PlayerSpriteAnimator>();
        spriteBlink = GetComponent<SpriteBlink>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteBlink == null)
        {
            spriteBlink = gameObject.AddComponent<SpriteBlink>();
        }

        currentHp = maxHp;
        stateLabel = "Normal";
    }

    public void Configure(SpriteRenderer targetBodyRenderer, Sprite targetHurtSprite, Sprite targetDownSprite)
    {
        bodyRenderer = targetBodyRenderer;
        hurtSprite = targetHurtSprite;
        downSprite = targetDownSprite;
        AutoFindBodyRenderer();
        spriteAnimator = GetComponent<PlayerSpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, transform.position, Vector2.zero);
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 knockbackDirection)
    {
        if (amount <= 0 || isDown || isInvincible)
        {
            return;
        }

        currentHp = Mathf.Max(0, currentHp - amount);
        Debug.Log($"Mipurin HP: {currentHp}/{maxHp}");

        ApplyHurtKnockback(hitPoint, knockbackDirection);
        PlayHurtBurst();
        Blink();

        if (currentHp <= 0)
        {
            Down();
            return;
        }

        StartCoroutine(HurtRoutine());
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || isDown)
        {
            return;
        }

        currentHp = Mathf.Min(maxHp, currentHp + amount);
    }

    public void ResetHealth()
    {
        StopAllCoroutines();
        currentHp = maxHp;
        isDown = false;
        isInvincible = false;
        stateLabel = "Normal";
        RestoreNormalVisual();

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = true;

        MipurinAttack attack = GetComponent<MipurinAttack>();
        if (attack != null)
        {
            attack.enabled = true;
            attack.ApplyPowerLevel(0);
        }

        PlayerWingAnimator wingAnimator = GetComponent<PlayerWingAnimator>();
        if (wingAnimator != null) wingAnimator.enabled = true;
    }

    private IEnumerator HurtRoutine()
    {
        isInvincible = true;
        stateLabel = "Hurt";

        if (spriteAnimator != null)
        {
            spriteAnimator.PlayHurt(invincibleDuration);
        }
        else if (bodyRenderer != null && hurtSprite != null)
        {
            bodyRenderer.sprite = hurtSprite;
        }

        yield return new WaitForSeconds(invincibleDuration);

        isInvincible = false;

        if (!isDown)
        {
            stateLabel = "Normal";
        }
    }

    private void Down()
    {
        isDown = true;
        isInvincible = false;
        stateLabel = "Down";
        PlayDownBurst();

        if (spriteAnimator != null)
        {
            spriteAnimator.PlayDown();
        }
        else if (bodyRenderer != null && downSprite != null)
        {
            bodyRenderer.sprite = downSprite;
        }

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        MipurinAttack attack = GetComponent<MipurinAttack>();
        if (attack != null) attack.enabled = false;

        PlayerWingAnimator wingAnimator = GetComponent<PlayerWingAnimator>();
        if (wingAnimator != null) wingAnimator.enabled = false;

        Debug.Log("Mipurin down.");
    }

    private void RestoreNormalVisual()
    {
        if (spriteAnimator == null)
        {
            spriteAnimator = GetComponent<PlayerSpriteAnimator>();
        }

        if (spriteAnimator != null)
        {
            spriteAnimator.enabled = true;
            spriteAnimator.ResetToNormal();
        }
    }

    private void Blink()
    {
        if (spriteBlink == null)
        {
            return;
        }

        spriteBlink.Configure(GetComponentsInChildren<SpriteRenderer>(), damageColor, hurtBlinkDuration);
        spriteBlink.Blink();
    }

    private void ApplyHurtKnockback(Vector2 hitPoint, Vector2 knockbackDirection)
    {
        Vector2 direction = knockbackDirection;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = (Vector2)transform.position - hitPoint;
        }

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.down;
        }

        Vector2 nextPosition = (Vector2)transform.position + direction.normalized * hurtKnockbackDistance;

        if (rb != null)
        {
            rb.position = nextPosition;
        }
        else
        {
            transform.position = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
        }
    }

    private void PlayHurtBurst()
    {
        PlayBurst("MipurinHurt_Burst", hurtBurstColor, 10, 1.4f, 0.08f, 0.55f);
    }

    private void PlayDownBurst()
    {
        PlayBurst("MipurinDown_Burst", new Color(1f, 0.18f, 0.06f, 0.95f), 18, 1.9f, 0.12f, 0.75f);
    }

    private void PlayBurst(string effectName, Color color, short count, float speed, float size, float destroyAfter)
    {
        GameObject effectObject = new GameObject(effectName);
        effectObject.transform.position = transform.position + new Vector3(0f, 0.2f, 0f);

        ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.18f;
        main.loop = false;
        main.startLifetime = 0.24f;
        main.startSpeed = speed;
        main.startSize = size;
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, count) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.14f;

        ParticleSystemRenderer renderer = effectObject.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 40;
        }

        particles.Play();
        Destroy(effectObject, destroyAfter);
    }

    private void AutoFindBodyRenderer()
    {
        if (bodyRenderer != null)
        {
            return;
        }

        Transform body = transform.Find("Body");

        if (body != null)
        {
            bodyRenderer = body.GetComponent<SpriteRenderer>();
        }
    }
}
