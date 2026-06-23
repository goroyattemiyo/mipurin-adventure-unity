using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinSetupTools
{
    private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Player_Mipurin.prefab";

    private static readonly string[] IdleSpritePaths =
    {
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_idle_01.png",
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_idle_02.png"
    };

    private static readonly string[] WalkSpritePaths =
    {
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_walk_01.png",
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_walk_02.png",
        "Assets/_Project/Sprites/Player/Mipurin/Body/front_walk_03.png"
    };

    [MenuItem("Mipurin/Setup/Setup Player Sprite Animator")]
    public static void SetupPlayerSpriteAnimator()
    {
        SetupPrefab();
        SetupActiveSceneInstance();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Mipurin setup complete: PlayerSpriteAnimator configured.");
    }

    private static void SetupPrefab()
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);

        if (prefabRoot == null)
        {
            Debug.LogError($"Prefab not found: {PlayerPrefabPath}");
            return;
        }

        ConfigurePlayer(prefabRoot);

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, PlayerPrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Debug.Log($"Updated prefab: {PlayerPrefabPath}");
    }

    private static void SetupActiveSceneInstance()
    {
        GameObject scenePlayer = GameObject.Find("Player_Mipurin");

        if (scenePlayer == null)
        {
            Debug.LogWarning("Active scene does not contain Player_Mipurin. Prefab was updated only.");
            return;
        }

        ConfigurePlayer(scenePlayer);
        EditorUtility.SetDirty(scenePlayer);
        EditorSceneManager.MarkSceneDirty(scenePlayer.scene);
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("Updated active scene instance: Player_Mipurin");
    }

    private static void ConfigurePlayer(GameObject player)
    {
        if (player == null)
        {
            return;
        }

        Transform body = player.transform.Find("Body");

        if (body == null)
        {
            GameObject bodyObject = new GameObject("Body");
            bodyObject.transform.SetParent(player.transform);
            bodyObject.transform.localPosition = Vector3.zero;
            bodyObject.transform.localRotation = Quaternion.identity;
            bodyObject.transform.localScale = Vector3.one;
            body = bodyObject.transform;
        }

        SpriteRenderer bodyRenderer = body.GetComponent<SpriteRenderer>();

        if (bodyRenderer == null)
        {
            bodyRenderer = body.gameObject.AddComponent<SpriteRenderer>();
        }

        bodyRenderer.sortingOrder = 10;

        Sprite[] idleSprites = LoadSprites(IdleSpritePaths);
        Sprite[] walkSprites = LoadSprites(WalkSpritePaths);

        if (idleSprites.Length > 0)
        {
            bodyRenderer.sprite = idleSprites[0];
        }

        PlayerController controller = player.GetComponent<PlayerController>();

        if (controller == null)
        {
            controller = player.AddComponent<PlayerController>();
        }

        PlayerSpriteAnimator animator = player.GetComponent<PlayerSpriteAnimator>();

        if (animator == null)
        {
            animator = player.AddComponent<PlayerSpriteAnimator>();
        }

        animator.Configure(bodyRenderer, idleSprites, walkSprites);

        Transform wings = player.transform.Find("Wings");

        if (wings != null)
        {
            SpriteRenderer wingsRenderer = wings.GetComponent<SpriteRenderer>();

            if (wingsRenderer != null)
            {
                wingsRenderer.sortingOrder = 9;
            }
        }

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(bodyRenderer);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(animator);
    }

    private static Sprite[] LoadSprites(string[] paths)
    {
        List<Sprite> sprites = new List<Sprite>();

        foreach (string path in paths)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"Sprite not found: {path}");
                continue;
            }

            sprites.Add(sprite);
        }

        return sprites.ToArray();
    }
}
