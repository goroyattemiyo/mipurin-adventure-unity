using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinFieldSetupTools
{
    private const string FieldRootName = "Field_TestRoot";
    private const string OldStageRootName = "Stage_TestRoot";

    private const string FloorSpritePath1 = "Assets/_Project/Sprites/Stage/Floors/floor_soft_grass_01.png";
    private const string FloorSpritePath2 = "Assets/_Project/Sprites/Stage/Floors/floor_soft_grass_02.png";
    private const string BorderSpritePath = "Assets/_Project/Sprites/Stage/Borders/stage_border_flower_01.png";
    private const string CornerSpritePath = "Assets/_Project/Sprites/Stage/Borders/stage_corner_flower_01.png";

    private static readonly Vector3 PlayerSpawn = new Vector3(0f, -1.2f, 0f);
    private static readonly Vector3 HoneySlimeSpawn = new Vector3(2.5f, 0.35f, 0f);
    private static readonly Vector3 PoisonMushroomSpawn = new Vector3(-2.5f, 0.35f, 0f);

    private static readonly Vector3 HoneySlimeScale = new Vector3(0.4f, 0.4f, 1f);
    private static readonly Vector3 PoisonMushroomScale = new Vector3(0.6f, 0.6f, 1f);
    private static readonly Vector3 FloorScale = new Vector3(0.28f, 0.28f, 1f);
    private static readonly Vector3 BorderScale = new Vector3(0.22f, 0.22f, 1f);
    private static readonly Vector3 CornerScale = new Vector3(0.22f, 0.22f, 1f);

    private static readonly Vector2 PlayerMin = new Vector2(-5.4f, -3.0f);
    private static readonly Vector2 PlayerMax = new Vector2(5.4f, 3.0f);
    private static readonly Vector2 CameraMin = new Vector2(-1.4f, -0.4f);
    private static readonly Vector2 CameraMax = new Vector2(1.4f, 0.4f);

    [MenuItem("Mipurin/Setup/Setup Flower Field Test")]
    public static void SetupFlowerFieldTest()
    {
        HideOldStageRoot();
        DeleteExistingFieldRoot();

        GameObject root = new GameObject(FieldRootName);
        Transform groundRoot = CreateChild(root.transform, "Ground");
        Transform borderRoot = CreateChild(root.transform, "Borders");
        Transform spawnRoot = CreateChild(root.transform, "SpawnPoints");

        CreateGround(groundRoot);
        CreateBorders(borderRoot);
        CreateSpawnPoint(spawnRoot, "PlayerSpawn", PlayerSpawn);
        CreateSpawnPoint(spawnRoot, "HoneySlimeSpawn", HoneySlimeSpawn);
        CreateSpawnPoint(spawnRoot, "PoisonMushroomSpawn", PoisonMushroomSpawn);

        ConfigureActors();
        ConfigureCamera();

        EditorUtility.SetDirty(root);
        EditorSceneManager.MarkSceneDirty(root.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Flower field test setup complete.");
    }

    private static void HideOldStageRoot()
    {
        GameObject oldStage = GameObject.Find(OldStageRootName);
        if (oldStage == null)
        {
            return;
        }

        oldStage.SetActive(false);
        EditorUtility.SetDirty(oldStage);
    }

    private static void DeleteExistingFieldRoot()
    {
        GameObject oldRoot = GameObject.Find(FieldRootName);
        if (oldRoot != null)
        {
            Object.DestroyImmediate(oldRoot);
        }
    }

    private static void CreateGround(Transform parent)
    {
        Sprite floorA = LoadSprite(FloorSpritePath1);
        Sprite floorB = LoadSprite(FloorSpritePath2);

        const int columns = 9;
        const int rows = 6;
        const float stepX = 1.15f;
        const float stepY = 0.92f;
        float startX = -(columns - 1) * stepX * 0.5f;
        float startY = -(rows - 1) * stepY * 0.5f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Sprite sprite = (x + y) % 3 == 0 && floorB != null ? floorB : floorA;
                Vector3 position = new Vector3(startX + x * stepX, startY + y * stepY, 0f);
                CreateSpriteObject(parent, "Floor_" + x + "_" + y, sprite, position, Quaternion.identity, FloorScale, -30);
            }
        }
    }

    private static void CreateBorders(Transform parent)
    {
        Sprite border = LoadSprite(BorderSpritePath);
        Sprite corner = LoadSprite(CornerSpritePath);

        const int columns = 9;
        const int rows = 6;
        const float stepX = 1.15f;
        const float stepY = 0.92f;
        float left = -columns * stepX * 0.5f;
        float right = columns * stepX * 0.5f;
        float bottom = -rows * stepY * 0.5f;
        float top = rows * stepY * 0.5f;

        for (int x = 0; x < columns; x++)
        {
            float px = -(columns - 1) * stepX * 0.5f + x * stepX;
            CreateSpriteObject(parent, "Border_Top_" + x, border, new Vector3(px, top, 0f), Quaternion.identity, BorderScale, -20);
            CreateSpriteObject(parent, "Border_Bottom_" + x, border, new Vector3(px, bottom, 0f), Quaternion.Euler(0f, 0f, 180f), BorderScale, -20);
        }

        for (int y = 0; y < rows; y++)
        {
            float py = -(rows - 1) * stepY * 0.5f + y * stepY;
            CreateSpriteObject(parent, "Border_Left_" + y, border, new Vector3(left, py, 0f), Quaternion.Euler(0f, 0f, 90f), BorderScale, -20);
            CreateSpriteObject(parent, "Border_Right_" + y, border, new Vector3(right, py, 0f), Quaternion.Euler(0f, 0f, -90f), BorderScale, -20);
        }

        CreateSpriteObject(parent, "Corner_TopLeft", corner, new Vector3(left, top, 0f), Quaternion.identity, CornerScale, -19);
        CreateSpriteObject(parent, "Corner_TopRight", corner, new Vector3(right, top, 0f), Quaternion.Euler(0f, 0f, -90f), CornerScale, -19);
        CreateSpriteObject(parent, "Corner_BottomLeft", corner, new Vector3(left, bottom, 0f), Quaternion.Euler(0f, 0f, 90f), CornerScale, -19);
        CreateSpriteObject(parent, "Corner_BottomRight", corner, new Vector3(right, bottom, 0f), Quaternion.Euler(0f, 0f, 180f), CornerScale, -19);
    }

    private static void ConfigureActors()
    {
        GameObject player = GameObject.Find("Player_Mipurin");
        if (player != null)
        {
            player.transform.position = PlayerSpawn;
            PlayerBoundsLimiter limiter = player.GetComponent<PlayerBoundsLimiter>();
            if (limiter == null)
            {
                limiter = player.AddComponent<PlayerBoundsLimiter>();
            }
            limiter.Configure(PlayerMin, PlayerMax, true);
            EditorUtility.SetDirty(player);
            EditorUtility.SetDirty(limiter);
        }

        GameObject honeySlime = GameObject.Find("Enemy_HoneySlime");
        if (honeySlime != null)
        {
            honeySlime.transform.position = HoneySlimeSpawn;
            honeySlime.transform.localScale = HoneySlimeScale;
            EditorUtility.SetDirty(honeySlime);
        }

        GameObject legacyEnemy = GameObject.Find("Enemy_Test");
        if (legacyEnemy != null && honeySlime != null)
        {
            legacyEnemy.SetActive(false);
            EditorUtility.SetDirty(legacyEnemy);
        }
        else if (legacyEnemy != null)
        {
            legacyEnemy.transform.position = HoneySlimeSpawn;
            legacyEnemy.transform.localScale = HoneySlimeScale;
            EditorUtility.SetDirty(legacyEnemy);
        }

        GameObject poisonMushroom = GameObject.Find("Enemy_PoisonMushroom");
        if (poisonMushroom != null)
        {
            poisonMushroom.transform.position = PoisonMushroomSpawn;
            poisonMushroom.transform.localScale = PoisonMushroomScale;
            EditorUtility.SetDirty(poisonMushroom);
        }
    }

    private static void ConfigureCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
            if (Object.FindObjectOfType<AudioListener>() == null)
            {
                cameraObject.AddComponent<AudioListener>();
            }
        }

        GameObject player = GameObject.Find("Player_Mipurin");
        camera.orthographic = true;
        camera.orthographicSize = 4.1f;
        camera.backgroundColor = new Color(0.73f, 0.9f, 1f, 1f);

        SimpleCameraFollow follow = camera.GetComponent<SimpleCameraFollow>();
        if (follow == null)
        {
            follow = camera.gameObject.AddComponent<SimpleCameraFollow>();
        }

        follow.Configure(player != null ? player.transform : null, new Vector3(0f, 0f, -10f), 8f, true, CameraMin, CameraMax);
        follow.SnapToTarget();

        EditorUtility.SetDirty(camera.gameObject);
        EditorUtility.SetDirty(camera);
        EditorUtility.SetDirty(follow);
    }

    private static Transform CreateChild(Transform parent, string name)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
        return child.transform;
    }

    private static void CreateSpawnPoint(Transform parent, string name, Vector3 position)
    {
        GameObject point = new GameObject(name);
        point.transform.SetParent(parent);
        point.transform.position = position;
        point.transform.localRotation = Quaternion.identity;
        point.transform.localScale = Vector3.one;
    }

    private static void CreateSpriteObject(Transform parent, string name, Sprite sprite, Vector3 position, Quaternion rotation, Vector3 scale, int sortingOrder)
    {
        if (sprite == null)
        {
            return;
        }

        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = scale;

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = sortingOrder;
        EditorUtility.SetDirty(obj);
        EditorUtility.SetDirty(renderer);
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning("Sprite not found: " + path);
        }
        return sprite;
    }
}
