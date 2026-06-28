using UnityEngine;

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

    [Header("Interaction")]
    [SerializeField] private Transform player;
    [SerializeField] private float interactionRadius = 1.25f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode alternateInteractKey = KeyCode.Space;
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

        if (Input.GetKeyDown(interactKey) || Input.GetKeyDown(alternateInteractKey))
        {
            dialogueManager.OpenDialogue(npcName, dialogueLines);
        }
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
        npcName = targetNpcName;
        dialogueLines = targetDialogueLines;
        interactionRadius = Mathf.Max(0.1f, targetInteractionRadius);
    }
}
