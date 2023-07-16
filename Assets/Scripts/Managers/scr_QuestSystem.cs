using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class scr_QuestSystem : MonoBehaviour
{
    public static scr_QuestSystem instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }
    
    private void Start()
    {
        SetupQuestFile("q_MyFirstQuestTemplate");
        // Debug.Log(Application.streamingAssetsPath);
        // Debug.Log(Application.dataPath);

    }

    public void AssignQuest(string questName)
    {
        //if name is incorrect?
        var missionDictionary = scr_MissionStatusSystem.instance.missions;
        missionDictionary[questName] = missionStatus.inProgress;
        AddQuest(questName);
    }

    private void AddQuest(string questName)
    {
        Quest questComponent = gameObject.AddComponent<Quest>();
        questComponent.FillQuestAttributes(GetQuest(questName));
    }

    public void CompleteQuest(string questName)
    {
        scr_MissionStatusSystem.instance.missions[questName] = missionStatus.achieved;
        
    }

    public QuestSerializable GetQuest(string questName)
    {
        if (QuestExists(questName))
        {
            string path = Application.streamingAssetsPath + "/Quests/" + questName + ".json";
            QuestSerializable quest = JsonUtility.FromJson<QuestSerializable>(File.ReadAllText(path));
            return quest;
        }
        else
        {
            return new QuestSerializable();
        }

    }

    public bool QuestExists(string quest)
    {
        string path = Application.streamingAssetsPath + "/Quests/" + quest + ".json";
        return File.Exists(path);
    }
    
    public void SetupQuestFile(string questName)
    {
        if (QuestExists(questName))
        {
            Debug.Log(questName + "Quest file exists");
        }
        else
        {
            QuestSerializable quest = new QuestSerializable();
            quest.goals.Add(new KillGoal());
            quest.goals.Add(new GatheringGoal());

            string data = JsonUtility.ToJson(quest);
            string path = Application.streamingAssetsPath + "/Quests/" + questName + ".json";

            System.IO.File.WriteAllText(path, data);
        }

    }
}
