using UnityEngine;

public class SimpleEffectPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Frames")]
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 16f;
    [SerializeField] private bool destroyOnEnd = true;
    [SerializeField] private float fallbackLifetime = 0.25f;

    private float timer;
    private int frameIndex;
    private float aliveTimer;

    private void Reset()
    {
        TryFindRenderer();
    }

    private void Awake()
    {
        TryFindRenderer();
    }

    private void OnEnable()
    {
        timer = 0f;
        frameIndex = 0;
        aliveTimer = 0f;
        ApplyFrame();
    }

    private void Update()
    {
        aliveTimer += Time.deltaTime;

        if (frames == null || frames.Length == 0)
        {
            if (destroyOnEnd && aliveTimer >= fallbackLifetime)
            {
                Destroy(gameObject);
            }

            return;
        }

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(1f, fps);

        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            frameIndex++;

            if (frameIndex >= frames.Length)
            {
                if (destroyOnEnd)
                {
                    Destroy(gameObject);
                    return;
                }

                frameIndex = 0;
            }

            ApplyFrame();
        }
    }

    public void Configure(SpriteRenderer renderer, Sprite[] animationFrames, float animationFps, bool shouldDestroyOnEnd, float lifetime)
    {
        targetRenderer = renderer;
        frames = animationFrames;
        fps = animationFps;
        destroyOnEnd = shouldDestroyOnEnd;
        fallbackLifetime = lifetime;
        frameIndex = 0;
        timer = 0f;
        aliveTimer = 0f;
        ApplyFrame();
    }

    private void ApplyFrame()
    {
        if (targetRenderer == null || frames == null || frames.Length == 0)
        {
            return;
        }

        frameIndex = Mathf.Clamp(frameIndex, 0, frames.Length - 1);
        targetRenderer.sprite = frames[frameIndex];
    }

    private void TryFindRenderer()
    {
        if (targetRenderer != null)
        {
            return;
        }

        targetRenderer = GetComponent<SpriteRenderer>();
    }
}
