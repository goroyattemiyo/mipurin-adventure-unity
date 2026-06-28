using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MipurinCharacterTestVillageSetupTools
{
    private const string CharacterTestScenePath = "Assets/_Project/Scenes/CharacterTest.unity";

    [MenuItem("Mipurin/Setup/Setup CharacterTest Village Return")]
    public static void SetupCharacterTestVillageReturn()
    {
        Scene scene = EditorSceneManager.OpenScene(CharacterTestScenePath, OpenSceneMode.Single);
        GameObject root = GameObject.Find("CharacterTest_VillageReturn");
        if (root == null)
        {
            root = new GameObject("CharacterTest_VillageReturn");
        }

        CharacterTestReturnToVillage controller = root.GetComponent<CharacterTestReturnToVillage>();
        if (controller == null)
        {
            controller = root.AddComponent<CharacterTestReturnToVillage>();
        }

        controller.Configure("VillageTest", true);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("CharacterTest village return setup complete.");
    }
}
