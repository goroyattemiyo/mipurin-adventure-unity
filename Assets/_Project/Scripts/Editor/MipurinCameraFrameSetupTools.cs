using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinCameraFrameSetupTools
{
    [MenuItem("Mipurin/Setup/Setup Character Test Camera Frame")]
    public static void SetupCharacterTestCameraFrame()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera was not found.");
            return;
        }

        GameObject player = GameObject.Find("Player_Mipurin");
        Transform target = player != null ? player.transform : null;

        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 3.2f;
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color(0.66f, 0.86f, 0.96f, 1f);

        SimpleCameraFollow follow = mainCamera.GetComponent<SimpleCameraFollow>();
        if (follow == null)
        {
            follow = mainCamera.gameObject.AddComponent<SimpleCameraFollow>();
        }

        follow.Configure(
            target,
            new Vector3(0f, 0f, -10f),
            8f,
            true,
            new Vector2(-0.55f, -0.2f),
            new Vector2(0.55f, 0.2f)
        );

        if (target != null)
        {
            mainCamera.transform.position = new Vector3(target.position.x, target.position.y, -10f);
            follow.SnapToTarget();
        }
        else
        {
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        }

        EditorUtility.SetDirty(mainCamera);
        EditorUtility.SetDirty(mainCamera.gameObject);
        EditorUtility.SetDirty(follow);
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("CharacterTest camera frame setup complete.");
    }
}
