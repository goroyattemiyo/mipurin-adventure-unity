using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageHudManager : MonoBehaviour
{
    private static VillageHudManager instance;

    private GUIStyle labelStyle;
    private GUIStyle smallStyle;

    public static VillageHudManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VillageHudManager>();
            }

            if (instance == null)
            {
                GameObject hudObject = new GameObject("VillageHudManager");
                instance = hudObject.AddComponent<VillageHudManager>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name != "VillageTest")
        {
            return;
        }

        if (VillageShopManager.Instance.IsOpen)
        {
            return;
        }

        EnsureStyles();

        HoneyWallet wallet = HoneyWallet.Instance;
        VillageInventory inventory = VillageInventory.Instance;
        StoryProgress storyProgress = StoryProgress.Instance;

        GUI.Label(new Rect(22f, 18f, 420f, 28f), "Honey: " + wallet.Honey, labelStyle);
        GUI.Label(new Rect(22f, 46f, 520f, 24f), "Story: " + storyProgress.CurrentStage, smallStyle);
        GUI.Label(new Rect(22f, 70f, 620f, 24f), "Items: はちみつ瓶 x" + inventory.SmallHoneyBottleCount + " / 花粉キャンディ x" + inventory.PollenCandyCount + " / ワックスシールド x" + inventory.WaxShieldCount, smallStyle);
    }

    private void EnsureStyles()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 22;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.normal.textColor = new Color(0.22f, 0.12f, 0.04f, 1f);
        }

        if (smallStyle == null)
        {
            smallStyle = new GUIStyle(GUI.skin.label);
            smallStyle.fontSize = 16;
            smallStyle.fontStyle = FontStyle.Bold;
            smallStyle.normal.textColor = new Color(0.22f, 0.12f, 0.04f, 1f);
        }
    }
}
