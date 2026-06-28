using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ScenePortalInteractable : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] private string portalName = "南の森への道";
    [SerializeField] private string targetSceneName = "CharacterTest";
    [SerializeField] private string promptText = "E / Space：南の森へ行く";

    [Header("Interaction")]
    [SerializeField] private Transform player;
    [SerializeField] private float interactionRadius = 1.35f;

    private bool wasPromptVisible;
    private bool isLoading;

    private void Update()
    {
        if (isLoading)
        {
            return;
        }

        EnsurePlayer();

        DialogueManager dialogueManager = DialogueManager.Instance;
        if (dialogueManager == null || player == null)
        {
            return;
        }

        if (dialogueManager.IsOpen)
        {
            HidePromptIfNeeded(dialogueManager);
            return;
        }

        bool isInRange = Vector2.Distance(transform.position, player.position) <= interactionRadius;
        if (!isInRange)
        {
            HidePromptIfNeeded(dialogueManager);
            return;
        }

        dialogueManager.ShowInteractionPrompt($"{portalName}\n{promptText}");
        wasPromptVisible = true;

        if (WasInteractPressed())
        {
            LoadTargetScene(dialogueManager);
        }
    }

    public void Configure(string targetPortalName, string targetScene, string targetPrompt, float radius)
    {
        portalName = string.IsNullOrWhiteSpace(targetPortalName) ? portalName : targetPortalName;
        targetSceneName = string.IsNullOrWhiteSpace(targetScene) ? targetSceneName : targetScene;
        promptText = string.IsNullOrWhiteSpace(targetPrompt) ? promptText : targetPrompt;
        interactionRadius = Mathf.Max(0.1f, radius);
    }

    private void LoadTargetScene(DialogueManager dialogueManager)
    {
        isLoading = true;
        HidePromptIfNeeded(dialogueManager);

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning("ScenePortalInteractable targetSceneName is empty.");
            isLoading = false;
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }

    private bool WasInteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.eKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame))
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }
#endif

        return false;
    }

    private void EnsurePlayer()
    {
        if (player != null)
        {
            return;
        }

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
    }

    private void HidePromptIfNeeded(DialogueManager dialogueManager)
    {
        if (!wasPromptVisible)
        {
            return;
        }

        dialogueManager.HideInteractionPrompt();
        wasPromptVisible = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.95f, 0.35f, 0.45f);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
