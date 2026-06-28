using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CharacterTestReturnToVillage : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "VillageTest";
    [SerializeField] private bool allowReturnAnytime = true;
    private bool isLoading;

    private void Update()
    {
        if (isLoading || !allowReturnAnytime)
        {
            return;
        }

        if (WasReturnPressed())
        {
            isLoading = true;
            SceneManager.LoadScene(targetSceneName);
        }
    }

    public void Configure(string sceneName, bool canReturnAnytime)
    {
        targetSceneName = string.IsNullOrWhiteSpace(sceneName) ? targetSceneName : sceneName;
        allowReturnAnytime = canReturnAnytime;
    }

    private bool WasReturnPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.vKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.V))
        {
            return true;
        }
#endif

        return false;
    }
}
