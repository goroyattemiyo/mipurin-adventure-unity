using UnityEngine;

public class PickupFloatingText : MonoBehaviour
{
    [SerializeField] private string message = "Nectar +1";
    [SerializeField] private Color textColor = new Color(0.95f, 0.48f, 0.05f, 1f);
    [SerializeField] private float duration = 0.75f;
    [SerializeField] private float riseSpeed = 0.65f;
    [SerializeField] private int fontSize = 20;

    private Vector3 worldPosition;
    private float timer;
    private GUIStyle labelStyle;

    public static void Show(Vector3 position, string text)
    {
        GameObject textObject = new GameObject("PickupFloatingText");
        PickupFloatingText floatingText = textObject.AddComponent<PickupFloatingText>();
        floatingText.Configure(position, text);
    }

    public void Configure(Vector3 position, string text)
    {
        worldPosition = position + new Vector3(0f, 0.35f, 0f);
        message = text;
        timer = duration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        worldPosition += Vector3.up * riseSpeed * Time.deltaTime;

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnGUI()
    {
        if (Camera.main == null || string.IsNullOrEmpty(message))
        {
            return;
        }

        EnsureStyle();

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        if (screenPosition.z < 0f)
        {
            return;
        }

        float alpha = Mathf.Clamp01(timer / duration);
        Color originalColor = labelStyle.normal.textColor;
        labelStyle.normal.textColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        Rect rect = new Rect(screenPosition.x - 80f, Screen.height - screenPosition.y - 20f, 160f, 32f);
        GUI.Label(rect, message, labelStyle);

        labelStyle.normal.textColor = originalColor;
    }

    private void EnsureStyle()
    {
        if (labelStyle != null)
        {
            return;
        }

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = fontSize,
            fontStyle = FontStyle.Bold,
            normal = { textColor = textColor }
        };
    }
}
