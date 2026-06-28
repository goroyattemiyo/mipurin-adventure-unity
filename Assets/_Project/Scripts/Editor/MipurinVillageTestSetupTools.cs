using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MipurinVillageTestSetupTools
{
    private const string VillageScenePath = "Assets/_Project/Scenes/VillageTest.unity";
    private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player_Mipurin.prefab";
    private const string HatchPrefabPath = "Assets/_Project/Prefabs/NPC/NPC_Hatch.prefab";
    private const string DialogueCanvasPrefabPath = "Assets/_Project/Prefabs/UI/DialogueCanvas.prefab";
    private const string HatchSpritePath = "Assets/_Project/Sprites/NPC/Hatch/npc_hatch_placeholder.png";
    private const string GroundSpritePath = "Assets/_Project/Sprites/Village/village_ground_placeholder.png";

    [MenuItem("Mipurin/Setup/Setup Village Test")]
    public static void SetupVillageTest()
    {
        EnsureProjectFolders();
        CreatePlaceholderSprites();
        CreateOrUpdateHatchPrefab();
        CreateOrUpdateDialogueCanvasPrefab();
        CreateOrUpdateVillageScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("VillageTest setup complete. Open Assets/_Project/Scenes/VillageTest.unity and press Play.");
    }

    private static void EnsureProjectFolders()
    {
        EnsureFolder("Assets/_Project/Scenes");
        EnsureFolder("Assets/_Project/Scripts/NPC");
        EnsureFolder("Assets/_Project/Scripts/UI");
        EnsureFolder("Assets/_Project/Prefabs/NPC");
        EnsureFolder("Assets/_Project/Prefabs/UI");
        EnsureFolder("Assets/_Project/Sprites/NPC");
        EnsureFolder("Assets/_Project/Sprites/NPC/Hatch");
        EnsureFolder("Assets/_Project/Sprites/Village");
    }

    private static void CreatePlaceholderSprites()
    {
        CreateHatchSpriteIfNeeded();
        CreateGroundSpriteIfNeeded();
    }

    private static void CreateHatchSpriteIfNeeded()
    {
        if (File.Exists(ToAbsolutePath(HatchSpritePath)))
        {
            ForceImportSprite(HatchSpritePath, 120f);
            return;
        }

        const int size = 96;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color32 clear = new Color32(0, 0, 0, 0);
        Color32 body = new Color32(165, 103, 42, 255);
        Color32 face = new Color32(255, 218, 107, 255);
        Color32 beard = new Color32(245, 235, 198, 255);
        Color32 robe = new Color32(104, 66, 35, 255);
        Color32 eye = new Color32(65, 36, 18, 255);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        FillEllipse(texture, 48, 56, 28, 30, body);
        FillEllipse(texture, 48, 58, 22, 22, face);
        FillEllipse(texture, 48, 38, 26, 18, robe);
        FillEllipse(texture, 48, 47, 18, 11, beard);
        FillEllipse(texture, 39, 61, 3, 4, eye);
        FillEllipse(texture, 57, 61, 3, 4, eye);
        FillEllipse(texture, 32, 76, 4, 10, body);
        FillEllipse(texture, 64, 76, 4, 10, body);

        texture.Apply(false, false);
        File.WriteAllBytes(ToAbsolutePath(HatchSpritePath), texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        ForceImportSprite(HatchSpritePath, 120f);
    }

    private static void CreateGroundSpriteIfNeeded()
    {
        if (File.Exists(ToAbsolutePath(GroundSpritePath)))
        {
            ForceImportSprite(GroundSpritePath, 16f);
            return;
        }

        Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color32 grassA = new Color32(182, 222, 149, 255);
        Color32 grassB = new Color32(164, 207, 132, 255);

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                texture.SetPixel(x, y, (x + y) % 2 == 0 ? grassA : grassB);
            }
        }

        texture.Apply(false, false);
        File.WriteAllBytes(ToAbsolutePath(GroundSpritePath), texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        ForceImportSprite(GroundSpritePath, 16f);
    }

    private static void CreateOrUpdateHatchPrefab()
    {
        Sprite hatchSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HatchSpritePath);
        GameObject root = new GameObject("NPC_Hatch");
        root.transform.localScale = Vector3.one;

        SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
        renderer.sprite = hatchSprite;
        renderer.sortingOrder = 3;

        NPCInteractable interactable = root.AddComponent<NPCInteractable>();
        interactable.Configure(
            "長老ハッチ",
            new[]
            {
                "ミプリンよ、南の森に黄金蜂蜜のかけらがあるという噂じゃ。",
                "気をつけて行くのじゃぞ。",
                "この村の未来は、おぬしの小さな羽にかかっておる。"
            },
            1.25f);

        PrefabUtility.SaveAsPrefabAsset(root, HatchPrefabPath);
        Object.DestroyImmediate(root);
    }

    private static void CreateOrUpdateDialogueCanvasPrefab()
    {
        Font font = GetBuiltInUiFont();

        GameObject canvasObject = new GameObject("DialogueCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        DialogueManager dialogueManager = canvasObject.AddComponent<DialogueManager>();

        GameObject dialogueRoot = CreatePanel("DialoguePanel", canvasObject.transform, new Color(0.14f, 0.09f, 0.04f, 0.88f));
        RectTransform dialogueRect = dialogueRoot.GetComponent<RectTransform>();
        dialogueRect.anchorMin = new Vector2(0.08f, 0.03f);
        dialogueRect.anchorMax = new Vector2(0.92f, 0.28f);
        dialogueRect.offsetMin = Vector2.zero;
        dialogueRect.offsetMax = Vector2.zero;

        Text speakerText = CreateText("SpeakerText", dialogueRoot.transform, font, 28, FontStyle.Bold, TextAnchor.UpperLeft, new Color(1f, 0.86f, 0.38f, 1f));
        SetRect(speakerText.rectTransform, new Vector2(0.04f, 0.65f), new Vector2(0.96f, 0.95f), Vector2.zero, Vector2.zero);

        Text bodyText = CreateText("BodyText", dialogueRoot.transform, font, 25, FontStyle.Normal, TextAnchor.MiddleLeft, Color.white);
        bodyText.horizontalOverflow = HorizontalWrapMode.Wrap;
        bodyText.verticalOverflow = VerticalWrapMode.Truncate;
        SetRect(bodyText.rectTransform, new Vector2(0.04f, 0.24f), new Vector2(0.96f, 0.68f), Vector2.zero, Vector2.zero);

        Text hintText = CreateText("HintText", dialogueRoot.transform, font, 18, FontStyle.Normal, TextAnchor.LowerRight, new Color(1f, 0.92f, 0.7f, 1f));
        SetRect(hintText.rectTransform, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.22f), Vector2.zero, Vector2.zero);

        GameObject promptRoot = CreatePanel("InteractionPrompt", canvasObject.transform, new Color(0.08f, 0.05f, 0.02f, 0.75f));
        RectTransform promptRect = promptRoot.GetComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.36f, 0.72f);
        promptRect.anchorMax = new Vector2(0.64f, 0.85f);
        promptRect.offsetMin = Vector2.zero;
        promptRect.offsetMax = Vector2.zero;

        Text promptText = CreateText("PromptText", promptRoot.transform, font, 21, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        SetRect(promptText.rectTransform, Vector2.zero, Vector2.one, new Vector2(12f, 8f), new Vector2(-12f, -8f));

        dialogueManager.ConfigureReferences(dialogueRoot, speakerText, bodyText, hintText, promptRoot, promptText);

        PrefabUtility.SaveAsPrefabAsset(canvasObject, DialogueCanvasPrefabPath);
        Object.DestroyImmediate(canvasObject);
    }

    private static void CreateOrUpdateVillageScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateVillageGround();
        InstantiatePlayer();
        InstantiateHatchNpc();
        InstantiateDialogueCanvas();

        EditorSceneManager.SaveScene(scene, VillageScenePath);
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 4f;
        camera.backgroundColor = new Color(0.73f, 0.9f, 1f, 1f);
        camera.clearFlags = CameraClearFlags.SolidColor;
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
    }

    private static void CreateVillageGround()
    {
        Sprite groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GroundSpritePath);
        GameObject ground = new GameObject("Village_Ground");
        SpriteRenderer renderer = ground.AddComponent<SpriteRenderer>();
        renderer.sprite = groundSprite;
        renderer.sortingOrder = -10;
        ground.transform.localScale = new Vector3(14f, 8f, 1f);
    }

    private static void InstantiatePlayer()
    {
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
        if (playerPrefab == null)
        {
            Debug.LogWarning("Player_Mipurin prefab not found: " + PlayerPrefabPath);
            return;
        }

        GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
        if (player == null)
        {
            return;
        }

        player.name = "Player_Mipurin";
        player.transform.position = new Vector3(0f, -1.2f, 0f);
    }

    private static void InstantiateHatchNpc()
    {
        GameObject hatchPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HatchPrefabPath);
        if (hatchPrefab == null)
        {
            Debug.LogWarning("NPC_Hatch prefab not found: " + HatchPrefabPath);
            return;
        }

        GameObject hatch = PrefabUtility.InstantiatePrefab(hatchPrefab) as GameObject;
        if (hatch == null)
        {
            return;
        }

        hatch.name = "NPC_Hatch";
        hatch.transform.position = new Vector3(2f, -0.45f, 0f);
    }

    private static void InstantiateDialogueCanvas()
    {
        GameObject canvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DialogueCanvasPrefabPath);
        if (canvasPrefab == null)
        {
            Debug.LogWarning("DialogueCanvas prefab not found: " + DialogueCanvasPrefabPath);
            return;
        }

        GameObject canvas = PrefabUtility.InstantiatePrefab(canvasPrefab) as GameObject;
        if (canvas != null)
        {
            canvas.name = "DialogueCanvas";
        }
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private static Text CreateText(string name, Transform parent, Font font, int fontSize, FontStyle fontStyle, TextAnchor alignment, Color color)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    private static Font GetBuiltInUiFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null)
        {
            return font;
        }

        Debug.LogWarning("LegacyRuntime.ttf was not found. Falling back to the default UI font if available.");
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
    }

    private static void FillEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY, Color32 color)
    {
        for (int y = centerY - radiusY; y <= centerY + radiusY; y++)
        {
            for (int x = centerX - radiusX; x <= centerX + radiusX; x++)
            {
                if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
                {
                    continue;
                }

                float normalizedX = (x - centerX) / (float)radiusX;
                float normalizedY = (y - centerY) / (float)radiusY;
                if (normalizedX * normalizedX + normalizedY * normalizedY <= 1f)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private static void ForceImportSprite(string path, float pixelsPerUnit)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string parent = Path.GetDirectoryName(folderPath).Replace("\\", "/");
        string folderName = Path.GetFileName(folderPath);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, folderName);
    }

    private static string ToAbsolutePath(string assetPath)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/");
        return Path.Combine(projectRoot, assetPath).Replace("\\", "/");
    }
}
