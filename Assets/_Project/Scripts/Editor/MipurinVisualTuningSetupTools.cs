using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinVisualTuningSetupTools
{
    private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player_Mipurin.prefab";
    private const string EnemyPrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_Test.prefab";
    private const string AttackEffectPrefabPath = "Assets/_Project/Prefabs/Effects/AttackEffect_SlashYellow.prefab";
    private const string HitEffectPrefabPath = "Assets/_Project/Prefabs/Effects/HitEffect_DamageStar.prefab";

    private static readonly Vector3 PlayerVisualScale = new Vector3(0.24f, 0.24f, 1f);
    private static readonly Vector3 WingsLocalPosition = new Vector3(0f, 0.15f, 0f);
    private static readonly Vector3 WingsLocalScale = new Vector3(0.82f, 0.82f, 1f);
    private static readonly Vector3 EnemyScenePosition = new Vector3(3.2f, -0.2f, 0f);
    private static readonly Vector3 EnemyVisualScale = new Vector3(0.4f, 0.4f, 1f);
    private static readonly Vector3 AttackEffectScale = new Vector3(0.35f, 0.35f, 1f);
    private static readonly Vector3 HitEffectScale = new Vector3(0.25f, 0.25f, 1f);

    private static readonly Vector2 PlayerMin = new Vector2(-8.5f, -4.5f);
    private static readonly Vector2 PlayerMax = new Vector2(8.5f, 4.5f);
    private static readonly Vector2 CameraMin = new Vector2(-5.5f, -1.5f);
    private static readonly Vector2 CameraMax = new Vector2(5.5f, 1.5f);

    [MenuItem("Mipurin/Setup/Setup Visual Tuning")]
    public static void SetupVisualTuning()
    {
        ConfigurePlayerPrefab();
        ConfigureEnemyPrefab();
        ConfigureEffectPrefab(AttackEffectPrefabPath, AttackEffectScale, 30);
        ConfigureEffectPrefab(HitEffectPrefabPath, HitEffectScale, 35);
        ConfigureActiveScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("Mipurin visual tuning setup complete. Player, wings, enemy, HUD, effects, and camera were tuned.");
    }

    private static void ConfigurePlayerPrefab()
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);

        if (prefabRoot == null)
        {
            Debug.LogWarning($"Player prefab not found: {PlayerPrefabPath}");
            return;
        }

        ConfigurePlayerVisuals(prefabRoot);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, PlayerPrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    private static void ConfigureEnemyPrefab()
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(EnemyPrefabPath);

        if (prefabRoot == null)
        {
            Debug.LogWarning($"Enemy prefab not found: {EnemyPrefabPath}");
            return;
        }

        ConfigureEnemyVisuals(prefabRoot, false);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, EnemyPrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    private static void ConfigureEffectPrefab(string prefabPath, Vector3 scale, int sortingOrder)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

        if (prefabRoot == null)
        {
            Debug.LogWarning($"Effect prefab not found: {prefabPath}");
            return;
        }

        prefabRoot.transform.localPosition = Vector3.zero;
        prefabRoot.transform.localRotation = Quaternion.identity;
        prefabRoot.transform.localScale = scale;

        SpriteRenderer renderer = prefabRoot.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.sortingOrder = sortingOrder;
            EditorUtility.SetDirty(renderer);
        }

        EditorUtility.SetDirty(prefabRoot);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    private static void ConfigureActiveScene()
    {
        GameObject player = GameObject.Find("Player_Mipurin");

        if (player != null)
        {
            ConfigurePlayerVisuals(player);
            ConfigurePlayerBounds(player);
        }
        else
        {
            Debug.LogWarning("Player_Mipurin was not found in the active scene.");
        }

        GameObject enemy = GameObject.Find("Enemy_Test");

        if (enemy != null)
        {
            ConfigureEnemyVisuals(enemy, true);
        }
        else
        {
            Debug.LogWarning("Enemy_Test was not found in the active scene.");
        }

        ConfigureHud();
        ConfigureCamera(player != null ? player.transform : null);

        if (player != null)
        {
            EditorSceneManager.MarkSceneDirty(player.scene);
        }
        else if (enemy != null)
        {
            EditorSceneManager.MarkSceneDirty(enemy.scene);
        }
    }

    private static void ConfigurePlayerVisuals(GameObject player)
    {
        player.transform.localRotation = Quaternion.identity;
        player.transform.localScale = PlayerVisualScale;

        Transform body = player.transform.Find("Body");

        if (body != null)
        {
            body.localPosition = Vector3.zero;
            body.localRotation = Quaternion.identity;
            body.localScale = Vector3.one;

            SpriteRenderer bodyRenderer = body.GetComponent<SpriteRenderer>();

            if (bodyRenderer != null)
            {
                bodyRenderer.sortingOrder = 10;
                EditorUtility.SetDirty(bodyRenderer);
            }

            EditorUtility.SetDirty(body.gameObject);
        }

        Transform wings = player.transform.Find("Wings");

        if (wings != null)
        {
            wings.localPosition = WingsLocalPosition;
            wings.localRotation = Quaternion.identity;
            wings.localScale = WingsLocalScale;

            SpriteRenderer wingRenderer = wings.GetComponent<SpriteRenderer>();

            if (wingRenderer != null)
            {
                wingRenderer.sortingOrder = 9;
                EditorUtility.SetDirty(wingRenderer);
            }

            EditorUtility.SetDirty(wings.gameObject);
        }

        EditorUtility.SetDirty(player);
    }

    private static void ConfigurePlayerBounds(GameObject player)
    {
        PlayerBoundsLimiter limiter = player.GetComponent<PlayerBoundsLimiter>();

        if (limiter == null)
        {
            limiter = player.AddComponent<PlayerBoundsLimiter>();
        }

        limiter.Configure(PlayerMin, PlayerMax, true);
        EditorUtility.SetDirty(limiter);
    }

    private static void ConfigureEnemyVisuals(GameObject enemy, bool forceScenePosition)
    {
        if (forceScenePosition)
        {
            enemy.transform.position = EnemyScenePosition;
        }

        enemy.transform.localRotation = Quaternion.identity;
        enemy.transform.localScale = EnemyVisualScale;

        SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.sortingOrder = 8;
            EditorUtility.SetDirty(renderer);
        }

        CircleCollider2D circleCollider = enemy.GetComponent<CircleCollider2D>();

        if (circleCollider != null)
        {
            circleCollider.radius = 0.8f;
            EditorUtility.SetDirty(circleCollider);
        }

        EditorUtility.SetDirty(enemy);
    }

    private static void ConfigureHud()
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

        hud.ConfigureAppearance(new Color(0.22f, 0.12f, 0.04f, 1f), new Color(1f, 0.96f, 0.78f, 0.72f), 18);
        EditorUtility.SetDirty(hudObject);
        EditorUtility.SetDirty(hud);
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
