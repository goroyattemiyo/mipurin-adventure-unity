using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float speed = 2.2f;
    [SerializeField] private float hitRadius = 0.18f;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private Color projectileColor = new Color(0.95f, 0.2f, 0.85f, 0.9f);

    private Vector2 direction = Vector2.down;
    private float age;
    private MipurinHealth target;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = projectileColor;
        spriteRenderer.sortingOrder = 22;
    }

    private void Update()
    {
        age += Time.deltaTime;
        if (age >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (target == null)
        {
            target = FindObjectOfType<MipurinHealth>();
        }

        if (target == null || !target.IsAlive)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= hitRadius)
        {
            target.TakeDamage(damage, transform.position, direction);
            Destroy(gameObject);
        }
    }

    public void Configure(Vector2 shotDirection, int projectileDamage, float projectileSpeed, float projectileLifeTime)
    {
        direction = shotDirection.sqrMagnitude > 0.01f ? shotDirection.normalized : Vector2.down;
        damage = Mathf.Max(0, projectileDamage);
        speed = Mathf.Max(0.1f, projectileSpeed);
        lifeTime = Mathf.Max(0.1f, projectileLifeTime);
    }

    private Sprite CreateCircleSprite()
    {
        const int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.42f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                Color color = distance <= radius ? Color.white : Color.clear;
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 32f);
    }
}
