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
    [SerializeField] private Color damageColor = Color.red;

    private int currentHp;
    private bool isInvincible;
    private bool isDown;
    private SpriteBlink spriteBlink;
    private PlayerSpriteAnimator spriteAnimator;
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

        if (spriteBlink != null)
        {
            spriteBlink.Configure(GetComponentsInChildren<SpriteRenderer>(), damageColor, 0.08f);
            spriteBlink.Blink();
        }

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
