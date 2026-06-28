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
        ApplyReadableLayout();
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

        ApplyReadableLayout();
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

    private void ApplyReadableLayout()
    {
        if (dialogueRoot != null)
        {
            RectTransform dialogueRect = dialogueRoot.GetComponent<RectTransform>();
            if (dialogueRect != null)
            {
                dialogueRect.anchorMin = new Vector2(0.06f, 0.03f);
                dialogueRect.anchorMax = new Vector2(0.94f, 0.36f);
                dialogueRect.offsetMin = Vector2.zero;
                dialogueRect.offsetMax = Vector2.zero;
            }
        }

        if (speakerText != null)
        {
            speakerText.fontSize = 25;
            speakerText.horizontalOverflow = HorizontalWrapMode.Wrap;
            speakerText.verticalOverflow = VerticalWrapMode.Overflow;
            SetRect(speakerText.rectTransform, new Vector2(0.04f, 0.72f), new Vector2(0.96f, 0.94f), Vector2.zero, Vector2.zero);
        }

        if (bodyText != null)
        {
            bodyText.fontSize = 21;
            bodyText.resizeTextForBestFit = true;
            bodyText.resizeTextMinSize = 16;
            bodyText.resizeTextMaxSize = 21;
            bodyText.horizontalOverflow = HorizontalWrapMode.Wrap;
            bodyText.verticalOverflow = VerticalWrapMode.Overflow;
            bodyText.lineSpacing = 0.9f;
            SetRect(bodyText.rectTransform, new Vector2(0.04f, 0.22f), new Vector2(0.96f, 0.72f), Vector2.zero, Vector2.zero);
        }

        if (hintText != null)
        {
            hintText.fontSize = 17;
            hintText.horizontalOverflow = HorizontalWrapMode.Wrap;
            hintText.verticalOverflow = VerticalWrapMode.Overflow;
            SetRect(hintText.rectTransform, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.20f), Vector2.zero, Vector2.zero);
        }

        if (promptRoot != null)
        {
            RectTransform promptRect = promptRoot.GetComponent<RectTransform>();
            if (promptRect != null)
            {
                promptRect.anchorMin = new Vector2(0.33f, 0.70f);
                promptRect.anchorMax = new Vector2(0.67f, 0.86f);
                promptRect.offsetMin = Vector2.zero;
                promptRect.offsetMax = Vector2.zero;
            }
        }

        if (promptText != null)
        {
            promptText.fontSize = 20;
            promptText.horizontalOverflow = HorizontalWrapMode.Wrap;
            promptText.verticalOverflow = VerticalWrapMode.Overflow;
        }
    }

    private void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
    }
}
