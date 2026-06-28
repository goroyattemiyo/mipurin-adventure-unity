using UnityEngine;
using UnityEngine.UI;

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

    [Header("Input")]
    [SerializeField] private KeyCode nextKey = KeyCode.Space;
    [SerializeField] private KeyCode alternateNextKey = KeyCode.E;
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;

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

        if (Input.GetKeyDown(closeKey))
        {
            CloseDialogue();
            return;
        }

        if (Input.GetKeyDown(nextKey) || Input.GetKeyDown(alternateNextKey) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
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
