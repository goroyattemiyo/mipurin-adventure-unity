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

        if (ShouldStartFirstForestQuest() && storyProgress.CurrentStage == StoryStage.NotStarted)
        {
            storyProgress.StartFirstForestQuest();
        }
    }

    private bool ShouldStartFirstForestQuest()
    {
        if (startsFirstForestQuest)
        {
            return true;
        }

        return npcName.Contains("長老ハッチ") || npcName.Contains("ハッチ");
    }

    private string[] GetCurrentDialogueLines(StoryProgress storyProgress)
    {
        if (storyProgress != null && storyProgress.HasReturnedFromFirstAdventure)
        {
            if (HasLines(firstAdventureReturnedDialogueLines))
            {
                return firstAdventureReturnedDialogueLines;
            }

            return GetFallbackReturnedDialogueLines();
        }

        if (storyProgress != null && storyProgress.IsQuestStarted)
        {
            if (HasLines(questStartedDialogueLines))
            {
                return questStartedDialogueLines;
            }

            return GetFallbackQuestStartedDialogueLines();
        }

        return dialogueLines;
    }

    private string[] GetFallbackQuestStartedDialogueLines()
    {
        if (npcName.Contains("ハッチ"))
        {
            return new[]
            {
                "南の森へ向かうのじゃ、ミプリン。",
                "黄金蜂蜜は、やさしい記憶から生まれる特別な力。",
                "闇胞子には十分気をつけるのじゃぞ。"
            };
        }

        if (npcName.Contains("掲示板"))
        {
            return new[]
            {
                "現在の目標：南の森を調べる。",
                "南の森への道から出発できます。"
            };
        }

        return dialogueLines;
    }

    private string[] GetFallbackReturnedDialogueLines()
    {
        if (npcName.Contains("ハッチ"))
        {
            return new[]
            {
                "戻ったか、ミプリン。無事で何よりじゃ。",
                "森の奥で何かが動き始めておる。黄金蜂蜜のかけらは近いかもしれん。",
                "次は、森で見たものを村のみんなにも聞いてみるとよい。"
            };
        }

        if (npcName.Contains("ミエル"))
        {
            return new[] { "森の風が変わったわ。", "あなたが戻ったことで、細い記憶の糸が村へつながったみたい。" };
        }

        if (npcName.Contains("マルシェ"))
        {
            return new[] { "おかえり、ミプリンちゃん。顔を見るだけで安心するよ。", "次は小さなはちみつ瓶くらい用意しておきたいね。" };
        }

        if (npcName.Contains("ビー"))
        {
            return new[] { "本当に森へ行ってきたの？すごい！", "ぼくもいつか、ミプリンみたいに飛べるかな。" };
        }

        if (npcName.Contains("ポーレ"))
        {
            return new[] { "あなたが森から戻って、花壇の色が少し明るくなった気がするの。", "記憶と蜂蜜は、やっぱりつながっているのね。" };
        }

        if (npcName.Contains("ナビィ"))
        {
            return new[] { "村に戻る流れも確認できたね。", "次は森で何を持ち帰るか決める段階だよ。" };
        }

        if (npcName.Contains("グランパ"))
        {
            return new[] { "戻ったか。あの森の空気を知った目をしておる。", "父タイガと母ハナの話も、いずれせねばならんのう。" };
        }

        if (npcName.Contains("掲示板"))
        {
            return new[]
            {
                "現在の目標：森から戻ったことを村のみんなに報告する。",
                "次の実装候補：黄金蜂蜜のかけらAを入手する。"
            };
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
