using UnityEngine;

public class PlayerSpriteAnimator : MonoBehaviour
{
    private enum VisualState
    {
        Normal,
        Attack,
        Hurt,
        Down
    }

    [Header("References")]
    [SerializeField] private SpriteRenderer bodyRenderer;

    [Header("Loop Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;

    [Header("Action Sprites")]
    [SerializeField] private Sprite[] attackSprites;
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private Sprite downSprite;

    [Header("Animation")]
    [SerializeField] private float idleFps = 2f;
    [SerializeField] private float walkFps = 8f;
    [SerializeField] private float attackFps = 12f;
    [SerializeField] private float hurtDuration = 0.35f;
    [SerializeField] private float movementThreshold = 0.0001f;

    private Vector3 previousPosition;
    private float timer;
    private int frameIndex;
    private bool wasMoving;
    private VisualState visualState = VisualState.Normal;
    private float stateTimer;

    public bool IsLocked => visualState == VisualState.Down;

    private void Reset()
    {
        TryAutoFindBodyRenderer();
    }

    private void Awake()
    {
        TryAutoFindBodyRenderer();
        previousPosition = transform.position;
        ApplyLoopFrame(false, true);
    }

    private void OnEnable()
    {
        previousPosition = transform.position;
        timer = 0f;
        frameIndex = 0;
        wasMoving = false;
        visualState = VisualState.Normal;
        stateTimer = 0f;
        ApplyLoopFrame(false, true);
    }

    private void Update()
    {
        if (bodyRenderer == null)
        {
            return;
        }

        if (visualState == VisualState.Down)
        {
            ApplyDownFrame();
            return;
        }

        if (visualState == VisualState.Attack)
        {
            UpdateAttackState();
            return;
        }

        if (visualState == VisualState.Hurt)
        {
            UpdateHurtState();
            return;
        }

        UpdateNormalLoop();
    }

    public void Configure(SpriteRenderer targetBodyRenderer, Sprite[] idleFrames, Sprite[] walkFrames)
    {
        Configure(targetBodyRenderer, idleFrames, walkFrames, attackSprites, hurtSprite, downSprite);
    }

    public void Configure(
        SpriteRenderer targetBodyRenderer,
        Sprite[] idleFrames,
        Sprite[] walkFrames,
        Sprite[] attackFrames,
        Sprite targetHurtSprite,
        Sprite targetDownSprite)
    {
        bodyRenderer = targetBodyRenderer;
        idleSprites = idleFrames;
        walkSprites = walkFrames;
        attackSprites = attackFrames;
        hurtSprite = targetHurtSprite;
        downSprite = targetDownSprite;
        frameIndex = 0;
        timer = 0f;
        stateTimer = 0f;
        visualState = VisualState.Normal;

        ApplyLoopFrame(false, true);
    }

    public void PlayAttack()
    {
        if (visualState == VisualState.Down)
        {
            return;
        }

        if (attackSprites == null || attackSprites.Length == 0)
        {
            return;
        }

        visualState = VisualState.Attack;
        frameIndex = 0;
        timer = 0f;
        stateTimer = 0f;
        ApplyActionFrame(attackSprites, true);
    }

    public void PlayHurt(float duration)
    {
        if (visualState == VisualState.Down)
        {
            return;
        }

        visualState = VisualState.Hurt;
        frameIndex = 0;
        timer = 0f;
        stateTimer = Mathf.Max(0.05f, duration);

        if (hurtSprite != null)
        {
            bodyRenderer.sprite = hurtSprite;
        }
    }

    public void PlayDown()
    {
        visualState = VisualState.Down;
        frameIndex = 0;
        timer = 0f;
        stateTimer = 0f;
        ApplyDownFrame();
    }

    private void UpdateNormalLoop()
    {
        bool isMoving = (transform.position - previousPosition).sqrMagnitude > movementThreshold;
        previousPosition = transform.position;

        Sprite[] currentSprites = isMoving ? walkSprites : idleSprites;
        float currentFps = isMoving ? walkFps : idleFps;

        if (currentSprites == null || currentSprites.Length == 0)
        {
            return;
        }

        if (isMoving != wasMoving)
        {
            frameIndex = 0;
            timer = 0f;
            wasMoving = isMoving;
            ApplyLoopFrame(isMoving, true);
            return;
        }

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(1f, currentFps);

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            frameIndex = (frameIndex + 1) % currentSprites.Length;
            ApplyLoopFrame(isMoving, false);
        }
    }

    private void UpdateAttackState()
    {
        if (attackSprites == null || attackSprites.Length == 0)
        {
            ReturnToNormal();
            return;
        }

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(1f, attackFps);

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            frameIndex++;

            if (frameIndex >= attackSprites.Length)
            {
                ReturnToNormal();
                return;
            }

            ApplyActionFrame(attackSprites, true);
        }
    }

    private void UpdateHurtState()
    {
        stateTimer -= Time.deltaTime;

        if (hurtSprite != null)
        {
            bodyRenderer.sprite = hurtSprite;
        }

        if (stateTimer <= 0f)
        {
            ReturnToNormal();
        }
    }

    private void ReturnToNormal()
    {
        visualState = VisualState.Normal;
        frameIndex = 0;
        timer = 0f;
        previousPosition = transform.position;
        ApplyLoopFrame(false, true);
    }

    private void ApplyLoopFrame(bool isMoving, bool force)
    {
        if (bodyRenderer == null)
        {
            return;
        }

        Sprite[] currentSprites = isMoving ? walkSprites : idleSprites;

        if (currentSprites == null || currentSprites.Length == 0)
        {
            return;
        }

        frameIndex = Mathf.Clamp(frameIndex, 0, currentSprites.Length - 1);

        if (force || bodyRenderer.sprite != currentSprites[frameIndex])
        {
            bodyRenderer.sprite = currentSprites[frameIndex];
        }
    }

    private void ApplyActionFrame(Sprite[] sprites, bool force)
    {
        if (bodyRenderer == null || sprites == null || sprites.Length == 0)
        {
            return;
        }

        frameIndex = Mathf.Clamp(frameIndex, 0, sprites.Length - 1);

        if (force || bodyRenderer.sprite != sprites[frameIndex])
        {
            bodyRenderer.sprite = sprites[frameIndex];
        }
    }

    private void ApplyDownFrame()
    {
        if (bodyRenderer != null && downSprite != null)
        {
            bodyRenderer.sprite = downSprite;
        }
    }

    private void TryAutoFindBodyRenderer()
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
