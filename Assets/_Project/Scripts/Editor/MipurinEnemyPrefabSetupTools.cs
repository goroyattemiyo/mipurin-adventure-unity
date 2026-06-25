using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinEnemyPrefabSetupTools
{
    private const string HoneySlimePrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_HoneySlime.prefab";
    private const string HoneySlimeName = "Enemy_HoneySlime";
    private const string LegacyEnemyName = "Enemy_Test";

    [MenuItem("Mipurin/Setup/Formalize HoneySlime Enemy")]
    public static void FormalizeHoneySlimeEnemy()
    {
        Directory.CreateDirectory("Assets/_Project/Prefabs/Enemies");
        AssetDatabase.Refresh();

        GameObject sceneEnemy = GameObject.Find(HoneySlimeName);

        if (sceneEnemy == null)
        {
            sceneEnemy = GameObject.Find(LegacyEnemyName);
        }

        if (sceneEnemy == null)
        {
            Debug.LogWarning("Enemy_Test or Enemy_HoneySlime was not found in the active scene. Run Setup MVP Combat first.");
            return;
        }

        sceneEnemy.name = HoneySlimeName;
        sceneEnemy.transform.position = new Vector3(3.2f, -0.2f, 0f);
        sceneEnemy.transform.localRotation = Quaternion.identity;
        sceneEnemy.transform.localScale = new Vector3(0.2f, 0.2f, 1f);

        SpriteRenderer renderer = sceneEnemy.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 8;
            EditorUtility.SetDirty(renderer);
        }

        CircleCollider2D circleCollider = sceneEnemy.GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            circleCollider.radius = 0.8f;
            circleCollider.isTrigger = true;
            EditorUtility.SetDirty(circleCollider);
        }

        PrefabUtility.SaveAsPrefabAsset(sceneEnemy, HoneySlimePrefabPath);
        EditorUtility.SetDirty(sceneEnemy);
        EditorSceneManager.MarkSceneDirty(sceneEnemy.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"HoneySlime enemy formalized: {HoneySlimePrefabPath}");
    }
}
