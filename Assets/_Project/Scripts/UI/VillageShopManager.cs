using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

public class VillageShopManager : MonoBehaviour
{
    private static VillageShopManager instance;

    private const int SmallHoneyBottlePrice = 8;
    private const int PollenCandyPrice = 12;
    private const int WaxShieldPrice = 15;

    private bool isOpen;
    private string message = "1 / 2 / 3 またはボタンで購入できます。";

    private Canvas canvas;
    private GameObject root;
    private Text honeyText;
    private Text messageText;
    private Text smallBottleCountText;
    private Text pollenCandyCountText;
    private Text waxShieldCountText;
    private Font uiFont;

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

        Render();
    }

    public void OpenShop()
    {
        EnsureUi();
        EnsureEventSystem();

        isOpen = true;
        message = "いらっしゃい、ミプリンちゃん。今日は仮ショップだよ。";

        if (root != null)
        {
            root.SetActive(true);
        }

        Render();
    }

    public void CloseShop()
    {
        isOpen = false;

        if (root != null)
        {
            root.SetActive(false);
        }
    }

    private void Render()
    {
        if (root == null)
        {
            return;
        }

        HoneyWallet wallet = HoneyWallet.Instance;
        VillageInventory inventory = VillageInventory.Instance;

        honeyText.text = "所持 Honey  " + wallet.Honey;
        smallBottleCountText.text = "所持 x" + inventory.SmallHoneyBottleCount;
        pollenCandyCountText.text = "所持 x" + inventory.PollenCandyCount;
        waxShieldCountText.text = "所持 x" + inventory.WaxShieldCount;
        messageText.text = message;
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

    private void EnsureUi()
    {
        if (root != null)
        {
            return;
        }

        GameObject canvasObject = new GameObject("VillageShopCanvas");
        canvasObject.transform.SetParent(transform, false);
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 80;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        root = new GameObject("VillageShopRoot");
        root.transform.SetParent(canvasObject.transform, false);

        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        GameObject dim = CreatePanel(root.transform, "DimBackground", new Color(0f, 0f, 0f, 0.36f));
        SetRect(dim.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject panel = CreatePanel(root.transform, "ShopPanel", new Color(1f, 0.89f, 0.58f, 0.97f));
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(860f, 610f);

        Text titleText = CreateText(panel.transform, "TitleText", 38, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.18f, 0.08f, 0.02f, 1f));
        titleText.text = "商人マルシェのショップ";
        SetRect(titleText.rectTransform, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.98f), Vector2.zero, Vector2.zero);

        honeyText = CreateText(panel.transform, "HoneyText", 28, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.28f, 0.12f, 0.02f, 1f));
        SetRect(honeyText.rectTransform, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.86f), Vector2.zero, Vector2.zero);

        smallBottleCountText = CreateShopRow(panel.transform, 0.62f, "1", "小さなはちみつ瓶", "HP回復アイテム（仮所持）", SmallHoneyBottlePrice, BuySmallHoneyBottle);
        pollenCandyCountText = CreateShopRow(panel.transform, 0.44f, "2", "花粉キャンディ", "次の冒険用バフ候補（仮所持）", PollenCandyPrice, BuyPollenCandy);
        waxShieldCountText = CreateShopRow(panel.transform, 0.26f, "3", "ワックスシールド", "被ダメ軽減バフ候補（仮所持）", WaxShieldPrice, BuyWaxShield);

        messageText = CreateText(panel.transform, "MessageText", 24, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.20f, 0.08f, 0.02f, 1f));
        messageText.resizeTextForBestFit = true;
        messageText.resizeTextMinSize = 18;
        messageText.resizeTextMaxSize = 24;
        SetRect(messageText.rectTransform, new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.19f), Vector2.zero, Vector2.zero);

        Text hintText = CreateText(panel.transform, "HintText", 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.30f, 0.15f, 0.04f, 1f));
        hintText.text = "1 / 2 / 3：購入    Esc / X：閉じる";
        SetRect(hintText.rectTransform, new Vector2(0.08f, 0.02f), new Vector2(0.92f, 0.08f), Vector2.zero, Vector2.zero);

        root.SetActive(false);
    }

    private Text CreateShopRow(Transform parent, float anchorY, string key, string itemName, string description, int price, UnityEngine.Events.UnityAction buyAction)
    {
        GameObject rowPanel = CreatePanel(parent, "ShopRow_" + key, new Color(1f, 0.98f, 0.80f, 0.86f));
        SetRect(rowPanel.GetComponent<RectTransform>(), new Vector2(0.07f, anchorY), new Vector2(0.93f, anchorY + 0.13f), Vector2.zero, Vector2.zero);

        Text itemText = CreateText(rowPanel.transform, "ItemText", 25, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.16f, 0.07f, 0.02f, 1f));
        itemText.text = key + ". " + itemName + "    " + price + " Honey";
        SetRect(itemText.rectTransform, new Vector2(0.04f, 0.48f), new Vector2(0.67f, 0.95f), Vector2.zero, Vector2.zero);

        Text descText = CreateText(rowPanel.transform, "DescriptionText", 18, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.30f, 0.16f, 0.04f, 1f));
        descText.text = description;
        SetRect(descText.rectTransform, new Vector2(0.04f, 0.06f), new Vector2(0.67f, 0.50f), Vector2.zero, Vector2.zero);

        Text countText = CreateText(rowPanel.transform, "CountText", 19, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.20f, 0.08f, 0.02f, 1f));
        SetRect(countText.rectTransform, new Vector2(0.66f, 0.12f), new Vector2(0.80f, 0.88f), Vector2.zero, Vector2.zero);

        Button button = CreateButton(rowPanel.transform, "BuyButton", "購入", buyAction);
        SetRect(button.GetComponent<RectTransform>(), new Vector2(0.82f, 0.16f), new Vector2(0.96f, 0.84f), Vector2.zero, Vector2.zero);

        return countText;
    }

    private Button CreateButton(Transform parent, string objectName, string label, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(objectName);
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.88f, 0.55f, 0.16f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.88f, 0.55f, 0.16f, 1f);
        colors.highlightedColor = new Color(1f, 0.68f, 0.22f, 1f);
        colors.pressedColor = new Color(0.72f, 0.38f, 0.10f, 1f);
        button.colors = colors;

        Text buttonText = CreateText(buttonObject.transform, "ButtonText", 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        buttonText.text = label;
        SetRect(buttonText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        return button;
    }

    private GameObject CreatePanel(Transform parent, string objectName, Color color)
    {
        GameObject panel = new GameObject(objectName);
        panel.transform.SetParent(parent, false);
        panel.AddComponent<RectTransform>();
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private Text CreateText(Transform parent, string objectName, int fontSize, FontStyle fontStyle, TextAnchor alignment, Color color)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        Text text = textObject.AddComponent<Text>();
        text.font = GetUiFont();
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        Outline outline = textObject.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.98f, 0.78f, 0.70f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return text;
    }

    private Font GetUiFont()
    {
        if (uiFont == null)
        {
            uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        return uiFont;
    }

    private void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
        eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
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
}
