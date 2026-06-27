using UnityEngine;

public class MipurinDebugHud : MonoBehaviour
{
    [SerializeField] private MipurinHealth playerHealth;
    [SerializeField] private NectarWallet nectarWallet;
    [SerializeField] private MipurinEnemy enemy;
    [SerializeField] private MipurinEnemy[] enemies;
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

        if (nectarWallet == null)
        {
            nectarWallet = FindObjectOfType<NectarWallet>();
        }

        if (enemies == null || enemies.Length == 0)
        {
            enemies = FindObjectsOfType<MipurinEnemy>();
        }

        if (enemy == null && enemies != null && enemies.Length > 0)
        {
            enemy = enemies[0];
        }
    }

    private void OnGUI()
    {
        EnsureStyles();

        Rect panelRect = new Rect(12, 12, 520, 190);
        GUI.Box(panelRect, GUIContent.none, boxStyle);

        GUI.Label(new Rect(24, 20, 480, 24), "Move: WASD / Arrow keys    Attack: Space / Left Click", labelStyle);

        if (playerHealth != null)
        {
            GUI.Label(new Rect(24, 46, 300, 24), "Mipurin HP: " + playerHealth.CurrentHp + "/" + playerHealth.MaxHp, labelStyle);
            GUI.Label(new Rect(24, 72, 300, 24), "Mipurin State: " + playerHealth.StateLabel, labelStyle);
        }

        if (nectarWallet != null)
        {
            GUI.Label(new Rect(24, 98, 300, 24), "Nectar: " + nectarWallet.Nectar, labelStyle);
        }

        DrawEnemies(124);
    }

    public void Configure(MipurinHealth health, MipurinEnemy targetEnemy)
    {
        playerHealth = health;
        nectarWallet = health != null ? health.GetComponent<NectarWallet>() : null;
        enemy = targetEnemy;
        enemies = targetEnemy != null ? new[] { targetEnemy } : null;
    }

    public void ConfigureEnemies(MipurinHealth health, MipurinEnemy[] targetEnemies)
    {
        playerHealth = health;
        nectarWallet = health != null ? health.GetComponent<NectarWallet>() : null;
        enemies = targetEnemies;
        enemy = targetEnemies != null && targetEnemies.Length > 0 ? targetEnemies[0] : null;
    }

    public void ConfigureAppearance(Color newTextColor, Color newBackgroundColor, int newFontSize)
    {
        textColor = newTextColor;
        backgroundColor = newBackgroundColor;
        fontSize = Mathf.Max(10, newFontSize);
        labelStyle = null;
        boxStyle = null;
    }

    private void DrawEnemies(float startY)
    {
        if (enemies == null || enemies.Length == 0)
        {
            GUI.Label(new Rect(24, startY, 360, 24), "Enemies: not found", labelStyle);
            return;
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            MipurinEnemy targetEnemy = enemies[i];

            if (targetEnemy == null)
            {
                continue;
            }

            string line = targetEnemy.gameObject.name + " HP: " + targetEnemy.CurrentHp + "/" + targetEnemy.MaxHp;
            GUI.Label(new Rect(24, startY + i * 24f, 460, 24), line, labelStyle);
        }
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
