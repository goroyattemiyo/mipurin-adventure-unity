using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinMvpSetupTools
{
    private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player_Mipurin.prefab";
    private const string AttackEffectPrefabPath = "Assets/_Project/Prefabs/Effects/AttackEffect_SlashYellow.prefab";
    private const string HitEffectPrefabPath = "Assets/_Project/Prefabs/Effects/HitEffect_DamageStar.prefab";
    private const string EnemyPrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_Test.prefab";

    private static readonly Vector3 HoneySlimeScale = new Vector3(0.4f, 0.4f, 1f);

    private static readonly string[] PlayerIdleSpritePaths =
    {
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_idle_01.png",
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_idle_02.png"
    };

    private static readonly string[] PlayerWalkSpritePaths =
    {
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_walk_01.png",
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_walk_02.png",
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_walk_03.png"
    };

    private static readonly string[] PlayerAttackSpritePaths =
    {
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_attack_01.png",
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_attack_02.png",
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_attack_03.png"
    };

    private static readonly string[] AttackEffectSpritePaths =
    {
        "Assets/_Project/Sprites/Effects/slash_yellow_01.png"
    };

    private static readonly string[] HitEffectSpritePaths =
    {
        "Assets/_Project/Sprites/Effects/damage_star_01.png",
        "Assets/_Project/Sprites/Effects/honey_spark_01.png"
    };

    private static readonly string[] EnemyIdleSpritePaths =
    {
        "Assets/_Project/Sprites/Enemies/HoneySlime/enemy_honey_slime_idle_01.png",
        "Assets/_Project/Sprites/Enemies/HoneySlime/enemy_honey_slime_idle_02.png"
    };

    private const string PlayerHurtSpritePath = "Assets/_Project/Sprites/Player/Mipurin/Body/front_hurt_01.png";
    private const string PlayerDownSpritePath = "Assets/_Project/Sprites/Player/Mipurin/Body/front_down_01.png";
    private const string EnemyHurtSpritePath = "Assets/_Project/Sprites/Enemies/HoneySlime/enemy_honey_slime_hurt_01.png";
    private const string EnemyDownSpritePath = "Assets/_Project/Sprites/Enemies/HoneySlime/enemy_honey_slime_down_01.png";

    [MenuItem("Mipurin/Setup/Setup MVP Combat")]
    public static void SetupMvpCombat()
    {
        EnsureFolders();

        GameObject attackEffectPrefab = CreateOrUpdateAttackEffectPrefab();
        GameObject hitEffectPrefab = CreateOrUpdateHitEffectPrefab();
        GameObject enemyPrefab = CreateOrUpdateEnemyPrefab();

        ConfigurePlayerPrefab(attackEffectPrefab, hitEffectPrefab);
        ConfigureActiveScene(attackEffectPrefab, hitEffectPrefab, enemyPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Mipurin MVP combat setup complete. Test with WASD and Space.");
    }

    private static void EnsureFolders()
    {
        Directory.CreateDirectory("Assets/_Project/Prefabs/Effects");
        Directory.CreateDirectory("Assets/_Project/Prefabs/Enemies");
        AssetDatabase.Refresh();
    }

    private static GameObject CreateOrUpdateAttackEffectPrefab()
    {
        Sprite[] effectSprites = LoadSprites(AttackEffectSpritePaths);

        GameObject root = new GameObject("AttackEffect_SlashYellow");
        root.transform.localScale = new Vector3(0.35f, 0.35f, 1f);

        SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 30;

        if (effectSprites.Length > 0)
        {
            renderer.sprite = effectSprites[0];
        }

        SimpleEffectPlayer effectPlayer = root.AddComponent<SimpleEffectPlayer>();
        effectPlayer.Configure(renderer, effectSprites, 18f, true, 0.18f);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, AttackEffectPrefabPath);
        Object.DestroyImmediate(root);

        Debug.Log($"Updated attack effect prefab: {AttackEffectPrefabPath}");
        return savedPrefab;
    }

    private static GameObject CreateOrUpdateHitEffectPrefab()
    {
        Sprite[] effectSprites = LoadSprites(HitEffectSpritePaths);

        GameObject root = new GameObject("HitEffect_DamageStar");
        root.transform.localScale = new Vector3(0.25f, 0.25f, 1f);

        SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 35;

        if (effectSprites.Length > 0)
        {
            renderer.sprite = effectSprites[0];
        }

        SimpleEffectPlayer effectPlayer = root.AddComponent<SimpleEffectPlayer>();
        effectPlayer.Configure(renderer, effectSprites, 14f, true, 0.22f);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, HitEffectPrefabPath);
        Object.DestroyImmediate(root);

        Debug.Log($"Updated hit effect prefab: {HitEffectPrefabPath}");
        return savedPrefab;
    }

    private static GameObject CreateOrUpdateEnemyPrefab()
    {
        Sprite[] idleSprites = LoadSprites(EnemyIdleSpritePaths);
        Sprite hurtSprite = LoadSprite(EnemyHurtSpritePath);
        Sprite downSprite = LoadSprite(EnemyDownSpritePath);

        GameObject root = new GameObject("Enemy_Test");
        root.transform.localScale = HoneySlimeScale;

        SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 8;

        if (idleSprites.Length > 0)
        {
            renderer.sprite = idleSprites[0];
        }

        CircleCollider2D collider = root.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.8f;

        EnemySpriteAnimator spriteAnimator = root.AddComponent<EnemySpriteAnimator>();
        spriteAnimator.Configure(renderer, idleSprites, hurtSprite, downSprite, 2f, 0.18f);

        MipurinEnemy enemy = root.AddComponent<MipurinEnemy>();
        enemy.ConfigureSprites(renderer, idleSprites, hurtSprite, downSprite);
        enemy.ConfigureHitReaction(0.55f, 0.25f, new Color(1f, 0.35f, 0.25f, 1f), 0.12f);

        MipurinContactDamage contactDamage = root.AddComponent<MipurinContactDamage>();
        contactDamage.Configure(null, 1, 1.2f, 0.85f);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, EnemyPrefabPath);
        Object.DestroyImmediate(root);

        Debug.Log($"Updated enemy prefab: {EnemyPrefabPath}");
        return savedPrefab;
    }

    private static void ConfigurePlayerPrefab(GameObject attackEffectPrefab, GameObject hitEffectPrefab)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);

        if (prefabRoot == null)
        {
            Debug.LogError($"Player prefab not found: {PlayerPrefabPath}");
            return;
        }

        ConfigurePlayer(prefabRoot, attackEffectPrefab, hitEffectPrefab);

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, PlayerPrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Debug.Log($"Updated player prefab: {PlayerPrefabPath}");
    }

    private static void ConfigureActiveScene(GameObject attackEffectPrefab, GameObject hitEffectPrefab, GameObject enemyPrefab)
    {
        GameObject scenePlayer = GameObject.Find("Player_Mipurin");

        if (scenePlayer == null)
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);

            if (playerPrefab != null)
            {
                scenePlayer = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                scenePlayer.name = "Player_Mipurin";
                scenePlayer.transform.position = Vector3.zero;
            }
        }

        if (scenePlayer == null)
        {
            Debug.LogWarning("Active scene does not contain Player_Mipurin and prefab could not be instantiated.");
            return;
        }

        MipurinHealth playerHealth = ConfigurePlayer(scenePlayer, attackEffectPrefab, hitEffectPrefab);
        MipurinEnemy sceneEnemy = EnsureSceneEnemy(enemyPrefab, scenePlayer.transform, playerHealth);
        EnsureDebugHud(playerHealth, sceneEnemy);
        EnsureCameraFollow(scenePlayer.transform);

        EditorUtility.SetDirty(scenePlayer);
        EditorSceneManager.MarkSceneDirty(scenePlayer.scene);
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("Updated active scene MVP combat objects.");
    }

    private static MipurinHealth ConfigurePlayer(GameObject player, GameObject attackEffectPrefab, GameObject hitEffectPrefab)
    {
        Transform body = FindOrCreateChild(player.transform, "Body", Vector3.zero);
        SpriteRenderer bodyRenderer = FindOrCreateSpriteRenderer(body);
        bodyRenderer.sortingOrder = 10;

        Sprite[] idleSprites = LoadSprites(PlayerIdleSpritePaths);
        Sprite[] walkSprites = LoadSprites(PlayerWalkSpritePaths);
        Sprite[] attackSprites = LoadSprites(PlayerAttackSpritePaths);
        Sprite hurtSprite = LoadSprite(PlayerHurtSpritePath);
        Sprite downSprite = LoadSprite(PlayerDownSpritePath);

        PlayerSpriteAnimator spriteAnimator = player.GetComponent<PlayerSpriteAnimator>();

        if (spriteAnimator == null)
        {
            spriteAnimator = player.AddComponent<PlayerSpriteAnimator>();
        }

        spriteAnimator.Configure(bodyRenderer, idleSprites, walkSprites, attackSprites, hurtSprite, downSprite);

        MipurinAttack attack = player.GetComponent<MipurinAttack>();

        if (attack == null)
        {
            attack = player.AddComponent<MipurinAttack>();
        }

        attack.Configure(attackEffectPrefab, hitEffectPrefab, 0.85f, 1.05f, 0.4f, 1.15f, 0.85f);

        MipurinHealth health = player.GetComponent<MipurinHealth>();

        if (health == null)
        {
            health = player.AddComponent<MipurinHealth>();
        }

        health.Configure(bodyRenderer, hurtSprite, downSprite);

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(bodyRenderer);
        EditorUtility.SetDirty(spriteAnimator);
        EditorUtility.SetDirty(attack);
        EditorUtility.SetDirty(health);

        return health;
    }

    private static MipurinEnemy EnsureSceneEnemy(GameObject enemyPrefab, Transform playerTransform, MipurinHealth playerHealth)
    {
        GameObject sceneEnemyObject = GameObject.Find("Enemy_Test");

        if (sceneEnemyObject == null && enemyPrefab != null)
        {
            sceneEnemyObject = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab);
            sceneEnemyObject.name = "Enemy_Test";
            sceneEnemyObject.transform.position = new Vector3(3.2f, -0.2f, 0f);
        }

        if (sceneEnemyObject == null)
        {
            return null;
        }

        ConfigureEnemyAppearance(sceneEnemyObject);

        MipurinEnemy enemy = sceneEnemyObject.GetComponent<MipurinEnemy>();

        if (enemy == null)
        {
            enemy = sceneEnemyObject.AddComponent<MipurinEnemy>();
        }

        enemy.SetTarget(playerTransform);
        enemy.ConfigureHitReaction(0.55f, 0.25f, new Color(1f, 0.35f, 0.25f, 1f), 0.12f);

        MipurinContactDamage contactDamage = sceneEnemyObject.GetComponent<MipurinContactDamage>();

        if (contactDamage == null)
        {
            contactDamage = sceneEnemyObject.AddComponent<MipurinContactDamage>();
        }

        contactDamage.Configure(playerHealth, 1, 1.2f, 0.85f);
        contactDamage.ResetDamageTimer();

        EditorUtility.SetDirty(sceneEnemyObject);
        EditorUtility.SetDirty(enemy);
        EditorUtility.SetDirty(contactDamage);

        return enemy;
    }

    private static void ConfigureEnemyAppearance(GameObject enemyObject)
    {
        Sprite[] idleSprites = LoadSprites(EnemyIdleSpritePaths);
        Sprite hurtSprite = LoadSprite(EnemyHurtSpritePath);
        Sprite downSprite = LoadSprite(EnemyDownSpritePath);

        SpriteRenderer renderer = enemyObject.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            renderer = enemyObject.AddComponent<SpriteRenderer>();
        }

        renderer.sortingOrder = 8;

        if (idleSprites.Length > 0)
        {
            renderer.sprite = idleSprites[0];
        }

        EnemySpriteAnimator spriteAnimator = enemyObject.GetComponent<EnemySpriteAnimator>();

        if (spriteAnimator == null)
        {
            spriteAnimator = enemyObject.AddComponent<EnemySpriteAnimator>();
        }

        spriteAnimator.Configure(renderer, idleSprites, hurtSprite, downSprite, 2f, 0.18f);

        MipurinEnemy enemy = enemyObject.GetComponent<MipurinEnemy>();

        if (enemy != null)
        {
            enemy.ConfigureSprites(renderer, idleSprites, hurtSprite, downSprite);
        }

        CircleCollider2D circleCollider = enemyObject.GetComponent<CircleCollider2D>();

        if (circleCollider != null)
        {
            circleCollider.radius = 0.8f;
            circleCollider.isTrigger = true;
            EditorUtility.SetDirty(circleCollider);
        }

        enemyObject.transform.localScale = HoneySlimeScale;

        EditorUtility.SetDirty(enemyObject);
        EditorUtility.SetDirty(renderer);
        EditorUtility.SetDirty(spriteAnimator);
    }

    private static void EnsureDebugHud(MipurinHealth playerHealth, MipurinEnemy enemy)
    {
        GameObject hudObject = GameObject.Find("MVP_DebugHUD");

        if (hudObject == null)
        {
            hudObject = new GameObject("MVP_DebugHUD");
        }

        MipurinDebugHud hud = hudObject.GetComponent<MipurinDebugHud>();

        if (hud == null)
        {
            hud = hudObject.AddComponent<MipurinDebugHud>();
        }

        hud.Configure(playerHealth, enemy);
        EditorUtility.SetDirty(hudObject);
        EditorUtility.SetDirty(hud);
    }

    private static void EnsureCameraFollow(Transform target)
    {
        Camera camera = Camera.main;

        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;

            if (Object.FindObjectOfType<AudioListener>() == null)
            {
                cameraObject.AddComponent<AudioListener>();
            }
        }

        camera.orthographic = true;
        camera.orthographicSize = 5f;

        SimpleCameraFollow cameraFollow = camera.GetComponent<SimpleCameraFollow>();

        if (cameraFollow == null)
        {
            cameraFollow = camera.gameObject.AddComponent<SimpleCameraFollow>();
        }

        cameraFollow.Configure(target, new Vector3(0f, 0f, -10f), 8f, false, new Vector2(-20f, -12f), new Vector2(20f, 12f));
        cameraFollow.SnapToTarget();

        EditorUtility.SetDirty(camera.gameObject);
        EditorUtility.SetDirty(camera);
        EditorUtility.SetDirty(cameraFollow);
    }

    private static Transform FindOrCreateChild(Transform parent, string childName, Vector3 defaultLocalPosition)
    {
        Transform child = parent.Find(childName);

        if (child != null)
        {
            return child;
        }

        GameObject childObject = new GameObject(childName);
        childObject.transform.SetParent(parent);
        childObject.transform.localPosition = defaultLocalPosition;
        childObject.transform.localRotation = Quaternion.identity;
        childObject.transform.localScale = Vector3.one;

        return childObject.transform;
    }

    private static SpriteRenderer FindOrCreateSpriteRenderer(Transform target)
    {
        SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            renderer = target.gameObject.AddComponent<SpriteRenderer>();
        }

        return renderer;
    }

    private static Sprite[] LoadSprites(string[] paths)
    {
        Sprite[] sprites = new Sprite[paths.Length];
        int count = 0;

        foreach (string path in paths)
        {
            Sprite sprite = LoadSprite(path);

            if (sprite == null)
            {
                continue;
            }

            sprites[count] = sprite;
            count++;
        }

        if (count == sprites.Length)
        {
            return sprites;
        }

        Sprite[] trimmed = new Sprite[count];

        for (int i = 0; i < count; i++)
        {
            trimmed[i] = sprites[i];
        }

        return trimmed;
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        if (sprite == null)
        {
            Debug.LogWarning($"Sprite not found: {path}");
        }

        return sprite;
    }
}
