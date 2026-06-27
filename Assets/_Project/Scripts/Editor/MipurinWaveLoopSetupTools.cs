using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinWaveLoopSetupTools
{
    private const string HoneySlimePrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_HoneySlime.prefab";
    private const string PoisonMushroomPrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_PoisonMushroom.prefab";

    [MenuItem("Mipurin/Setup/Setup Character Test Wave Loop")]
    public static void SetupCharacterTestWaveLoop()
    {
        GameObject honeyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HoneySlimePrefabPath);
        GameObject mushroomPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PoisonMushroomPrefabPath);

        RemoveSceneEnemies();

        CharacterTestEnemySpawner spawner = CreateOrUpdateSpawner(honeyPrefab, mushroomPrefab);
        NectarWallet wallet = EnsurePlayerWallet();
        ConfigureWaveHud(spawner, wallet);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("CharacterTest wave loop setup complete.");
    }

    private static CharacterTestEnemySpawner CreateOrUpdateSpawner(GameObject honeyPrefab, GameObject mushroomPrefab)
    {
        GameObject root = GameObject.Find("CharacterTest_WaveLoop");
        if (root == null)
        {
            root = new GameObject("CharacterTest_WaveLoop");
        }

        CharacterTestEnemySpawner spawner = root.GetComponent<CharacterTestEnemySpawner>();
        if (spawner == null)
        {
            spawner = root.AddComponent<CharacterTestEnemySpawner>();
        }

        Transform[] points = CreateSpawnPoints(root.transform);
        spawner.Configure(honeyPrefab, mushroomPrefab, points, 10, 5, 2f);

        EditorUtility.SetDirty(root);
        EditorUtility.SetDirty(spawner);
        return spawner;
    }

    private static Transform[] CreateSpawnPoints(Transform root)
    {
        Transform oldRoot = root.Find("SpawnPoints");
        if (oldRoot != null)
        {
            Object.DestroyImmediate(oldRoot.gameObject);
        }

        GameObject spawnRootObject = new GameObject("SpawnPoints");
        spawnRootObject.transform.SetParent(root);
        spawnRootObject.transform.localPosition = Vector3.zero;

        Vector3[] positions =
        {
            new Vector3(2.9f, 0.7f, 0f),
            new Vector3(-2.9f, 0.7f, 0f),
            new Vector3(2.4f, -1.2f, 0f),
            new Vector3(-2.4f, -1.2f, 0f),
            new Vector3(0f, 1.35f, 0f)
        };

        List<Transform> points = new List<Transform>();
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject point = new GameObject("SpawnPoint_" + (i + 1));
            point.transform.SetParent(spawnRootObject.transform);
            point.transform.position = positions[i];
            points.Add(point.transform);
        }

        return points.ToArray();
    }

    private static NectarWallet EnsurePlayerWallet()
    {
        GameObject player = GameObject.Find("Player_Mipurin");
        if (player == null)
        {
            return null;
        }

        NectarWallet wallet = player.GetComponent<NectarWallet>();
        if (wallet == null)
        {
            wallet = player.AddComponent<NectarWallet>();
        }

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(wallet);
        return wallet;
    }

    private static void ConfigureWaveHud(CharacterTestEnemySpawner spawner, NectarWallet wallet)
    {
        GameObject hudObject = GameObject.Find("MVP_DebugHUD");
        if (hudObject == null)
        {
            hudObject = new GameObject("MVP_DebugHUD");
        }

        CharacterTestWaveHud waveHud = hudObject.GetComponent<CharacterTestWaveHud>();
        if (waveHud == null)
        {
            waveHud = hudObject.AddComponent<CharacterTestWaveHud>();
        }

        waveHud.Configure(spawner, wallet);
        EditorUtility.SetDirty(hudObject);
        EditorUtility.SetDirty(waveHud);
    }

    private static void RemoveSceneEnemies()
    {
        MipurinEnemy[] enemies = Object.FindObjectsOfType<MipurinEnemy>();
        foreach (MipurinEnemy enemy in enemies)
        {
            if (enemy != null)
            {
                Object.DestroyImmediate(enemy.gameObject);
            }
        }
    }
}
