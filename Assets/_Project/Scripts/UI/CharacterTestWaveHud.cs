using UnityEngine;

public class CharacterTestWaveHud : MonoBehaviour
{
    [SerializeField] private CharacterTestEnemySpawner spawner;
    [SerializeField] private NectarWallet nectarWallet;
    [SerializeField] private int fontSize = 18;

    private GUIStyle labelStyle;
    private GUIStyle messageStyle;
    private GUIStyle resultStyle;

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
            DrawClearResult();
        }
    }

    public void Configure(CharacterTestEnemySpawner targetSpawner, NectarWallet wallet)
    {
        spawner = targetSpawner;
        nectarWallet = wallet;
    }

    private void DrawClearResult()
    {
        int nectar = nectarWallet != null ? nectarWallet.Nectar : 0;
        float x = Screen.width / 2f - 190f;
        float y = Screen.height / 2f - 92f;

        GUI.Label(new Rect(x, y, 380f, 44f), "TEST COMPLETE", messageStyle);
        GUI.Label(new Rect(x, y + 46f, 380f, 26f), "Nectar: " + nectar, resultStyle);
        GUI.Label(new Rect(x, y + 72f, 380f, 26f), "Defeated: " + spawner.DefeatedEnemyCount, resultStyle);
        GUI.Label(new Rect(x, y + 98f, 380f, 26f), "Wave Reached: " + spawner.CurrentWave + " / " + spawner.MaxWave, resultStyle);
        GUI.Label(new Rect(x, y + 130f, 380f, 30f), "R: restart", resultStyle);
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

        if (resultStyle == null)
        {
            resultStyle = new GUIStyle(GUI.skin.label);
            resultStyle.alignment = TextAnchor.MiddleCenter;
            resultStyle.fontSize = 20;
            resultStyle.fontStyle = FontStyle.Bold;
            resultStyle.normal.textColor = new Color(0.22f, 0.12f, 0.04f, 1f);
        }
    }
}
