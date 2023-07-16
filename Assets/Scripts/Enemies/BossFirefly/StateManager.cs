using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public State currentState;
    State nextState;

    void Update()
    {
        RunStateMachineLogic();
    }

    private void FixedUpdate()
    {
        RunStateMachinePhysics();
    }

    private void RunStateMachineLogic()
    {
        if (currentState != null)
        {
            nextState = currentState.RunCurrentStateLogic();

            if (nextState != null && nextState != currentState)
            {
                SwitchState(nextState);
            }
        }
    }

    private void RunStateMachinePhysics()
    {
        currentState.RunCurrentStatePhysics();
    }

    private void SwitchState(State nextState)
    {
        print(currentState.name + " exit");
        currentState.Exit();
        currentState = nextState;
        print(currentState.name + " enter");
        currentState.Enter();
    }

    private void OnGUI()
    {
        string content = currentState != null ? currentState.name : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}
