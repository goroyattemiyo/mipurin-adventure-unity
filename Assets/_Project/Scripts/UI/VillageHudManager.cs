using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VillageHudManager : MonoBehaviour
{
    private static VillageHudManager instance;

    private Canvas canvas;
    private GameObject root;
    private Text honeyText;
    private Text storyText;
    private Text itemsText;
    private Font uiFont;

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

    private void Update()
    {
        EnsureUi();

        bool shouldShow = SceneManager.GetActiveScene().name == "VillageTest" && !VillageShopManager.Instance.IsOpen;
        if (root != null && root.activeSelf != shouldShow)
        {
            root.SetActive(shouldShow);
        }

        if (shouldShow)
        {
            Render();
        }
    }

    private void Render()
    {
        HoneyWallet wallet = HoneyWallet.Instance;
        VillageInventory inventory = VillageInventory.Instance;
        StoryProgress storyProgress = StoryProgress.Instance;

        honeyText.text = "Honey  " + wallet.Honey;
        storyText.text = "Story  " + storyProgress.CurrentStage;
        itemsText.text = "Items  はちみつ瓶 x" + inventory.SmallHoneyBottleCount
            + "   花粉キャンディ x" + inventory.PollenCandyCount
            + "   ワックスシールド x" + inventory.WaxShieldCount;
    }

    private void EnsureUi()
    {
        if (root != null)
        {
            return;
        }

        GameObject canvasObject = new GameObject("VillageHudCanvas");
        canvasObject.transform.SetParent(transform, false);
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 40;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        root = new GameObject("VillageHudRoot");
        root.transform.SetParent(canvasObject.transform, false);

        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        GameObject panel = CreatePanel(root.transform, "VillageHudPanel", new Color(1f, 0.86f, 0.48f, 0.88f));
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(24f, -22f);
        panelRect.sizeDelta = new Vector2(760f, 128f);

        honeyText = CreateText(panel.transform, "HoneyText", 30, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.18f, 0.09f, 0.02f, 1f));
        SetRect(honeyText.rectTransform, new Vector2(0.035f, 0.58f), new Vector2(0.97f, 0.95f), Vector2.zero, Vector2.zero);

        storyText = CreateText(panel.transform, "StoryText", 22, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.22f, 0.12f, 0.04f, 1f));
        SetRect(storyText.rectTransform, new Vector2(0.035f, 0.34f), new Vector2(0.97f, 0.60f), Vector2.zero, Vector2.zero);

        itemsText = CreateText(panel.transform, "ItemsText", 20, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.22f, 0.12f, 0.04f, 1f));
        SetRect(itemsText.rectTransform, new Vector2(0.035f, 0.08f), new Vector2(0.97f, 0.35f), Vector2.zero, Vector2.zero);
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
        outline.effectColor = new Color(1f, 0.98f, 0.78f, 0.72f);
        outline.effectDistance = new Vector2(1.4f, -1.4f);

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
}
