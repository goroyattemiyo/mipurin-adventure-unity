using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MipurinStoryProgressSetupTools
{
    private const string VillageScenePath = "Assets/_Project/Scenes/VillageTest.unity";
    private const string SouthForestGatePrefabPath = "Assets/_Project/Prefabs/NPC/SouthForest_Gate.prefab";

    [MenuItem("Mipurin/Setup/Setup Story Progress Prototype")]
    public static void SetupStoryProgressPrototype()
    {
        ConfigureVillageScene();
        ConfigureSouthForestGatePrefab();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("StoryProgress prototype setup complete.");
    }

    private static void ConfigureVillageScene()
    {
        Scene scene = EditorSceneManager.OpenScene(VillageScenePath, OpenSceneMode.Single);
        ConfigureNpc("NPC_Hatch", "長老ハッチ", InitialHatch(), QuestHatch(), ReturnedHatch(), true);
        ConfigureNpc("NPC_Miel", "占い師ミエル", InitialMiel(), QuestMiel(), ReturnedMiel(), false);
        ConfigureNpc("NPC_Marche", "商人マルシェ", InitialMarche(), QuestMarche(), ReturnedMarche(), false);
        ConfigureNpc("NPC_Bee", "ビー", InitialBee(), QuestBee(), ReturnedBee(), false);
        ConfigureNpc("NPC_Polle", "ポーレ", InitialPolle(), QuestPolle(), ReturnedPolle(), false);
        ConfigureNpc("NPC_Navii", "ナビィ", InitialNavii(), QuestNavii(), ReturnedNavii(), false);
        ConfigureNpc("NPC_Grandpa", "グランパ", InitialGrandpa(), QuestGrandpa(), ReturnedGrandpa(), false);
        ConfigureNpc("Village_Board", "村の掲示板", InitialBoard(), QuestBoard(), ReturnedBoard(), false);
        ConfigureSouthForestGateObject(GameObject.Find("SouthForest_Gate"));

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void ConfigureSouthForestGatePrefab()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SouthForestGatePrefabPath);
        if (prefab == null)
        {
            return;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance == null)
        {
            return;
        }

        ConfigureSouthForestGateObject(instance);
        PrefabUtility.SaveAsPrefabAsset(instance, SouthForestGatePrefabPath);
        Object.DestroyImmediate(instance);
    }

    private static void ConfigureNpc(string objectName, string displayName, string[] initialLines, string[] questLines, string[] returnedLines, bool startsQuest)
    {
        GameObject npcObject = GameObject.Find(objectName);
        if (npcObject == null)
        {
            return;
        }

        NPCInteractable npc = npcObject.GetComponent<NPCInteractable>();
        if (npc == null)
        {
            npc = npcObject.AddComponent<NPCInteractable>();
        }

        npc.ConfigureStoryDialogue(displayName, initialLines, questLines, returnedLines, 1.25f, startsQuest);
    }

    private static void ConfigureSouthForestGateObject(GameObject gate)
    {
        if (gate == null)
        {
            return;
        }

        NPCInteractable npc = gate.GetComponent<NPCInteractable>();
        if (npc != null)
        {
            Object.DestroyImmediate(npc, true);
        }

        ScenePortalInteractable portal = gate.GetComponent<ScenePortalInteractable>();
        if (portal == null)
        {
            portal = gate.AddComponent<ScenePortalInteractable>();
        }

        portal.Configure("南の森への道", "CharacterTest", "E / Space：南の森へ行く", 1.45f);
        portal.ConfigureStoryGate(true, "E / Space：まだ行けない", new[]
        {
            "南の森へ続く道だ。",
            "でも、まだ何を調べればいいのか分からない。",
            "まずは長老ハッチに話を聞いてみよう。"
        });
    }

    private static string[] InitialHatch() => new[]
    {
        "ミプリンよ、南の森に黄金蜂蜜のかけらがあるという噂じゃ。",
        "女王レイラが姿を消してから、森の色が少しずつ薄れておる。",
        "小さな羽でよい。まずは森の入口を調べてきておくれ。"
    };

    private static string[] QuestHatch() => new[]
    {
        "南の森へ向かうのじゃ、ミプリン。",
        "黄金蜂蜜は、やさしい記憶から生まれる特別な力。",
        "闇胞子には十分気をつけるのじゃぞ。"
    };

    private static string[] ReturnedHatch() => new[]
    {
        "戻ったか、ミプリン。無事で何よりじゃ。",
        "森の奥で何かが動き始めておる。黄金蜂蜜のかけらは近いかもしれん。",
        "次は、森で見たものを村のみんなにも聞いてみるとよい。"
    };

    private static string[] InitialMiel() => new[] { "女王さまの気配が、黄金蜂蜜の香りに隠れておるわ。", "まずは長老ハッチの話を聞きなさい。" };
    private static string[] QuestMiel() => new[] { "南の森へ向かうなら、光る花を目印にしなさい。", "闇胞子は、迷いと怖さに寄ってくるわ。" };
    private static string[] ReturnedMiel() => new[] { "森の風が変わったわ。", "あなたが戻ったことで、細い記憶の糸が村へつながったみたい。" };

    private static string[] InitialMarche() => new[] { "いらっしゃい、ミプリンちゃん。今日はまだお店の準備中だよ。", "森へ行く前に、長老さまの話を聞いておいで。" };
    private static string[] QuestMarche() => new[] { "森へ行くなら、はちみつポットを忘れずにね。", "お店はまだ仮営業だけど、そのうち道具も並べるよ。" };
    private static string[] ReturnedMarche() => new[] { "おかえり、ミプリンちゃん。顔を見るだけで安心するよ。", "次は小さなはちみつ瓶くらい用意しておきたいね。" };

    private static string[] InitialBee() => new[] { "ミプリン、今日はどこへ行くの？", "長老さまが呼んでたよ。ちょっと真面目な顔だった。" };
    private static string[] QuestBee() => new[] { "ぼくも森を見てきたけど、変なキノコが増えてたよ。", "ミプリンならきっと大丈夫。無理だけはしないでね。" };
    private static string[] ReturnedBee() => new[] { "本当に森へ行ってきたの？すごい！", "ぼくもいつか、ミプリンみたいに飛べるかな。" };

    private static string[] InitialPolle() => new[] { "花粉の流れがいつもと違うの。", "黄金蜂蜜の話なら、長老ハッチが詳しいわ。" };
    private static string[] QuestPolle() => new[] { "黄金蜂蜜のかけらは、花の記憶を呼び起こすって聞いたわ。", "森で光る花を見つけたら、よく覚えておいて。" };
    private static string[] ReturnedPolle() => new[] { "あなたが森から戻って、花壇の色が少し明るくなった気がするの。", "記憶と蜂蜜は、やっぱりつながっているのね。" };

    private static string[] InitialNavii() => new[] { "困ったら村のみんなに話しかけてみよう。", "まずは長老ハッチ、それから南の森だね。" };
    private static string[] QuestNavii() => new[] { "南の森へ行けるようになったよ。", "入口でEかSpaceを押せば出発できるはず。" };
    private static string[] ReturnedNavii() => new[] { "村に戻る流れも確認できたね。", "次は森で何を持ち帰るか決める段階だよ。" };

    private static string[] InitialGrandpa() => new[] { "昔、この村の南には黄金色に光る森があったんじゃ。", "長老の話をよく聞くんじゃぞ。" };
    private static string[] QuestGrandpa() => new[] { "女王さまが消えてから、森の奥が少しずつ変わってしもうた。", "おぬしの両親も、かつてあの森へ向かった。" };
    private static string[] ReturnedGrandpa() => new[] { "戻ったか。あの森の空気を知った目をしておる。", "父タイガと母ハナの話も、いずれせねばならんのう。" };

    private static string[] InitialBoard() => new[] { "現在の目標：長老ハッチに話を聞く。", "南の森の入口は、まだ調査前です。" };
    private static string[] QuestBoard() => new[] { "現在の目標：南の森を調べる。", "南の森への道からCharacterTestへ移動できます。" };
    private static string[] ReturnedBoard() => new[] { "現在の目標：森から戻ったことを村のみんなに報告する。", "次の実装候補：黄金蜂蜜のかけらAを入手する。" };
}
