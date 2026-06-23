using System.Collections;
using UnityEngine;

public class SpriteBlink : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer[] renderers;

    [Header("Blink")]
    [SerializeField] private Color blinkColor = Color.red;
    [SerializeField] private float blinkDuration = 0.08f;

    private Coroutine routine;

    private void Reset()
    {
        AutoCollectRenderers();
    }

    private void Awake()
    {
        AutoCollectRenderers();
    }

    public void Blink()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(BlinkRoutine());
    }

    public void Configure(SpriteRenderer[] targetRenderers, Color color, float duration)
    {
        renderers = targetRenderers;
        blinkColor = color;
        blinkDuration = duration;
    }

    private IEnumerator BlinkRoutine()
    {
        AutoCollectRenderers();

        if (renderers == null || renderers.Length == 0)
        {
            yield break;
        }

        Color[] originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
            {
                continue;
            }

            originalColors[i] = renderers[i].color;
            renderers[i].color = blinkColor;
        }

        yield return new WaitForSeconds(blinkDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
            {
                continue;
            }

            renderers[i].color = originalColors[i];
        }

        routine = null;
    }

    private void AutoCollectRenderers()
    {
        if (renderers != null && renderers.Length > 0)
        {
            return;
        }

        renderers = GetComponentsInChildren<SpriteRenderer>();
    }
}
