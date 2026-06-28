using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private Text speakerText;
    [SerializeField] private Text bodyText;
    [SerializeField] private Text hintText;

    [Header("Prompt UI")]
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private Text promptText;

    private string speakerName;
    private string[] lines;
    private int currentLineIndex;
    private bool isOpen;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CloseDialogue();
        HideInteractionPrompt();
    }

    private void Update()
    {
        if (!isOpen)
        {
            return;
        }

        if (WasClosePressed())
        {
            CloseDialogue();
            return;
        }

        if (WasNextPressed())
        {
            ShowNextLine();
        }
    }

    private bool WasNextPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        bool keyboardPressed = keyboard != null &&
            (keyboard.spaceKey.wasPressedThisFrame || keyboard.eKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame);
        bool mousePressed = mouse != null && mouse.leftButton.wasPressedThisFrame;

        if (keyboardPressed || mousePressed)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            return true;
        }
#endif

        return false;
    }

    private bool WasClosePressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            return true;
        }
#endif

        return false;
    }

    public void ConfigureReferences(GameObject targetDialogueRoot, Text targetSpeakerText, Text targetBodyText, Text targetHintText, GameObject targetPromptRoot, Text targetPromptText)
    {
        dialogueRoot = targetDialogueRoot;
        speakerText = targetSpeakerText;
        bodyText = targetBodyText;
        hintText = targetHintText;
        promptRoot = targetPromptRoot;
        promptText = targetPromptText;

        CloseDialogue();
        HideInteractionPrompt();
    }

    public void OpenDialogue(string targetSpeakerName, string[] targetLines)
    {
        speakerName = string.IsNullOrWhiteSpace(targetSpeakerName) ? "？？？" : targetSpeakerName;
        lines = targetLines != null && targetLines.Length > 0 ? targetLines : new[] { "……" };
        currentLineIndex = 0;
        isOpen = true;

        if (dialogueRoot != null)
        {
            dialogueRoot.SetActive(true);
        }

        HideInteractionPrompt();
        RenderCurrentLine();
    }

    public void ShowNextLine()
    {
        if (!isOpen)
        {
            return;
        }

        currentLineIndex++;
        if (lines == null || currentLineIndex >= lines.Length)
        {
            CloseDialogue();
            return;
        }

        RenderCurrentLine();
    }

    public void CloseDialogue()
    {
        isOpen = false;
        currentLineIndex = 0;

        if (dialogueRoot != null)
        {
            dialogueRoot.SetActive(false);
        }
    }

    public void ShowInteractionPrompt(string message)
    {
        if (promptRoot != null)
        {
            promptRoot.SetActive(true);
        }

        if (promptText != null)
        {
            promptText.text = message;
        }
    }

    public void HideInteractionPrompt()
    {
        if (promptRoot != null)
        {
            promptRoot.SetActive(false);
        }
    }

    private void RenderCurrentLine()
    {
        if (speakerText != null)
        {
            speakerText.text = speakerName;
        }

        if (bodyText != null)
        {
            bodyText.text = lines[currentLineIndex];
        }

        if (hintText != null)
        {
            bool isLastLine = currentLineIndex >= lines.Length - 1;
            hintText.text = isLastLine ? "Space / E：閉じる" : "Space / E：次へ";
        }
    }
}
