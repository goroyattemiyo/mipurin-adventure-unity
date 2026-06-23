using UnityEngine;

public class PlayerWingAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer wingRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] flapSprites;

    [Header("Animation")]
    [SerializeField] private float idleFps = 3f;
    [SerializeField] private float flapFps = 12f;
    [SerializeField] private float movementThreshold = 0.0001f;

    private Vector3 previousPosition;
    private float timer;
    private int frameIndex;
    private bool wasMoving;

    private void Reset()
    {
        TryAutoFindWingRenderer();
    }

    private void Awake()
    {
        TryAutoFindWingRenderer();
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

        Sprite[] currentSprites = isMoving ? flapSprites : idleSprites;
        float currentFps = isMoving ? flapFps : idleFps;

        if (currentSprites == null || currentSprites.Length == 0 || wingRenderer == null)
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

    public void Configure(SpriteRenderer targetWingRenderer, Sprite[] idleFrames, Sprite[] flapFrames)
    {
        wingRenderer = targetWingRenderer;
        idleSprites = idleFrames;
        flapSprites = flapFrames;
        frameIndex = 0;
        timer = 0f;
        ApplyFrame(false, true);
    }

    private void ApplyFrame(bool isMoving, bool force)
    {
        if (wingRenderer == null)
        {
            return;
        }

        Sprite[] currentSprites = isMoving ? flapSprites : idleSprites;

        if (currentSprites == null || currentSprites.Length == 0)
        {
            return;
        }

        frameIndex = Mathf.Clamp(frameIndex, 0, currentSprites.Length - 1);

        if (force || wingRenderer.sprite != currentSprites[frameIndex])
        {
            wingRenderer.sprite = currentSprites[frameIndex];
        }
    }

    private void TryAutoFindWingRenderer()
    {
        if (wingRenderer != null)
        {
            return;
        }

        Transform wings = transform.Find("Wings");

        if (wings != null)
        {
            wingRenderer = wings.GetComponent<SpriteRenderer>();
        }
    }
}
