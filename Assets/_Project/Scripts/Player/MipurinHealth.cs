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

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsAlive => !isDown;
    public bool IsDown => isDown;

    private void Reset()
    {
        AutoFindBodyRenderer();
    }

    private void Awake()
    {
        AutoFindBodyRenderer();
        spriteBlink = GetComponent<SpriteBlink>();

        if (spriteBlink == null)
        {
            spriteBlink = gameObject.AddComponent<SpriteBlink>();
        }

        currentHp = maxHp;
    }

    public void Configure(SpriteRenderer targetBodyRenderer, Sprite targetHurtSprite, Sprite targetDownSprite)
    {
        bodyRenderer = targetBodyRenderer;
        hurtSprite = targetHurtSprite;
        downSprite = targetDownSprite;
        AutoFindBodyRenderer();
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

        if (bodyRenderer != null && hurtSprite != null)
        {
            bodyRenderer.sprite = hurtSprite;
        }

        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void Down()
    {
        isDown = true;

        if (bodyRenderer != null && downSprite != null)
        {
            bodyRenderer.sprite = downSprite;
        }

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        MipurinAttack attack = GetComponent<MipurinAttack>();
        if (attack != null) attack.enabled = false;

        PlayerSpriteAnimator bodyAnimator = GetComponent<PlayerSpriteAnimator>();
        if (bodyAnimator != null) bodyAnimator.enabled = false;

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
