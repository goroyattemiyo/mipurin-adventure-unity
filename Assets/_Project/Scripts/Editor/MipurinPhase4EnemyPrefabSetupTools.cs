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

    [MenuItem("Mipurin/Setup/Setup Phase 4 Enemy Prefabs")]
    public static void SetupPhase4EnemyPrefabs()
    {
        EnsureFolder("Assets/_Project/Prefabs");
        EnsureFolder(EnemyPrefabFolder);

        GameObject honeyBase = AssetDatabase.LoadAssetAtPath<GameObject>(HoneySlimePrefabPath);
        GameObject mushroomBase = AssetDatabase.LoadAssetAtPath<GameObject>(PoisonMushroomPrefabPath);

        if (honeyBase == null || mushroomBase == null)
        {
            Debug.LogError("Phase 4 enemy setup failed. Base enemy prefabs are missing.");
            return;
        }

        CreateEnemyPrefabFromBase(honeyBase, StingerBeePrefabPath, "Enemy_StingerBee", "StingerBee", new Vector3(0.34f, 0.34f, 1f));
        CreateEnemyPrefabFromBase(mushroomBase, FlowerTurretPrefabPath, "Enemy_FlowerTurret", "FlowerTurret", new Vector3(0.48f, 0.48f, 1f));
        CreateEnemyPrefabFromBase(honeyBase, HeavyBeetlePrefabPath, "Enemy_HeavyBeetle", "HeavyBeetle", new Vector3(0.58f, 0.58f, 1f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Phase 4 enemy prefabs setup complete.");
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
