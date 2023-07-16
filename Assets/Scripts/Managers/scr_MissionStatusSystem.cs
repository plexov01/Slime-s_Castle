using System.Collections.Generic;
using UnityEngine;

public class scr_MissionStatusSystem : MonoBehaviour
{
    public Dictionary<string, missionStatus> missions = new Dictionary<string, missionStatus>();

    private string[] missionsToInit = {"q_MyFirstQuest","m_MyFirstMission"};

    public static scr_MissionStatusSystem instance;

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
        InitMissions();

        scr_EventSystem.instance.playerEnteredMissionTrigger.AddListener(ChangeMissionStatus);
    }


    private void ChangeMissionStatus(string missionName, missionStatus status)
    {
        missions[missionName] = status;
    }


    public string CheckMissionStatus(List<string> missionList)
    {
        string missionStatus = "";
        foreach (var mission in missionList)
        {
            var tmp = missions[mission];
            missionStatus += (int)tmp;
        }
        return missionStatus;
    }

    private void InitMissions()
    {
        foreach (var mission in missionsToInit)
        {
            missions.Add(mission,missionStatus.notAssigned);
        }
    }

    private void OnDestroy()
    {
        scr_EventSystem.instance.playerEnteredMissionTrigger.RemoveListener(ChangeMissionStatus);
    }
}

public enum missionStatus
{
    notAssigned,
    inProgress,
    achieved,
    completed,
    failed
}
