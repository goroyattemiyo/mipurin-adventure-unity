using UnityEngine;

public class EnemySpriteAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private Sprite downSprite;

    [Header("Timing")]
    [SerializeField] private float idleFps = 2f;
    [SerializeField] private float hurtDuration = 0.18f;

    private int currentFrame;
    private float frameTimer;
    private float hurtTimer;
    private bool isDown;

    private void Reset()
    {
        AutoFindRenderer();
    }

    private void Awake()
    {
        AutoFindRenderer();
        ApplyIdleFrame(0);
    }

    private void Update()
    {
        if (spriteRenderer == null || isDown)
        {
            return;
        }

        if (hurtTimer > 0f)
        {
            hurtTimer -= Time.deltaTime;

            if (hurtTimer <= 0f)
            {
                ApplyIdleFrame(currentFrame);
            }

            return;
        }

        UpdateIdleAnimation();
    }

    public void Configure(SpriteRenderer targetRenderer, Sprite[] targetIdleSprites, Sprite targetHurtSprite, Sprite targetDownSprite, float targetIdleFps, float targetHurtDuration)
    {
        spriteRenderer = targetRenderer;
        idleSprites = targetIdleSprites;
        hurtSprite = targetHurtSprite;
        downSprite = targetDownSprite;
        idleFps = Mathf.Max(0.1f, targetIdleFps);
        hurtDuration = Mathf.Max(0.01f, targetHurtDuration);
        isDown = false;
        hurtTimer = 0f;
        currentFrame = 0;
        frameTimer = 0f;

        AutoFindRenderer();
        ApplyIdleFrame(0);
    }

    public void PlayHurt()
    {
        if (spriteRenderer == null || isDown)
        {
            return;
        }

        if (hurtSprite != null)
        {
            spriteRenderer.sprite = hurtSprite;
        }

        hurtTimer = hurtDuration;
    }

    public void PlayDown()
    {
        isDown = true;
        hurtTimer = 0f;

        if (spriteRenderer != null && downSprite != null)
        {
            spriteRenderer.sprite = downSprite;
        }
    }

    public void PlayIdle()
    {
        if (isDown)
        {
            return;
        }

        hurtTimer = 0f;
        ApplyIdleFrame(currentFrame);
    }

    private void UpdateIdleAnimation()
    {
        if (idleSprites == null || idleSprites.Length == 0)
        {
            return;
        }

        if (idleSprites.Length == 1)
        {
            ApplyIdleFrame(0);
            return;
        }

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(0.1f, idleFps);

        if (frameTimer < frameDuration)
        {
            return;
        }

        frameTimer -= frameDuration;
        currentFrame = (currentFrame + 1) % idleSprites.Length;
        ApplyIdleFrame(currentFrame);
    }

    private void ApplyIdleFrame(int frameIndex)
    {
        if (spriteRenderer == null || idleSprites == null || idleSprites.Length == 0)
        {
            return;
        }

        int safeIndex = Mathf.Clamp(frameIndex, 0, idleSprites.Length - 1);
        Sprite sprite = idleSprites[safeIndex];

        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
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
