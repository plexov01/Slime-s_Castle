using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    [SerializeField] protected List<string> myMissions = new List<string>();
    [SerializeField] protected List<string> myQuests = new List<string>();
    [SerializeField] protected List<string> myQuestsRequiredState = new List<string>();
    [SerializeField] private int currentQuestIndex = 0;

    [SerializeField] protected bool playerIsClose = false;

    public void Interact(InputAction.CallbackContext context)
    {
        if (playerIsClose)
        {
            string myState = scr_MissionStatusSystem.instance.CheckMissionStatus(myMissions);
            DialogueManager.instance.StartDialogue("dlg_"+gameObject.name+"_"+myState);
            TryAssignNextQuest(myState);
        }
    }

    protected void TryAssignNextQuest(string myState)
    {
        if (currentQuestIndex < myQuests.Count)
        {
            if (myQuestsRequiredState[currentQuestIndex] == myState)
            {
                scr_QuestSystem.instance.AssignQuest(myQuests[currentQuestIndex]);
                currentQuestIndex += 1;
            }
        }
    }

    protected virtual void Start()
    {
        InputManager.instance.playerInput.actions["Interaction"].performed += Interact;
    }

    protected void OnDestroy()
    {
        InputManager.instance.playerInput.actions["Interaction"].performed -= Interact;
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
        }

    }
}

