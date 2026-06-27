using UnityEngine;

public class CharacterTestWaveHud : MonoBehaviour
{
    [SerializeField] private CharacterTestEnemySpawner spawner;
    [SerializeField] private NectarWallet nectarWallet;
    [SerializeField] private int fontSize = 18;

    private GUIStyle labelStyle;
    private GUIStyle messageStyle;

    private void Update()
    {
        if (spawner == null)
        {
            spawner = FindObjectOfType<CharacterTestEnemySpawner>();
        }

        if (nectarWallet == null)
        {
            nectarWallet = FindObjectOfType<NectarWallet>();
        }
    }

    private void OnGUI()
    {
        EnsureStyles();

        if (spawner == null)
        {
            return;
        }

        float y = 124f;
        GUI.Label(new Rect(24f, y, 360f, 24f), "Wave: " + spawner.CurrentWave + " / " + spawner.MaxWave, labelStyle);
        GUI.Label(new Rect(24f, y + 24f, 360f, 24f), "Alive: " + spawner.AliveEnemyCount, labelStyle);

        if (nectarWallet != null)
        {
            GUI.Label(new Rect(24f, y + 48f, 360f, 24f), "Goal: Nectar " + nectarWallet.Nectar + " / " + spawner.NectarGoal, labelStyle);
        }

        if (spawner.WaitingForNextWave)
        {
            GUI.Label(new Rect(24f, y + 72f, 360f, 24f), "Next Wave: " + spawner.NextWaveTimer.ToString("0.0") + "s", labelStyle);
        }

        if (spawner.StageCleared)
        {
            GUI.Label(new Rect(Screen.width / 2f - 180f, 52f, 360f, 44f), "TEST COMPLETE", messageStyle);
            GUI.Label(new Rect(Screen.width / 2f - 180f, 94f, 360f, 30f), "R: restart", messageStyle);
        }
    }

    public void Configure(CharacterTestEnemySpawner targetSpawner, NectarWallet wallet)
    {
        spawner = targetSpawner;
        nectarWallet = wallet;
    }

    private void EnsureStyles()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = fontSize;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.normal.textColor = new Color(0.22f, 0.12f, 0.04f, 1f);
        }

        if (messageStyle == null)
        {
            messageStyle = new GUIStyle(GUI.skin.label);
            messageStyle.alignment = TextAnchor.MiddleCenter;
            messageStyle.fontSize = 30;
            messageStyle.fontStyle = FontStyle.Bold;
            messageStyle.normal.textColor = new Color(0.95f, 0.48f, 0.05f, 1f);
        }
    }
}
