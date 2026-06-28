using UnityEngine;

public enum StoryStage
{
    NotStarted = 0,
    QuestStarted = 1,
    FirstAdventureReturned = 2,
    GoldenHoneyShardAObtained = 3
}

public class StoryProgress : MonoBehaviour
{
    private static StoryProgress instance;

    [SerializeField] private StoryStage currentStage = StoryStage.NotStarted;

    public static StoryProgress Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StoryProgress>();
            }

            if (instance == null)
            {
                GameObject storyObject = new GameObject("StoryProgress");
                instance = storyObject.AddComponent<StoryProgress>();
            }

            return instance;
        }
    }

    public StoryStage CurrentStage => currentStage;
    public bool IsQuestStarted => currentStage >= StoryStage.QuestStarted;
    public bool HasReturnedFromFirstAdventure => currentStage >= StoryStage.FirstAdventureReturned;
    public bool HasGoldenHoneyShardA => currentStage >= StoryStage.GoldenHoneyShardAObtained;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartFirstForestQuest()
    {
        AdvanceTo(StoryStage.QuestStarted);
    }

    public void MarkFirstAdventureReturned()
    {
        AdvanceTo(StoryStage.FirstAdventureReturned);
    }

    public void ObtainGoldenHoneyShardA()
    {
        AdvanceTo(StoryStage.GoldenHoneyShardAObtained);
    }

    public void ResetStoryForTest()
    {
        currentStage = StoryStage.NotStarted;
    }

    private void AdvanceTo(StoryStage nextStage)
    {
        if (nextStage <= currentStage)
        {
            return;
        }

        currentStage = nextStage;
        Debug.Log("StoryProgress advanced to: " + currentStage);
    }
}
