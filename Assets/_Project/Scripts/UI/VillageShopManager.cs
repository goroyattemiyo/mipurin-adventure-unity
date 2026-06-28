using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class VillageShopManager : MonoBehaviour
{
    private static VillageShopManager instance;

    private const int SmallHoneyBottlePrice = 8;
    private const int PollenCandyPrice = 12;
    private const int WaxShieldPrice = 15;

    private bool isOpen;
    private string message = "1 / 2 / 3 またはボタンで購入できます。";
    private GUIStyle panelStyle;
    private GUIStyle titleStyle;
    private GUIStyle itemStyle;
    private GUIStyle buttonStyle;
    private GUIStyle hintStyle;
    private GUIStyle messageStyle;

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
            return;
        }

        if (WasBuyPressed(1))
        {
            BuySmallHoneyBottle();
        }
        else if (WasBuyPressed(2))
        {
            BuyPollenCandy();
        }
        else if (WasBuyPressed(3))
        {
            BuyWaxShield();
        }
    }

    private void OnGUI()
    {
        if (!isOpen)
        {
            return;
        }

        EnsureStyles();

        float width = 560f;
        float height = 390f;
        float x = Screen.width / 2f - width / 2f;
        float y = Screen.height / 2f - height / 2f;

        HoneyWallet wallet = HoneyWallet.Instance;
        VillageInventory inventory = VillageInventory.Instance;

        GUI.Box(new Rect(x, y, width, height), string.Empty, panelStyle);
        GUI.Label(new Rect(x + 28f, y + 20f, width - 56f, 38f), "商人マルシェのショップ", titleStyle);
        GUI.Label(new Rect(x + 28f, y + 58f, width - 56f, 26f), "所持Honey: " + wallet.Honey, itemStyle);

        DrawShopRow(x, y + 104f, "1", "小さなはちみつ瓶", "HP回復アイテム（仮所持）", SmallHoneyBottlePrice, inventory.SmallHoneyBottleCount, BuySmallHoneyBottle);
        DrawShopRow(x, y + 154f, "2", "花粉キャンディ", "次の冒険用バフ候補（仮所持）", PollenCandyPrice, inventory.PollenCandyCount, BuyPollenCandy);
        DrawShopRow(x, y + 204f, "3", "ワックスシールド", "被ダメ軽減バフ候補（仮所持）", WaxShieldPrice, inventory.WaxShieldCount, BuyWaxShield);

        GUI.Label(new Rect(x + 28f, y + 270f, width - 56f, 42f), message, messageStyle);
        GUI.Label(new Rect(x + 28f, y + 328f, width - 56f, 38f), "Esc / X：閉じる", hintStyle);
    }

    private void DrawShopRow(float x, float y, string key, string itemName, string description, int price, int count, System.Action buyAction)
    {
        GUI.Label(new Rect(x + 36f, y, 340f, 24f), key + ". " + itemName + "  " + price + " Honey", itemStyle);
        GUI.Label(new Rect(x + 58f, y + 22f, 340f, 22f), description + " / 所持 x" + count, hintStyle);

        if (GUI.Button(new Rect(x + 412f, y + 6f, 96f, 32f), "購入", buttonStyle))
        {
            buyAction?.Invoke();
        }
    }

    public void OpenShop()
    {
        isOpen = true;
        message = "いらっしゃい、ミプリンちゃん。今日は仮ショップだよ。";
    }

    public void CloseShop()
    {
        isOpen = false;
    }

    private void BuySmallHoneyBottle()
    {
        if (!TrySpend(SmallHoneyBottlePrice, "Honeyが足りないよ。小さなはちみつ瓶は " + SmallHoneyBottlePrice + " Honey だよ。"))
        {
            return;
        }

        VillageInventory.Instance.AddSmallHoneyBottle();
        message = "小さなはちみつ瓶を買った！ 回復アイテムの仮所持数が増えた。";
    }

    private void BuyPollenCandy()
    {
        if (!TrySpend(PollenCandyPrice, "Honeyが足りないよ。花粉キャンディは " + PollenCandyPrice + " Honey だよ。"))
        {
            return;
        }

        VillageInventory.Instance.AddPollenCandy();
        message = "花粉キャンディを買った！ バフ候補アイテムの仮所持数が増えた。";
    }

    private void BuyWaxShield()
    {
        if (!TrySpend(WaxShieldPrice, "Honeyが足りないよ。ワックスシールドは " + WaxShieldPrice + " Honey だよ。"))
        {
            return;
        }

        VillageInventory.Instance.AddWaxShield();
        message = "ワックスシールドを買った！ 防御アイテムの仮所持数が増えた。";
    }

    private bool TrySpend(int price, string failMessage)
    {
        HoneyWallet wallet = HoneyWallet.Instance;
        if (!wallet.TrySpend(price))
        {
            message = failMessage;
            return false;
        }

        return true;
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

    private bool WasBuyPressed(int number)
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (number == 1 && keyboard.digit1Key.wasPressedThisFrame)
            {
                return true;
            }

            if (number == 2 && keyboard.digit2Key.wasPressedThisFrame)
            {
                return true;
            }

            if (number == 3 && keyboard.digit3Key.wasPressedThisFrame)
            {
                return true;
            }
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (number == 1 && Input.GetKeyDown(KeyCode.Alpha1))
        {
            return true;
        }

        if (number == 2 && Input.GetKeyDown(KeyCode.Alpha2))
        {
            return true;
        }

        if (number == 3 && Input.GetKeyDown(KeyCode.Alpha3))
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
            itemStyle.fontSize = 18;
            itemStyle.fontStyle = FontStyle.Bold;
            itemStyle.alignment = TextAnchor.MiddleLeft;
            itemStyle.normal.textColor = new Color(0.18f, 0.1f, 0.04f, 1f);
        }

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
        }

        if (hintStyle == null)
        {
            hintStyle = new GUIStyle(GUI.skin.label);
            hintStyle.fontSize = 14;
            hintStyle.alignment = TextAnchor.MiddleLeft;
            hintStyle.normal.textColor = new Color(0.28f, 0.16f, 0.06f, 1f);
        }

        if (messageStyle == null)
        {
            messageStyle = new GUIStyle(GUI.skin.label);
            messageStyle.fontSize = 16;
            messageStyle.fontStyle = FontStyle.Bold;
            messageStyle.alignment = TextAnchor.MiddleCenter;
            messageStyle.normal.textColor = new Color(0.36f, 0.18f, 0.04f, 1f);
        }
    }
}
