using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinPlayerBoundsSetupTools
{
    [MenuItem("Mipurin/Setup/Setup Character Test Player Bounds")]
    public static void SetupCharacterTestPlayerBounds()
    {
        GameObject player = GameObject.Find("Player_Mipurin");
        if (player == null)
        {
            Debug.LogWarning("Player_Mipurin was not found.");
            return;
        }

        PlayerBoundsLimiter limiter = player.GetComponent<PlayerBoundsLimiter>();
        if (limiter == null)
        {
            limiter = player.AddComponent<PlayerBoundsLimiter>();
        }

        limiter.Configure(
            new Vector2(-4.65f, -2.15f),
            new Vector2(4.65f, 2.15f),
            true
        );

        Vector3 position = player.transform.position;
        position.x = Mathf.Clamp(position.x, -4.65f, 4.65f);
        position.y = Mathf.Clamp(position.y, -2.15f, 2.15f);
        player.transform.position = position;

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(limiter);
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("CharacterTest player bounds setup complete.");
    }
}
