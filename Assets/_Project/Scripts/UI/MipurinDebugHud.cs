using UnityEngine;

public class MipurinDebugHud : MonoBehaviour
{
    [SerializeField] private MipurinHealth playerHealth;
    [SerializeField] private MipurinEnemy enemy;
    [SerializeField] private Color textColor = new Color(0.22f, 0.12f, 0.04f, 1f);
    [SerializeField] private Color backgroundColor = new Color(1f, 0.96f, 0.78f, 0.72f);
    [SerializeField] private int fontSize = 18;

    private GUIStyle labelStyle;
    private GUIStyle boxStyle;

    private void Update()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<MipurinHealth>();
        }

        if (enemy == null)
        {
            enemy = FindObjectOfType<MipurinEnemy>();
        }
    }

    private void OnGUI()
    {
        EnsureStyles();

        Rect panelRect = new Rect(12, 12, 470, 116);
        GUI.Box(panelRect, GUIContent.none, boxStyle);

        GUI.Label(new Rect(24, 20, 440, 24), "Move: WASD / Arrow keys    Attack: Space / Left Click", labelStyle);

        if (playerHealth != null)
        {
            GUI.Label(new Rect(24, 46, 260, 24), $"Mipurin HP: {playerHealth.CurrentHp}/{playerHealth.MaxHp}", labelStyle);
            GUI.Label(new Rect(24, 72, 260, 24), $"Mipurin State: {playerHealth.StateLabel}", labelStyle);
        }

        if (enemy != null)
        {
            GUI.Label(new Rect(24, 98, 220, 24), $"Enemy HP: {enemy.CurrentHp}/{enemy.MaxHp}", labelStyle);
        }
        else
        {
            GUI.Label(new Rect(24, 98, 260, 24), "Enemy: defeated or not found", labelStyle);
        }
    }

    public void Configure(MipurinHealth health, MipurinEnemy targetEnemy)
    {
        playerHealth = health;
        enemy = targetEnemy;
    }

    public void ConfigureAppearance(Color newTextColor, Color newBackgroundColor, int newFontSize)
    {
        textColor = newTextColor;
        backgroundColor = newBackgroundColor;
        fontSize = Mathf.Max(10, newFontSize);
        labelStyle = null;
        boxStyle = null;
    }

    private void EnsureStyles()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                normal = { textColor = textColor }
            };
        }

        if (boxStyle == null)
        {
            Texture2D backgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            backgroundTexture.SetPixel(0, 0, backgroundColor);
            backgroundTexture.Apply();

            boxStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = backgroundTexture }
            };
        }
    }
}
