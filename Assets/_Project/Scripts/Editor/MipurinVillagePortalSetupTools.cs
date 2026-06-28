using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MipurinVillagePortalSetupTools
{
    private const string VillageScenePath = "Assets/_Project/Scenes/VillageTest.unity";
    private const string CharacterTestScenePath = "Assets/_Project/Scenes/CharacterTest.unity";
    private const string SouthForestGatePrefabPath = "Assets/_Project/Prefabs/NPC/SouthForest_Gate.prefab";

    [MenuItem("Mipurin/Setup/Setup Village Portal To CharacterTest")]
    public static void SetupVillagePortalToCharacterTest()
    {
        EnsureScenesInBuildSettings();
        ConfigureSouthForestGatePrefab();
        ConfigureSouthForestGateInVillageScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Village portal setup complete. SouthForest_Gate now loads CharacterTest.");
    }

    [MenuItem("Mipurin/Setup/Setup Village Test With Portal")]
    public static void SetupVillageTestWithPortal()
    {
        MipurinVillageTestSetupTools.SetupVillageTest();
        SetupVillagePortalToCharacterTest();
    }

    private static void EnsureScenesInBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        AddSceneIfMissing(scenes, VillageScenePath);
        AddSceneIfMissing(scenes, CharacterTestScenePath);
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void AddSceneIfMissing(List<EditorBuildSettingsScene> scenes, string scenePath)
    {
        if (string.IsNullOrWhiteSpace(scenePath) || !AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath))
        {
            Debug.LogWarning("Scene not found for build settings: " + scenePath);
            return;
        }

        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (scene.path == scenePath)
            {
                scene.enabled = true;
                return;
            }
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
    }

    private static void ConfigureSouthForestGatePrefab()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SouthForestGatePrefabPath);
        if (prefab == null)
        {
            Debug.LogWarning("SouthForest_Gate prefab not found: " + SouthForestGatePrefabPath);
            return;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance == null)
        {
            return;
        }

        ConfigureGateObject(instance);
        PrefabUtility.SaveAsPrefabAsset(instance, SouthForestGatePrefabPath);
        Object.DestroyImmediate(instance);
    }

    private static void ConfigureSouthForestGateInVillageScene()
    {
        Scene scene = EditorSceneManager.OpenScene(VillageScenePath, OpenSceneMode.Single);
        GameObject gate = GameObject.Find("SouthForest_Gate");
        if (gate == null)
        {
            Debug.LogWarning("SouthForest_Gate was not found in VillageTest scene.");
            return;
        }

        ConfigureGateObject(gate);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void ConfigureGateObject(GameObject gate)
    {
        NPCInteractable npcInteractable = gate.GetComponent<NPCInteractable>();
        if (npcInteractable != null)
        {
            Object.DestroyImmediate(npcInteractable, true);
        }

        ScenePortalInteractable portal = gate.GetComponent<ScenePortalInteractable>();
        if (portal == null)
        {
            portal = gate.AddComponent<ScenePortalInteractable>();
        }

        portal.Configure("南の森への道", "CharacterTest", "E / Space：南の森へ行く", 1.45f);
    }
}
