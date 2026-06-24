using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinStageSetupTools
{
    private const string StageBlockSpritePath = "Assets/_Project/Sprites/Environment/stage_block_01.png";
    private const string EnemyPrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_Test.prefab";

    private static readonly Vector2 PlayerMin = new Vector2(-8.5f, -4.5f);
    private static readonly Vector2 PlayerMax = new Vector2(8.5f, 4.5f);
    private static readonly Vector2 CameraMin = new Vector2(-5.5f, -1.5f);
    private static readonly Vector2 CameraMax = new Vector2(5.5f, 1.5f);

    [MenuItem("Mipurin/Setup/Setup Character Test Stage")]
    public static void SetupCharacterTestStage()
    {
        EnsureFolders();
        Sprite stageSprite = EnsureStageSprite();

        CreateOrUpdateStageObjects(stageSprite);

        GameObject player = GameObject.Find("Player_Mipurin");
        ConfigurePlayerBounds(player);
        ConfigureEnemyPosition();
        ConfigureCamera(player != null ? player.transform : null);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("Mipurin CharacterTest stage setup complete. Check floor, bounds, enemy position, and camera follow.");
    }

    private static void EnsureFolders()
    {
        Directory.CreateDirectory("Assets/_Project/Sprites/Environment");
        AssetDatabase.Refresh();
    }

    private static Sprite EnsureStageSprite()
    {
        if (!File.Exists(StageBlockSpritePath))
        {
            Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[16 * 16];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(StageBlockSpritePath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(StageBlockSpritePath);
        }

        TextureImporter importer = AssetImporter.GetAtPath(StageBlockSpritePath) as TextureImporter;

        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 16f;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(StageBlockSpritePath);

        if (sprite == null)
        {
            Debug.LogWarning($"Stage sprite could not be loaded: {StageBlockSpritePath}");
        }

        return sprite;
    }

    private static void CreateOrUpdateStageObjects(Sprite stageSprite)
    {
        GameObject root = GameObject.Find("Stage_TestRoot");

        if (root == null)
        {
            root = new GameObject("Stage_TestRoot");
        }

        root.transform.position = Vector3.zero;

        ConfigureStageBlock(root.transform, "Stage_Background", new Vector3(0f, 0f, 1f), new Vector3(23f, 15f, 1f), new Color(0.78f, 0.9f, 1f, 1f), -100, stageSprite);
        ConfigureStageBlock(root.transform, "Stage_Floor", new Vector3(0f, -5f, 0f), new Vector3(20f, 0.35f, 1f), new Color(0.98f, 0.84f, 0.38f, 1f), -20, stageSprite);
        ConfigureStageBlock(root.transform, "Stage_LeftWall", new Vector3(-9.5f, 0f, 0f), new Vector3(0.35f, 10f, 1f), new Color(0.85f, 0.65f, 0.28f, 1f), -15, stageSprite);
        ConfigureStageBlock(root.transform, "Stage_RightWall", new Vector3(9.5f, 0f, 0f), new Vector3(0.35f, 10f, 1f), new Color(0.85f, 0.65f, 0.28f, 1f), -15, stageSprite);
        ConfigureStageBlock(root.transform, "Stage_TopWall", new Vector3(0f, 5f, 0f), new Vector3(20f, 0.25f, 1f), new Color(0.95f, 0.74f, 0.32f, 1f), -15, stageSprite);
        ConfigureStageBlock(root.transform, "Stage_BottomWall", new Vector3(0f, -5f, 0f), new Vector3(20f, 0.25f, 1f), new Color(0.78f, 0.5f, 0.24f, 1f), -10, stageSprite);

        EditorUtility.SetDirty(root);
    }

    private static void ConfigureStageBlock(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Color color, int sortingOrder, Sprite sprite)
    {
        Transform child = parent.Find(name);

        if (child == null)
        {
            GameObject childObject = new GameObject(name);
            childObject.transform.SetParent(parent);
            child = childObject.transform;
        }

        child.localPosition = localPosition;
        child.localRotation = Quaternion.identity;
        child.localScale = localScale;

        SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            renderer = child.gameObject.AddComponent<SpriteRenderer>();
        }

        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        EditorUtility.SetDirty(child.gameObject);
        EditorUtility.SetDirty(renderer);
    }

    private static void ConfigurePlayerBounds(GameObject player)
    {
        if (player == null)
        {
            Debug.LogWarning("Player_Mipurin was not found. Player bounds were not configured.");
            return;
        }

        PlayerBoundsLimiter limiter = player.GetComponent<PlayerBoundsLimiter>();

        if (limiter == null)
        {
            limiter = player.AddComponent<PlayerBoundsLimiter>();
        }

        limiter.Configure(PlayerMin, PlayerMax, true);
        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(limiter);
    }

    private static void ConfigureEnemyPosition()
    {
        GameObject enemy = GameObject.Find("Enemy_Test");

        if (enemy == null)
        {
            GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefabPath);

            if (enemyPrefab != null)
            {
                enemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab);
                enemy.name = "Enemy_Test";
            }
        }

        if (enemy == null)
        {
            Debug.LogWarning("Enemy_Test was not found. Enemy position was not configured.");
            return;
        }

        enemy.transform.position = new Vector3(4f, 1f, 0f);
        EditorUtility.SetDirty(enemy);
    }

    private static void ConfigureCamera(Transform target)
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

        camera.orthographic = true;
        camera.orthographicSize = 4.5f;
        camera.backgroundColor = new Color(0.78f, 0.9f, 1f, 1f);
        camera.transform.position = new Vector3(0f, 0f, -10f);

        SimpleCameraFollow cameraFollow = camera.GetComponent<SimpleCameraFollow>();

        if (cameraFollow == null)
        {
            cameraFollow = camera.gameObject.AddComponent<SimpleCameraFollow>();
        }

        cameraFollow.Configure(target, new Vector3(0f, 0f, -10f), 8f, true, CameraMin, CameraMax);
        cameraFollow.SnapToTarget();

        EditorUtility.SetDirty(camera.gameObject);
        EditorUtility.SetDirty(camera);
        EditorUtility.SetDirty(cameraFollow);
    }
}
