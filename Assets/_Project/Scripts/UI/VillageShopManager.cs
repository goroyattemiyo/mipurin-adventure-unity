using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class VillageShopManager : MonoBehaviour
{
    private static VillageShopManager instance;

    private bool isOpen;
    private GUIStyle panelStyle;
    private GUIStyle titleStyle;
    private GUIStyle itemStyle;
    private GUIStyle hintStyle;

    public static VillageShopManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VillageShopManager>();
            }

            if (instance == null)
            {
                GameObject shopObject = new GameObject("VillageShopManager");
                instance = shopObject.AddComponent<VillageShopManager>();
            }

            return instance;
        }
    }

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        if (!isOpen)
        {
            return;
        }

        if (WasClosePressed())
        {
            CloseShop();
        }
    }

    private void OnGUI()
    {
        if (!isOpen)
        {
            return;
        }

        EnsureStyles();

        float width = 520f;
        float height = 310f;
        float x = Screen.width / 2f - width / 2f;
        float y = Screen.height / 2f - height / 2f;

        GUI.Box(new Rect(x, y, width, height), string.Empty, panelStyle);
        GUI.Label(new Rect(x + 28f, y + 22f, width - 56f, 38f), "商人マルシェの仮ショップ", titleStyle);
        GUI.Label(new Rect(x + 36f, y + 74f, width - 72f, 34f), "小さなはちみつ瓶    HPを少し回復（準備中）", itemStyle);
        GUI.Label(new Rect(x + 36f, y + 122f, width - 72f, 34f), "花粉キャンディ      次の冒険だけ攻撃速度UP（準備中）", itemStyle);
        GUI.Label(new Rect(x + 36f, y + 170f, width - 72f, 34f), "ワックスシールド    次の冒険で1回だけ被ダメ軽減（準備中）", itemStyle);
        GUI.Label(new Rect(x + 28f, y + 235f, width - 56f, 48f), "今回は表示テストです。購入処理と通貨消費は次の段階で追加します。\nEsc / X：閉じる", hintStyle);
    }

    public void OpenShop()
    {
        isOpen = true;
    }

    public void CloseShop()
    {
        isOpen = false;
    }

    private bool WasClosePressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.escapeKey.wasPressedThisFrame || keyboard.xKey.wasPressedThisFrame))
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X))
        {
            return true;
        }
#endif

        return false;
    }

    private void EnsureStyles()
    {
        if (panelStyle == null)
        {
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = Texture2D.whiteTexture;
            panelStyle.normal.textColor = Color.white;
        }

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 28;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = new Color(0.22f, 0.12f, 0.04f, 1f);
        }

        if (itemStyle == null)
        {
            itemStyle = new GUIStyle(GUI.skin.label);
            itemStyle.fontSize = 20;
            itemStyle.fontStyle = FontStyle.Bold;
            itemStyle.alignment = TextAnchor.MiddleLeft;
            itemStyle.normal.textColor = new Color(0.18f, 0.1f, 0.04f, 1f);
        }

        if (hintStyle == null)
        {
            hintStyle = new GUIStyle(GUI.skin.label);
            hintStyle.fontSize = 16;
            hintStyle.alignment = TextAnchor.MiddleCenter;
            hintStyle.normal.textColor = new Color(0.28f, 0.16f, 0.06f, 1f);
        }
    }
}
