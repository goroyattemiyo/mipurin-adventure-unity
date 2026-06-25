using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinPoisonMushroomSetupTools
{
    private const string EnemyName = "Enemy_PoisonMushroom";
    private const string PrefabPath = "Assets/_Project/Prefabs/Enemies/Enemy_PoisonMushroom.prefab";

    [MenuItem("Mipurin/Setup/Setup PoisonMushroom Enemy")]
    public static void SetupPoisonMushroomEnemy()
    {
        Directory.CreateDirectory("Assets/_Project/Prefabs/Enemies");
        AssetDatabase.Refresh();

        GameObject enemyObject = GameObject.Find(EnemyName);

        if (enemyObject == null)
        {
            enemyObject = new GameObject(EnemyName);
        }

        ConfigureEnemy(enemyObject);
        PrefabUtility.SaveAsPrefabAsset(enemyObject, PrefabPath);

        EditorUtility.SetDirty(enemyObject);
        EditorSceneManager.MarkSceneDirty(enemyObject.scene);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"PoisonMushroom enemy setup complete: {PrefabPath}");
    }

    private static void ConfigureEnemy(GameObject enemyObject)
    {
        Sprite idle01 = LoadSprite("Assets/_Project/Sprites/Enemies/PoisonMushroom/enemy_poison_mushroom_idle_01.png");
        Sprite idle02 = LoadSprite("Assets/_Project/Sprites/Enemies/PoisonMushroom/enemy_poison_mushroom_idle_02.png");
        Sprite hurt = LoadSprite("Assets/_Project/Sprites/Enemies/PoisonMushroom/enemy_poison_mushroom_hurt_01.png");
        Sprite down = LoadSprite("Assets/_Project/Sprites/Enemies/PoisonMushroom/enemy_poison_mushroom_down_01.png");
        Sprite[] idleSprites = idle02 != null ? new[] { idle01, idle02 } : new[] { idle01 };

        enemyObject.transform.position = new Vector3(-3.2f, -0.2f, 0f);
        enemyObject.transform.localRotation = Quaternion.identity;
        enemyObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);

        SpriteRenderer renderer = enemyObject.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = enemyObject.AddComponent<SpriteRenderer>();
        }
        renderer.sortingOrder = 8;
        renderer.sprite = idle01;

        CircleCollider2D circleCollider = enemyObject.GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            circleCollider = enemyObject.AddComponent<CircleCollider2D>();
        }
        circleCollider.isTrigger = true;
        circleCollider.radius = 0.8f;

        EnemySpriteAnimator spriteAnimator = enemyObject.GetComponent<EnemySpriteAnimator>();
        if (spriteAnimator == null)
        {
            spriteAnimator = enemyObject.AddComponent<EnemySpriteAnimator>();
        }
        spriteAnimator.Configure(renderer, idleSprites, hurt, down, 2f, 0.18f);

        MipurinEnemy enemy = enemyObject.GetComponent<MipurinEnemy>();
        if (enemy == null)
        {
            enemy = enemyObject.AddComponent<MipurinEnemy>();
        }
        enemy.ConfigureHp(4, true);
        enemy.ConfigureSprites(renderer, idleSprites, hurt, down);
        enemy.ConfigureHitReaction(0.55f, 0.25f, new Color(0.55f, 0.25f, 1f, 1f), 0.12f);

        GameObject player = GameObject.Find("Player_Mipurin");
        if (player != null)
        {
            enemy.SetTarget(player.transform);
        }

        MipurinContactDamage contactDamage = enemyObject.GetComponent<MipurinContactDamage>();
        if (contactDamage == null)
        {
            contactDamage = enemyObject.AddComponent<MipurinContactDamage>();
        }
        contactDamage.Configure(player != null ? player.GetComponent<MipurinHealth>() : null, 1, 1.6f, 0.85f);
        contactDamage.ResetDamageTimer();

        EditorUtility.SetDirty(enemyObject);
        EditorUtility.SetDirty(renderer);
        EditorUtility.SetDirty(circleCollider);
        EditorUtility.SetDirty(spriteAnimator);
        EditorUtility.SetDirty(enemy);
        EditorUtility.SetDirty(contactDamage);
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
