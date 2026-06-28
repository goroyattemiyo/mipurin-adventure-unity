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
    private const string DialogueCanvasPrefabPath = "Assets/_Project/Prefabs/UI/DialogueCanvas.prefab";
    private const string GroundSpritePath = "Assets/_Project/Sprites/Village/village_ground_placeholder.png";

    private struct TalkTargetDefinition
    {
        public string ObjectName;
        public string DisplayName;
        public string PrefabPath;
        public string SpritePath;
        public Vector3 Position;
        public Vector3 Scale;
        public Color32 PrimaryColor;
        public Color32 SecondaryColor;
        public Color32 AccentColor;
        public bool IsObject;
        public string[] Lines;

        public TalkTargetDefinition(string objectName, string displayName, string prefabPath, string spritePath, Vector3 position, Vector3 scale, Color32 primaryColor, Color32 secondaryColor, Color32 accentColor, bool isObject, string[] lines)
        {
            ObjectName = objectName;
            DisplayName = displayName;
            PrefabPath = prefabPath;
            SpritePath = spritePath;
            Position = position;
            Scale = scale;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            AccentColor = accentColor;
            IsObject = isObject;
            Lines = lines;
        }
    }

    [MenuItem("Mipurin/Setup/Setup Village Test")]
    public static void SetupVillageTest()
    {
        EnsureProjectFolders();
        CreatePlaceholderSprites();
        CreateOrUpdateTalkTargetPrefabs();
        CreateOrUpdateDialogueCanvasPrefab();
        CreateOrUpdateVillageScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("VillageTest setup complete. Open Assets/_Project/Scenes/VillageTest.unity and press Play.");
    }

    private static TalkTargetDefinition[] GetTalkTargets()
    {
        return new[]
        {
            new TalkTargetDefinition(
                "NPC_Hatch",
                "長老ハッチ",
                "Assets/_Project/Prefabs/NPC/NPC_Hatch.prefab",
                "Assets/_Project/Sprites/NPC/Hatch/npc_hatch_placeholder.png",
                new Vector3(2.1f, -0.35f, 0f),
                Vector3.one,
                new Color32(165, 103, 42, 255),
                new Color32(255, 218, 107, 255),
                new Color32(245, 235, 198, 255),
                false,
                new[]
                {
                    "ミプリンよ、南の森に黄金蜂蜜のかけらがあるという噂じゃ。",
                    "気をつけて行くのじゃぞ。",
                    "この村の未来は、おぬしの小さな羽にかかっておる。"
                }),

            new TalkTargetDefinition(
                "NPC_Miel",
                "占い師ミエル",
                "Assets/_Project/Prefabs/NPC/NPC_Miel.prefab",
                "Assets/_Project/Sprites/NPC/Miel/npc_miel_placeholder.png",
                new Vector3(-2.55f, 1.05f, 0f),
                Vector3.one,
                new Color32(109, 83, 176, 255),
                new Color32(244, 223, 255, 255),
                new Color32(255, 213, 87, 255),
                false,
                new[]
                {
                    "女王さまの気配が、黄金蜂蜜の香りに隠れておるわ。",
                    "南の森へ向かうなら、光る花を目印にしなさい。",
                    "ミプリンの羽は、小さくても運命を動かす力があるの。"
                }),

            new TalkTargetDefinition(
                "NPC_Marche",
                "商人マルシェ",
                "Assets/_Project/Prefabs/NPC/NPC_Marche.prefab",
                "Assets/_Project/Sprites/NPC/Marche/npc_marche_placeholder.png",
                new Vector3(-0.95f, 0.65f, 0f),
                Vector3.one,
                new Color32(229, 129, 58, 255),
                new Color32(255, 228, 143, 255),
                new Color32(127, 76, 34, 255),
                false,
                new[]
                {
                    "いらっしゃい、ミプリンちゃん。今日はまだお店の準備中だよ。",
                    "森へ行くなら、はちみつポットを忘れずにね。",
                    "そのうち便利な道具も並べるから楽しみにしてて。"
                }),

            new TalkTargetDefinition(
                "NPC_Bee",
                "ビー",
                "Assets/_Project/Prefabs/NPC/NPC_Bee.prefab",
                "Assets/_Project/Sprites/NPC/Bee/npc_bee_placeholder.png",
                new Vector3(0.75f, 1.25f, 0f),
                new Vector3(0.92f, 0.92f, 1f),
                new Color32(252, 205, 64, 255),
                new Color32(90, 58, 31, 255),
                new Color32(255, 255, 255, 255),
                false,
                new[]
                {
                    "ぼくも森を見てきたけど、変なキノコが増えてたよ。",
                    "ミプリンならきっと大丈夫。無理だけはしないでね。"
                }),

            new TalkTargetDefinition(
                "NPC_Polle",
                "ポーレ",
                "Assets/_Project/Prefabs/NPC/NPC_Polle.prefab",
                "Assets/_Project/Sprites/NPC/Polle/npc_polle_placeholder.png",
                new Vector3(2.95f, 1.3f, 0f),
                new Vector3(0.9f, 0.9f, 1f),
                new Color32(232, 150, 218, 255),
                new Color32(255, 233, 250, 255),
                new Color32(143, 218, 112, 255),
                false,
                new[]
                {
                    "花粉の流れがいつもと違うの。",
                    "黄金蜂蜜のかけらは、花の記憶を呼び起こすって聞いたわ。"
                }),

            new TalkTargetDefinition(
                "NPC_Navii",
                "ナビィ",
                "Assets/_Project/Prefabs/NPC/NPC_Navii.prefab",
                "Assets/_Project/Sprites/NPC/Navii/npc_navii_placeholder.png",
                new Vector3(-3.25f, -1.1f, 0f),
                new Vector3(0.85f, 0.85f, 1f),
                new Color32(93, 171, 239, 255),
                new Color32(220, 242, 255, 255),
                new Color32(255, 239, 116, 255),
                false,
                new[]
                {
                    "困ったら村のみんなに話しかけてみよう。",
                    "EかSpaceで会話できるよ。今のうちに操作を覚えておくと安心だね。"
                }),

            new TalkTargetDefinition(
                "NPC_Grandpa",
                "グランパ",
                "Assets/_Project/Prefabs/NPC/NPC_Grandpa.prefab",
                "Assets/_Project/Sprites/NPC/Grandpa/npc_grandpa_placeholder.png",
                new Vector3(3.35f, -1.25f, 0f),
                new Vector3(1.05f, 1.05f, 1f),
                new Color32(128, 99, 72, 255),
                new Color32(237, 227, 200, 255),
                new Color32(99, 139, 83, 255),
                false,
                new[]
                {
                    "昔、この村の南には黄金色に光る森があったんじゃ。",
                    "女王さまが消えてから、森の奥が少しずつ変わってしもうた。"
                }),

            new TalkTargetDefinition(
                "Village_Board",
                "村の掲示板",
                "Assets/_Project/Prefabs/NPC/Village_Board.prefab",
                "Assets/_Project/Sprites/Village/village_board_placeholder.png",
                new Vector3(-3.15f, 2.45f, 0f),
                new Vector3(1.1f, 1.1f, 1f),
                new Color32(143, 91, 45, 255),
                new Color32(252, 226, 141, 255),
                new Color32(88, 54, 24, 255),
                true,
                new[]
                {
                    "現在の目標：南の森のうわさを調べる。",
                    "ショップ、図鑑、クエスト管理はまだ準備中。"
                }),

            new TalkTargetDefinition(
                "SouthForest_Gate",
                "南の森への道",
                "Assets/_Project/Prefabs/NPC/SouthForest_Gate.prefab",
                "Assets/_Project/Sprites/Village/south_forest_gate_placeholder.png",
                new Vector3(0f, 3.0f, 0f),
                new Vector3(1.2f, 1.2f, 1f),
                new Color32(82, 143, 75, 255),
                new Color32(55, 100, 59, 255),
                new Color32(255, 217, 73, 255),
                true,
                new[]
                {
                    "この先は南の森。",
                    "今はまだシーン移動は未実装です。",
                    "次のプロトタイプで森へ行けるようにします。"
                })
        };
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
        EnsureFolder("Assets/_Project/Sprites/NPC/Miel");
        EnsureFolder("Assets/_Project/Sprites/NPC/Marche");
        EnsureFolder("Assets/_Project/Sprites/NPC/Bee");
        EnsureFolder("Assets/_Project/Sprites/NPC/Polle");
        EnsureFolder("Assets/_Project/Sprites/NPC/Navii");
        EnsureFolder("Assets/_Project/Sprites/NPC/Grandpa");
        EnsureFolder("Assets/_Project/Sprites/Village");
    }

    private static void CreatePlaceholderSprites()
    {
        CreateGroundSpriteIfNeeded();

        foreach (TalkTargetDefinition target in GetTalkTargets())
        {
            CreateTalkTargetSpriteIfNeeded(target);
        }
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

    private static void CreateTalkTargetSpriteIfNeeded(TalkTargetDefinition target)
    {
        EnsureFolder(Path.GetDirectoryName(target.SpritePath).Replace("\\", "/"));

        if (File.Exists(ToAbsolutePath(target.SpritePath)))
        {
            ForceImportSprite(target.SpritePath, 120f);
            return;
        }

        const int size = 96;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color32 clear = new Color32(0, 0, 0, 0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        if (target.IsObject)
        {
            DrawObjectIcon(texture, target);
        }
        else
        {
            DrawNpcIcon(texture, target);
        }

        texture.Apply(false, false);
        File.WriteAllBytes(ToAbsolutePath(target.SpritePath), texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        ForceImportSprite(target.SpritePath, 120f);
    }

    private static void DrawNpcIcon(Texture2D texture, TalkTargetDefinition target)
    {
        Color32 eye = new Color32(50, 32, 20, 255);
        FillEllipse(texture, 48, 55, 26, 29, target.PrimaryColor);
        FillEllipse(texture, 48, 58, 20, 20, target.SecondaryColor);
        FillEllipse(texture, 48, 38, 24, 17, target.PrimaryColor);
        FillEllipse(texture, 48, 75, 17, 8, target.AccentColor);
        FillEllipse(texture, 39, 61, 3, 4, eye);
        FillEllipse(texture, 57, 61, 3, 4, eye);
        FillEllipse(texture, 35, 46, 5, 9, target.AccentColor);
        FillEllipse(texture, 61, 46, 5, 9, target.AccentColor);
    }

    private static void DrawObjectIcon(Texture2D texture, TalkTargetDefinition target)
    {
        FillRect(texture, 25, 24, 46, 42, target.PrimaryColor);
        FillRect(texture, 29, 31, 38, 27, target.SecondaryColor);
        FillRect(texture, 33, 36, 30, 5, target.AccentColor);
        FillRect(texture, 33, 47, 30, 5, target.AccentColor);
        FillRect(texture, 43, 12, 10, 18, target.PrimaryColor);

        if (target.ObjectName.Contains("Gate"))
        {
            FillEllipse(texture, 48, 50, 28, 24, target.PrimaryColor);
            FillEllipse(texture, 48, 48, 21, 17, target.SecondaryColor);
            FillRect(texture, 34, 21, 8, 31, target.PrimaryColor);
            FillRect(texture, 54, 21, 8, 31, target.PrimaryColor);
            FillEllipse(texture, 48, 67, 6, 6, target.AccentColor);
        }
    }

    private static void CreateOrUpdateTalkTargetPrefabs()
    {
        foreach (TalkTargetDefinition target in GetTalkTargets())
        {
            CreateOrUpdateTalkTargetPrefab(target);
        }
    }

    private static void CreateOrUpdateTalkTargetPrefab(TalkTargetDefinition target)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(target.SpritePath);
        GameObject root = new GameObject(target.ObjectName);
        root.transform.localScale = target.Scale;

        SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = target.IsObject ? 2 : 3;

        NPCInteractable interactable = root.AddComponent<NPCInteractable>();
        interactable.Configure(target.DisplayName, target.Lines, target.IsObject ? 1.35f : 1.25f);

        PrefabUtility.SaveAsPrefabAsset(root, target.PrefabPath);
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
        promptRect.anchorMin = new Vector2(0.35f, 0.72f);
        promptRect.anchorMax = new Vector2(0.65f, 0.86f);
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
        CreateVillageDecorations();
        InstantiatePlayer();
        InstantiateTalkTargets();
        InstantiateDialogueCanvas();

        EditorSceneManager.SaveScene(scene, VillageScenePath);
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 4.4f;
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
        ground.transform.localScale = new Vector3(16f, 9f, 1f);
    }

    private static void CreateVillageDecorations()
    {
        Sprite groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GroundSpritePath);
        GameObject root = new GameObject("Village_Decorations");

        CreateDecorRect(root.transform, groundSprite, "Village_MainRoad", new Vector3(0f, 0.4f, 0f), new Vector3(1.7f, 6.4f, 1f), new Color(0.74f, 0.55f, 0.32f, 1f), -6);
        CreateDecorRect(root.transform, groundSprite, "Village_CrossRoad", new Vector3(0f, 0.1f, 0f), new Vector3(7.5f, 1.15f, 1f), new Color(0.78f, 0.59f, 0.36f, 1f), -5);
        CreateDecorRect(root.transform, groundSprite, "Elder_House", new Vector3(2.1f, 0.65f, 0f), new Vector3(1.65f, 1.05f, 1f), new Color(0.83f, 0.57f, 0.27f, 1f), -2);
        CreateDecorRect(root.transform, groundSprite, "Marche_Tent", new Vector3(-0.95f, 1.55f, 0f), new Vector3(1.75f, 0.95f, 1f), new Color(0.95f, 0.58f, 0.28f, 1f), -2);
        CreateDecorRect(root.transform, groundSprite, "Miel_Tent", new Vector3(-2.55f, 1.95f, 0f), new Vector3(1.45f, 0.95f, 1f), new Color(0.54f, 0.39f, 0.83f, 1f), -2);
        CreateDecorRect(root.transform, groundSprite, "Honeycomb_House_A", new Vector3(3.35f, -0.35f, 0f), new Vector3(1.35f, 1.0f, 1f), new Color(0.92f, 0.73f, 0.28f, 1f), -2);
        CreateDecorRect(root.transform, groundSprite, "FlowerBed_Left", new Vector3(-3.5f, -2.2f, 0f), new Vector3(1.7f, 0.5f, 1f), new Color(0.96f, 0.55f, 0.8f, 1f), -1);
        CreateDecorRect(root.transform, groundSprite, "FlowerBed_Right", new Vector3(3.0f, 2.25f, 0f), new Vector3(1.5f, 0.45f, 1f), new Color(0.92f, 0.64f, 0.95f, 1f), -1);
    }

    private static void CreateDecorRect(Transform parent, Sprite sprite, string name, Vector3 position, Vector3 scale, Color color, int sortingOrder)
    {
        GameObject decor = new GameObject(name);
        decor.transform.SetParent(parent);
        decor.transform.position = position;
        decor.transform.localScale = scale;
        SpriteRenderer renderer = decor.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
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
        player.transform.position = new Vector3(0f, -1.7f, 0f);
        DisableVillageCombatComponents(player);
    }

    private static void DisableVillageCombatComponents(GameObject player)
    {
        MonoBehaviour[] behaviours = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == null)
            {
                continue;
            }

            string typeName = behaviour.GetType().Name;
            if (typeName == "MipurinAttack" || typeName == "PlayerAttack")
            {
                behaviour.enabled = false;
            }
        }
    }

    private static void InstantiateTalkTargets()
    {
        foreach (TalkTargetDefinition target in GetTalkTargets())
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(target.PrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning("Talk target prefab not found: " + target.PrefabPath);
                continue;
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
            {
                continue;
            }

            instance.name = target.ObjectName;
            instance.transform.position = target.Position;
            instance.transform.localScale = target.Scale;
        }
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

    private static void FillRect(Texture2D texture, int startX, int startY, int width, int height, Color32 color)
    {
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
                {
                    continue;
                }

                texture.SetPixel(x, y, color);
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
        if (string.IsNullOrWhiteSpace(folderPath) || AssetDatabase.IsValidFolder(folderPath))
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
