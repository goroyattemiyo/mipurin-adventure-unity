using UnityEngine;

public class PlayerBoundsLimiter : MonoBehaviour
{
    [Header("Stage Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 minPosition = new Vector2(-8.5f, -4.5f);
    [SerializeField] private Vector2 maxPosition = new Vector2(8.5f, 4.5f);

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        ClampToBounds();
    }

    public void Configure(Vector2 min, Vector2 max, bool enabled)
    {
        minPosition = min;
        maxPosition = max;
        useBounds = enabled;
        ClampToBounds();
    }

    private void ClampToBounds()
    {
        if (!useBounds)
        {
            return;
        }

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minPosition.x, maxPosition.x);
        position.y = Mathf.Clamp(position.y, minPosition.y, maxPosition.y);
        transform.position = position;

        if (rb != null)
        {
            rb.position = new Vector2(position.x, position.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!useBounds)
        {
            return;
        }

        Vector3 center = new Vector3(
            (minPosition.x + maxPosition.x) * 0.5f,
            (minPosition.y + maxPosition.y) * 0.5f,
            transform.position.z
        );

        Vector3 size = new Vector3(
            maxPosition.x - minPosition.x,
            maxPosition.y - minPosition.y,
            0f
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
