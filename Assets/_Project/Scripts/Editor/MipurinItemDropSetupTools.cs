using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinItemDropSetupTools
{
    private const string PickupPrefabPath = "Assets/_Project/Prefabs/Items/Pickup_HoneyNectar.prefab";
    private const string PickupSpritePath = "Assets/_Project/Sprites/Effects/honey_spark_01.png";

    private const float PickupScale = 0.18f;
    private const float PickupRadius = 0.65f;
    private const float MagnetRadius = 2.2f;
    private const float MagnetSpeed = 4.8f;
    private const float PickupLifeTime = 20f;

    [MenuItem("Mipurin/Setup/Setup Nectar Pickup Drops")]
    public static void SetupNectarPickupDrops()
    {
        EnsureFolders();

        GameObject pickupPrefab = CreateOrUpdatePickupPrefab();
        ConfigurePlayer();
        ConfigureEnemies(pickupPrefab);
        ConfigureHud();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("Nectar pickup drop setup complete. Defeat enemies and collect honey nectar.");
    }

    private static void EnsureFolders()
    {
        Directory.CreateDirectory("Assets/_Project/Prefabs/Items");
        Directory.CreateDirectory("Assets/_Project/Scripts/Items");
        AssetDatabase.Refresh();
    }

    private static GameObject CreateOrUpdatePickupPrefab()
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PickupSpritePath);

        GameObject root = new GameObject("Pickup_HoneyNectar");
        root.transform.localScale = new Vector3(PickupScale, PickupScale, 1f);

        SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 25;
        renderer.sprite = sprite;

        PickupItem pickupItem = root.AddComponent<PickupItem>();
        pickupItem.Configure(1, PickupRadius, PickupLifeTime);
        pickupItem.ConfigureMagnet(MagnetRadius, MagnetSpeed);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, PickupPrefabPath);
        Object.DestroyImmediate(root);
        return savedPrefab;
    }

    private static void ConfigurePlayer()
    {
        GameObject player = GameObject.Find("Player_Mipurin");
        if (player == null)
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Player_Mipurin.prefab");
            if (playerPrefab != null)
            {
                ConfigurePlayerObject(playerPrefab);
            }
            return;
        }

        ConfigurePlayerObject(player);
    }

    private static void ConfigurePlayerObject(GameObject player)
    {
        NectarWallet wallet = player.GetComponent<NectarWallet>();
        if (wallet == null)
        {
            wallet = player.AddComponent<NectarWallet>();
        }

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(wallet);
    }

    private static void ConfigureEnemies(GameObject pickupPrefab)
    {
        ConfigureEnemyObject(GameObject.Find("Enemy_HoneySlime"), pickupPrefab, 1);
        ConfigureEnemyObject(GameObject.Find("Enemy_Test"), pickupPrefab, 1);
        ConfigureEnemyObject(GameObject.Find("Enemy_PoisonMushroom"), pickupPrefab, 2);

        ConfigureEnemyPrefab("Assets/_Project/Prefabs/Enemies/Enemy_HoneySlime.prefab", pickupPrefab, 1);
        ConfigureEnemyPrefab("Assets/_Project/Prefabs/Enemies/Enemy_Test.prefab", pickupPrefab, 1);
        ConfigureEnemyPrefab("Assets/_Project/Prefabs/Enemies/Enemy_PoisonMushroom.prefab", pickupPrefab, 2);
    }

    private static void ConfigureEnemyPrefab(string prefabPath, GameObject pickupPrefab, int nectarAmount)
    {
        if (!File.Exists(prefabPath))
        {
            return;
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        if (prefabRoot == null)
        {
            return;
        }

        ConfigureEnemyObject(prefabRoot, pickupPrefab, nectarAmount);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    private static void ConfigureEnemyObject(GameObject enemyObject, GameObject pickupPrefab, int nectarAmount)
    {
        if (enemyObject == null)
        {
            return;
        }

        EnemyDropper dropper = enemyObject.GetComponent<EnemyDropper>();
        if (dropper == null)
        {
            dropper = enemyObject.AddComponent<EnemyDropper>();
        }

        dropper.Configure(pickupPrefab, nectarAmount);
        EditorUtility.SetDirty(enemyObject);
        EditorUtility.SetDirty(dropper);
    }

    private static void ConfigureHud()
    {
        GameObject hudObject = GameObject.Find("MVP_DebugHUD");
        if (hudObject == null)
        {
            return;
        }

        MipurinDebugHud hud = hudObject.GetComponent<MipurinDebugHud>();
        if (hud != null)
        {
            EditorUtility.SetDirty(hud);
        }
    }
}
