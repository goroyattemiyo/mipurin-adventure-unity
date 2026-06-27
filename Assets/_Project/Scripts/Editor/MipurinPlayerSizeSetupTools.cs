using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinPlayerSizeSetupTools
{
    private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player_Mipurin.prefab";

    private static readonly Vector3 TargetPlayerScale = new Vector3(0.18f, 0.18f, 1f);
    private static readonly Vector3 TargetWingLocalPosition = new Vector3(0f, 0.14f, 0f);
    private static readonly Vector3 TargetWingLocalScale = new Vector3(0.78f, 0.78f, 1f);

    private const float TargetAttackRadius = 0.72f;
    private const float TargetAttackForwardOffset = 0.86f;
    private const float TargetAttackEffectScale = 0.08f;
    private const float TargetHitEffectScale = 0.2f;

    [MenuItem("Mipurin/Setup/Setup Character Test Player Size")]
    public static void SetupCharacterTestPlayerSize()
    {
        ConfigurePlayerPrefab();
        ConfigureScenePlayer();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("CharacterTest player size setup complete.");
    }

    private static void ConfigurePlayerPrefab()
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
        if (prefabRoot == null)
        {
            Debug.LogWarning("Player prefab not found: " + PlayerPrefabPath);
            return;
        }

        ConfigurePlayer(prefabRoot);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, PlayerPrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    private static void ConfigureScenePlayer()
    {
        GameObject player = GameObject.Find("Player_Mipurin");
        if (player == null)
        {
            Debug.LogWarning("Player_Mipurin was not found in the active scene.");
            return;
        }

        ConfigurePlayer(player);
        EditorSceneManager.MarkSceneDirty(player.scene);
    }

    private static void ConfigurePlayer(GameObject player)
    {
        player.transform.localScale = TargetPlayerScale;

        Transform wings = player.transform.Find("Wings");
        if (wings != null)
        {
            wings.localPosition = TargetWingLocalPosition;
            wings.localScale = TargetWingLocalScale;
            EditorUtility.SetDirty(wings.gameObject);
        }

        MipurinAttack attack = player.GetComponent<MipurinAttack>();
        if (attack != null)
        {
            SerializedObject serializedAttack = new SerializedObject(attack);
            SetFloat(serializedAttack, "attackRadius", TargetAttackRadius);
            SetFloat(serializedAttack, "forwardOffset", TargetAttackForwardOffset);
            SetFloat(serializedAttack, "attackEffectScale", TargetAttackEffectScale);
            SetFloat(serializedAttack, "hitEffectScale", TargetHitEffectScale);
            serializedAttack.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(attack);
        }

        EditorUtility.SetDirty(player);
    }

    private static void SetFloat(SerializedObject serializedObject, string propertyName, float value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.floatValue = value;
        }
    }
}
