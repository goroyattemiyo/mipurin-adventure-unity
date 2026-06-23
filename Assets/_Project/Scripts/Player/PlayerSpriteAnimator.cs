using UnityEngine;

public class PlayerSpriteAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer bodyRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;

    [Header("Animation")]
    [SerializeField] private float idleFps = 2f;
    [SerializeField] private float walkFps = 8f;
    [SerializeField] private float movementThreshold = 0.0001f;

    private Vector3 previousPosition;
    private float timer;
    private int frameIndex;
    private bool wasMoving;

    private void Reset()
    {
        TryAutoFindBodyRenderer();
    }

    private void Awake()
    {
        TryAutoFindBodyRenderer();
        previousPosition = transform.position;
        ApplyFrame(false, true);
    }

    private void OnEnable()
    {
        previousPosition = transform.position;
        timer = 0f;
        frameIndex = 0;
        wasMoving = false;
        ApplyFrame(false, true);
    }

    private void Update()
    {
        bool isMoving = (transform.position - previousPosition).sqrMagnitude > movementThreshold;
        previousPosition = transform.position;

        Sprite[] currentSprites = isMoving ? walkSprites : idleSprites;
        float currentFps = isMoving ? walkFps : idleFps;

        if (currentSprites == null || currentSprites.Length == 0 || bodyRenderer == null)
        {
            return;
        }

        if (isMoving != wasMoving)
        {
            frameIndex = 0;
            timer = 0f;
            wasMoving = isMoving;
            ApplyFrame(isMoving, true);
            return;
        }

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(1f, currentFps);

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            frameIndex = (frameIndex + 1) % currentSprites.Length;
            ApplyFrame(isMoving, false);
        }
    }

    public void Configure(SpriteRenderer targetBodyRenderer, Sprite[] idleFrames, Sprite[] walkFrames)
    {
        bodyRenderer = targetBodyRenderer;
        idleSprites = idleFrames;
        walkSprites = walkFrames;
        frameIndex = 0;
        timer = 0f;
        ApplyFrame(false, true);
    }

    private void ApplyFrame(bool isMoving, bool force)
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
