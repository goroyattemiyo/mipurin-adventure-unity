using UnityEditor;
using UnityEngine;

public static class MipurinPhase4EnemySpriteApplyTools
{
    private const string StingerBeePrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_StingerBee.prefab";
    private const string FlowerTurretPrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_FlowerTurret.prefab";
    private const string HeavyBeetlePrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_HeavyBeetle.prefab";

    private const string StingerBeeSpriteFolder = "Assets/_Project/Sprites/Enemies/StingerBee/";
    private const string FlowerTurretSpriteFolder = "Assets/_Project/Sprites/Enemies/FlowerTurret/";
    private const string HeavyBeetleSpriteFolder = "Assets/_Project/Sprites/Enemies/HeavyBeetle/";

    [MenuItem("Mipurin/Setup/Apply Phase 4 Enemy Sprites")]
    public static void ApplyPhase4EnemySprites()
    {
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
        Debug.Log("Phase 4 enemy sprites apply complete.");
    }

    private static void ApplyEnemySprites(string prefabPath, string spriteFolder, string[] idleFileNames, string hurtFileName, string downFileName, float idleFps)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning("Enemy prefab not found. Run Setup Phase 4 Enemy Prefabs first: " + prefabPath);
            return;
        }

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
            EditorUtility.SetDirty(spriteRenderer);
        }

        animator.Configure(spriteRenderer, idleSprites, hurtSprite, downSprite, idleFps, 0.18f);

        EditorUtility.SetDirty(instance);
        EditorUtility.SetDirty(animator);
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        PrefabUtility.UnloadPrefabContents(instance);

        Debug.Log("Applied enemy sprites: " + prefabPath);
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
}
