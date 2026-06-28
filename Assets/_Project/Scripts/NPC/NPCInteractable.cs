using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NPCInteractable : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private string npcName = "長老ハッチ";
    [SerializeField]
    [TextArea(2, 4)]
    private string[] dialogueLines =
    {
        "ミプリンよ、南の森に黄金蜂蜜のかけらがあるという噂じゃ。",
        "気をつけて行くのじゃぞ。",
        "この村の未来は、おぬしの小さな羽にかかっておる。"
    };

    [SerializeField]
    [TextArea(2, 4)]
    private string[] questStartedDialogueLines;

    [SerializeField]
    [TextArea(2, 4)]
    private string[] firstAdventureReturnedDialogueLines;

    [Header("Story")]
    [SerializeField] private bool startsFirstForestQuest;

    [Header("Interaction")]
    [SerializeField] private Transform player;
    [SerializeField] private float interactionRadius = 1.25f;
    [SerializeField] private string promptText = "E / Space：話す";

    private bool wasPromptVisible;

    private void Update()
    {
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

        dialogueManager.ShowInteractionPrompt($"{npcName}\n{promptText}");
        wasPromptVisible = true;

        if (WasInteractPressed())
        {
            OpenStoryAwareDialogue(dialogueManager);
        }
    }

    private void OpenStoryAwareDialogue(DialogueManager dialogueManager)
    {
        StoryProgress storyProgress = StoryProgress.Instance;
        string[] selectedLines = GetCurrentDialogueLines(storyProgress);

        dialogueManager.OpenDialogue(npcName, selectedLines);

        if (startsFirstForestQuest && storyProgress.CurrentStage == StoryStage.NotStarted)
        {
            storyProgress.StartFirstForestQuest();
        }
    }

    private string[] GetCurrentDialogueLines(StoryProgress storyProgress)
    {
        if (storyProgress != null && storyProgress.HasReturnedFromFirstAdventure && HasLines(firstAdventureReturnedDialogueLines))
        {
            return firstAdventureReturnedDialogueLines;
        }

        if (storyProgress != null && storyProgress.IsQuestStarted && HasLines(questStartedDialogueLines))
        {
            return questStartedDialogueLines;
        }

        return dialogueLines;
    }

    private bool HasLines(string[] lines)
    {
        return lines != null && lines.Length > 0;
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
        Gizmos.color = new Color(1f, 0.75f, 0.1f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    public void Configure(string targetNpcName, string[] targetDialogueLines, float targetInteractionRadius)
    {
        ConfigureStoryDialogue(targetNpcName, targetDialogueLines, null, null, targetInteractionRadius, false);
    }

    public void ConfigureStoryDialogue(string targetNpcName, string[] initialLines, string[] questLines, string[] returnedLines, float targetInteractionRadius, bool shouldStartFirstForestQuest)
    {
        npcName = targetNpcName;
        dialogueLines = initialLines;
        questStartedDialogueLines = questLines;
        firstAdventureReturnedDialogueLines = returnedLines;
        interactionRadius = Mathf.Max(0.1f, targetInteractionRadius);
        startsFirstForestQuest = shouldStartFirstForestQuest;
    }
}
