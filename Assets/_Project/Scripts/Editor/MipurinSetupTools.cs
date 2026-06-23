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

    private static readonly string[] WingIdleSpritePaths =
    {
        "Assets/_Project/Sprites/Player/Mipurin/Wings/wing_idle_01.png",
        "Assets/_Project/Sprites/Player/Mipurin/Wings/wing_idle_02.png"
    };

    private static readonly string[] WingFlapSpritePaths =
    {
        "Assets/_Project/Sprites/Player/Mipurin/Wings/wing_flap_01.png",
        "Assets/_Project/Sprites/Player/Mipurin/Wings/wing_flap_02.png",
        "Assets/_Project/Sprites/Player/Mipurin/Wings/wing_flap_03.png"
    };

    [MenuItem("Mipurin/Setup/Setup Player Animation")]
    public static void SetupPlayerAnimation()
    {
        SetupPrefab();
        SetupActiveSceneInstance();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Mipurin setup complete: body and wing animation configured.");
    }

    [MenuItem("Mipurin/Setup/Setup Player Sprite Animator")]
    public static void SetupPlayerSpriteAnimator()
    {
        SetupPlayerAnimation();
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

        Transform body = FindOrCreateChild(player.transform, "Body", Vector3.zero);
        SpriteRenderer bodyRenderer = FindOrCreateSpriteRenderer(body);
        bodyRenderer.sortingOrder = 10;

        Sprite[] idleSprites = LoadSprites(IdleSpritePaths);
        Sprite[] walkSprites = LoadSprites(WalkSpritePaths);

        if (idleSprites.Length > 0)
        {
            bodyRenderer.sprite = idleSprites[0];
        }

        Transform wings = FindOrCreateChild(player.transform, "Wings", new Vector3(0f, 0.35f, 0f));
        SpriteRenderer wingsRenderer = FindOrCreateSpriteRenderer(wings);
        wingsRenderer.sortingOrder = 9;

        Sprite[] wingIdleSprites = LoadSprites(WingIdleSpritePaths);
        Sprite[] wingFlapSprites = LoadSprites(WingFlapSpritePaths);

        if (wingIdleSprites.Length > 0)
        {
            wingsRenderer.sprite = wingIdleSprites[0];
        }

        PlayerController controller = player.GetComponent<PlayerController>();

        if (controller == null)
        {
            controller = player.AddComponent<PlayerController>();
        }

        PlayerSpriteAnimator bodyAnimator = player.GetComponent<PlayerSpriteAnimator>();

        if (bodyAnimator == null)
        {
            bodyAnimator = player.AddComponent<PlayerSpriteAnimator>();
        }

        bodyAnimator.Configure(bodyRenderer, idleSprites, walkSprites);

        PlayerWingAnimator wingAnimator = player.GetComponent<PlayerWingAnimator>();

        if (wingAnimator == null)
        {
            wingAnimator = player.AddComponent<PlayerWingAnimator>();
        }

        wingAnimator.Configure(wingsRenderer, wingIdleSprites, wingFlapSprites);

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(bodyRenderer);
        EditorUtility.SetDirty(wingsRenderer);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(bodyAnimator);
        EditorUtility.SetDirty(wingAnimator);
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
