using UnityEditor;
using UnityEngine;

public static class MipurinPhase4EnemyPrefabSetupTools
{
    private const string EnemyPrefabFolder = "Assets/_Project/Prefabs/Enemies";

    private const string HoneySlimePrefabPath = EnemyPrefabFolder + "/Enemy_HoneySlime.prefab";
    private const string PoisonMushroomPrefabPath = EnemyPrefabFolder + "/Enemy_PoisonMushroom.prefab";
    private const string StingerBeePrefabPath = EnemyPrefabFolder + "/Enemy_StingerBee.prefab";
    private const string FlowerTurretPrefabPath = EnemyPrefabFolder + "/Enemy_FlowerTurret.prefab";
    private const string HeavyBeetlePrefabPath = EnemyPrefabFolder + "/Enemy_HeavyBeetle.prefab";

    private const string StingerBeeSpriteFolder = "Assets/_Project/Sprites/Enemies/StingerBee/";
    private const string FlowerTurretSpriteFolder = "Assets/_Project/Sprites/Enemies/FlowerTurret/";
    private const string HeavyBeetleSpriteFolder = "Assets/_Project/Sprites/Enemies/HeavyBeetle/";

    private const float SpritePixelsPerUnit = 300f;

    [MenuItem("Mipurin/Setup/Setup Phase 4 Enemy Prefabs")]
    public static void SetupPhase4EnemyPrefabs()
    {
        EnsureFolder("Assets/_Project/Prefabs");
        EnsureFolder(EnemyPrefabFolder);

        ForceImportAllPhase4EnemySprites();

        GameObject honeyBase = AssetDatabase.LoadAssetAtPath<GameObject>(HoneySlimePrefabPath);
        GameObject mushroomBase = AssetDatabase.LoadAssetAtPath<GameObject>(PoisonMushroomPrefabPath);

        if (honeyBase == null || mushroomBase == null)
        {
            Debug.LogError("Phase 4 enemy setup failed. Base enemy prefabs are missing.");
            return;
        }

        CreateEnemyPrefabFromBase(honeyBase, StingerBeePrefabPath, "Enemy_StingerBee", "StingerBee", new Vector3(0.26f, 0.26f, 1f));
        CreateEnemyPrefabFromBase(mushroomBase, FlowerTurretPrefabPath, "Enemy_FlowerTurret", "FlowerTurret", new Vector3(0.34f, 0.34f, 1f));
        CreateEnemyPrefabFromBase(honeyBase, HeavyBeetlePrefabPath, "Enemy_HeavyBeetle", "HeavyBeetle", new Vector3(0.42f, 0.42f, 1f));

        ApplyEnemySprites(
            StingerBeePrefabPath,
            StingerBeeSpriteFolder,
            new[] { "stinger_bee_idle_01.png", "stinger_bee_idle_02.png" },
            "stinger_bee_hurt_01.png",
            "stinger_bee_down_01.png",
            4.5f);

        ApplyEnemySprites(
            FlowerTurretPrefabPath,
            FlowerTurretSpriteFolder,
            new[] { "flower_turret_idle_01.png", "flower_turret_idle_02.png" },
            "flower_turret_hurt_01.png",
            "flower_turret_down_01.png",
            1.4f);

        ApplyEnemySprites(
            HeavyBeetlePrefabPath,
            HeavyBeetleSpriteFolder,
            new[] { "heavy_beetle_idle_01.png", "heavy_beetle_idle_02.png", "heavy_beetle_walk_01.png" },
            "heavy_beetle_hurt_01.png",
            "heavy_beetle_down_01.png",
            1.2f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Phase 4 enemy prefabs setup complete. Enemy sprites were imported and applied if matching PNG names were found.");
    }

    private static void CreateEnemyPrefabFromBase(GameObject basePrefab, string savePath, string objectName, string prototypeName, Vector3 scale)
    {
        GameObject instance = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
        if (instance == null)
        {
            Debug.LogError("Failed to instantiate base prefab: " + basePrefab.name);
            return;
        }

        instance.name = objectName;
        instance.transform.localScale = scale;

        MipurinEnemy enemy = instance.GetComponent<MipurinEnemy>();
        if (enemy != null)
        {
            enemy.ApplyRuntimePrototype(prototypeName);
            EditorUtility.SetDirty(enemy);
        }

        EnemyDropper dropper = instance.GetComponent<EnemyDropper>();
        if (dropper != null)
        {
            dropper.ConfigureAmount(GetNectarAmount(prototypeName));
            EditorUtility.SetDirty(dropper);
        }

        PrefabUtility.SaveAsPrefabAsset(instance, savePath);
        Object.DestroyImmediate(instance);
    }

    private static void ApplyEnemySprites(string prefabPath, string spriteFolder, string[] idleFileNames, string hurtFileName, string downFileName, float idleFps)
    {
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        if (instance == null)
        {
            Debug.LogWarning("Failed to load prefab contents: " + prefabPath);
            return;
        }

        SpriteRenderer spriteRenderer = instance.GetComponentInChildren<SpriteRenderer>();
        EnemySpriteAnimator animator = instance.GetComponent<EnemySpriteAnimator>();
        if (animator == null)
        {
            animator = instance.AddComponent<EnemySpriteAnimator>();
        }

        Sprite[] idleSprites = LoadSprites(spriteFolder, idleFileNames);
        Sprite hurtSprite = LoadSprite(spriteFolder + hurtFileName);
        Sprite downSprite = LoadSprite(spriteFolder + downFileName);

        if (spriteRenderer != null && idleSprites.Length > 0 && idleSprites[0] != null)
        {
            spriteRenderer.sprite = idleSprites[0];
            spriteRenderer.color = Color.white;
            EditorUtility.SetDirty(spriteRenderer);
        }

        animator.Configure(spriteRenderer, idleSprites, hurtSprite, downSprite, idleFps, 0.18f);
        EditorUtility.SetDirty(animator);
        EditorUtility.SetDirty(instance);

        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        PrefabUtility.UnloadPrefabContents(instance);

        Debug.Log("Applied enemy sprites if found: " + prefabPath);
    }

    private static void ForceImportAllPhase4EnemySprites()
    {
        ForceImportSprites(StingerBeeSpriteFolder, new[]
        {
            "stinger_bee_idle_01.png",
            "stinger_bee_idle_02.png",
            "stinger_bee_dash_01.png",
            "stinger_bee_hurt_01.png",
            "stinger_bee_down_01.png"
        });

        ForceImportSprites(FlowerTurretSpriteFolder, new[]
        {
            "flower_turret_idle_01.png",
            "flower_turret_idle_02.png",
            "flower_turret_shoot_01.png",
            "flower_turret_hurt_01.png",
            "flower_turret_down_01.png"
        });

        ForceImportSprites(HeavyBeetleSpriteFolder, new[]
        {
            "heavy_beetle_idle_01.png",
            "heavy_beetle_idle_02.png",
            "heavy_beetle_walk_01.png",
            "heavy_beetle_hurt_01.png",
            "heavy_beetle_down_01.png"
        });
    }

    private static void ForceImportSprites(string folder, string[] fileNames)
    {
        foreach (string fileName in fileNames)
        {
            ForceImportSprite(folder + fileName);
        }
    }

    private static void ForceImportSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning("PNG not found for sprite import: " + path);
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = SpritePixelsPerUnit;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static Sprite[] LoadSprites(string folder, string[] fileNames)
    {
        Sprite[] sprites = new Sprite[fileNames.Length];

        for (int i = 0; i < fileNames.Length; i++)
        {
            sprites[i] = LoadSprite(folder + fileNames[i]);
        }

        return sprites;
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning("Sprite not found or not imported as Sprite: " + path);
        }

        return sprite;
    }

    private static int GetNectarAmount(string prototypeName)
    {
        switch (prototypeName)
        {
            case "FlowerTurret": return 2;
            case "HeavyBeetle": return 3;
            default: return 1;
        }
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string parent = System.IO.Path.GetDirectoryName(folderPath).Replace("\\", "/");
        string folderName = System.IO.Path.GetFileName(folderPath);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, folderName);
    }
}
